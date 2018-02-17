using Mystery.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using MongoDB.Driver;
using Mystery.Register;
using System.Linq.Expressions;

namespace Mystery.Json
{
    public class MongoSearchResult {
    }

    abstract public class MongoContentCollection
    {
        abstract public long Count{ get; }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        abstract public void Add(IContent item);

        abstract public void AddContents(IEnumerable<IContent> contents);

        abstract public void Clear();

        abstract public bool Contains(IContent item);

        abstract public bool isEmpty { get; }

        abstract public IEnumerable<IContent> GetAll();

        abstract public IEnumerable<IContent> GetAllByFilter(LambdaExpression filter);

        abstract public IContent GetContent(Guid guid);


        abstract public IEnumerable<LightContentReferece> GetLightContentRereferece();

        abstract public IEnumerable<LightContentReferece> Search(string search_text, int max_results);

        abstract public bool Remove(IContent item);


        abstract public void RemoveContents(IEnumerable<IContent> items);

        
    }

    /// <summary>
    /// implement the content dispatcher over a specific type, poxing a mongodb collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoContentCollection<T>: MongoContentCollection where T : BaseContent, new()
    {

        private IMongoCollection<T> _collection;
        private IMongoCollection<T> _deleted_collection;
        private IMongoCollection<LightContentReferece> _light_contents_collection;

        private FilterDefinition<T> all = Builders<T>.Filter.Empty;

         public MongoContentCollection() {
            var content_type_name = typeof(T).getMysteryAttribute<ContentType>().name;
            var mongo_db = this.getGlobalObject<MysteryMongoDb>().content_db;
            _collection = mongo_db.GetCollection<T>(content_type_name);
            _deleted_collection = mongo_db.GetCollection<T>("Deleted" + content_type_name);
            _collection.Indexes.CreateOne(Builders<T>.IndexKeys.Text(x => x.SearchText));
            _light_contents_collection = mongo_db.GetCollection<LightContentReferece>(content_type_name);
        }

        override public long Count
        {
            get
            {
                return _collection.Count(all);
            }
        }


        override public void Add(IContent item)
        {
            _collection.ReplaceOne(x=>x.guid == item.guid,(T)item,new UpdateOptions() { IsUpsert= true});
        }

        override public void AddContents(IEnumerable<IContent> contents)
        {
            IEnumerable<WriteModel<T>> requests =
                from x in contents
                where x != null && x.GetType() == typeof(T)
                select new ReplaceOneModel<T>(Builders<T>.Filter.Eq(y => y.guid, x.guid), (T)x) {IsUpsert = true };

            _collection.BulkWrite(requests);
        }

        public override void RemoveContents(IEnumerable<IContent> items)
        {
            //moving them into the deleted collection
            IEnumerable<WriteModel<T>> copy =
                from x in items
                where x != null && x.GetType() == typeof(T)
                select new ReplaceOneModel<T>(Builders<T>.Filter.Eq(y => y.guid, x.guid), (T)x) { IsUpsert = true };

            _deleted_collection.BulkWrite(copy);

            IEnumerable<WriteModel<T>> requests =
                from x in items
                where x != null && x.GetType() == typeof(T)
                select new DeleteOneModel<T>(Builders<T>.Filter.Eq(y => y.guid, x.guid));
            
            _collection.BulkWrite(requests);
        }

        override public void Clear()
        {
            _collection.DeleteMany(all);
        }

        override public bool Contains(IContent item)
        {
            return _collection.Count(x=>x.guid == item.guid)>0;
        }

        override public bool isEmpty { get
            {
                return _collection.Count(all)==0;
            }
        }


        override public IEnumerable<IContent> GetAll() 
        {

            return (from x in  _collection.AsQueryable()
                    select (IContent)x);
        }

        override public IEnumerable<IContent> GetAllByFilter(LambdaExpression filter) 
        {
            Expression<Func<T, bool>> casted;
            if (filter is Expression<Func<T, bool>>)
            {
                casted = (Expression<Func<T, bool>>)filter;
            }
            else
            {
                casted = Expression.Lambda<Func<T, bool>>(filter.Body, filter.Parameters);
            }

            return (from x in _collection.Find(casted).ToEnumerable()
                    select x);
        }

        override public IContent GetContent(Guid guid) 
        {
            //only one which checks deleted ones to
            var result = _collection.Find(x => x.guid == guid).SingleOrDefault();
            if(result == null)
                result = _deleted_collection.Find(x => x.guid == guid).SingleOrDefault();
            return result;
        }

        override public IEnumerable<LightContentReferece> GetLightContentRereferece() 
        {
            return _light_contents_collection.AsQueryable();
        }

        override public bool Remove(IContent item)
        {
            _deleted_collection.ReplaceOne(x => x.guid == item.guid, (T)item, new UpdateOptions() { IsUpsert = true });
            return _collection.DeleteOne(x => x.guid == item.guid).DeletedCount > 0;
        }

        public override IEnumerable<LightContentReferece> Search(string search_text, int max_results)
        {
            
            var F = Builders<LightContentReferece>.Filter.Text(search_text);
            var P = Builders<LightContentReferece>.Projection.MetaTextScore(nameof(LightContentReferece.TextMatchScore));
            var S = Builders<LightContentReferece>.Sort.MetaTextScore(nameof(LightContentReferece.TextMatchScore));
            var result = _light_contents_collection.Find(F)
                .Project<LightContentReferece>(P).
                Sort(S).Limit(max_results).ToList();
            var content_type_name = typeof(T).getMysteryAttribute<ContentType>().name;
            foreach (var lr in result) {
                lr.url = content_type_name + '/' + lr.guid.Tiny();
            }
            return result;
        }
    }
}
