using Mystery.Content;
using Mystery.Properties;
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson;

namespace Mystery.Json
{
    [GlobalAvalibleObjectImplementation(singleton = true)]
    public class ContentsDatabase : IContentDispatcher
    {

        /// <summary>
        /// map for collection, using object as each will be an IMongoCollection<ContentType>
        /// </summary>
        private Dictionary<Type, MongoContentCollection> _collections = new Dictionary<Type, MongoContentCollection>();
        private Dictionary<Type, IEnumerable<Type>> child_types = new Dictionary<Type, IEnumerable<Type>>();
        private ICollection<MongoContentCollection> _searchable = new LinkedList<MongoContentCollection>();
        public ContentsDatabase()
        {

            

            foreach (Type content_type in ContentType.getAllContentTypes())
            {
                var coll = (MongoContentCollection)Activator.CreateInstance(
                    typeof(MongoContentCollection<>).MakeGenericType(content_type));
                _collections[content_type] = coll;
                if (content_type.getFirstAttribute<SearchText>() != null) {
                    _searchable.Add(coll);
                }
            }
            var all_content_types = this.getMystery().AssemblyRegister.getTypesMarkedWith<ContentType>();
            child_types[typeof(IContent)] = all_content_types;
            foreach (Type type in all_content_types)
            {
                ContentType content_type = type.getMysteryAttribute<ContentType>();
                HashSet<Type> all_types = new HashSet<Type> { type };
                foreach (Type child_type in this.getMystery().AssemblyRegister.getChildTypes(type))
                {
                    ContentType child_content_type = child_type.getMysteryAttribute<ContentType>();
                    if (child_content_type == null)
                        continue;
                    all_types.Add(child_type);
                    //actually here we could detected some unregistered type which will
                    //cause issue while querying
                    if (!_collections.ContainsKey(child_type)) {
                        _collections[child_type] = (MongoContentCollection)Activator.CreateInstance(
                            typeof(MongoContentCollection<>).MakeGenericType(child_type));
                    }
                }
                child_types[type] = all_types;
            }
        }

        public void Dispose()
        {
            _collections = null;
        }

      

        public void AddContents(IEnumerable<IContent> contents)
        {
            //we divide by types
            var types_collections = new Dictionary<Type, ICollection<IContent>>();
            foreach (IContent c in contents)
            {
                if (c == null) continue;
                var this_content_type = c.GetType();
                if (!types_collections.ContainsKey(this_content_type))
                    types_collections[this_content_type] = new LinkedList<IContent>();
                types_collections[this_content_type].Add(c);
            }

            //content going to db, time to seal them
            foreach (IContent c in contents)
                c.seal();

            foreach (Type content_type in types_collections.Keys) {
                _collections[content_type].AddContents(types_collections[content_type]);
            }

        }

        public void RemoveContents(IEnumerable<IContent> contents)
        {
            //we divide by types
            var types_collections = new Dictionary<Type, ICollection<IContent>>();
            foreach (IContent c in contents)
            {
                if (c == null) continue;
                var this_content_type = c.GetType();
                if (!types_collections.ContainsKey(this_content_type))
                    types_collections[this_content_type] = new LinkedList<IContent>();
                types_collections[this_content_type].Add(c);
            }

            foreach (Type content_type in types_collections.Keys)
            {
                _collections[content_type].RemoveContents(types_collections[content_type]);
            }

        }

        public IEnumerable<T> GetAll<T>() where T : IContent, new()
        {
            var type = typeof(T);

            ContentType content_type = type.getMysteryAttribute<ContentType>();
            if (content_type == null)
                throw new Exception(typeof(T).FullName + " is not a content type");
            

            IEnumerable<T> result = new List<T>();

            foreach (Type child_type in child_types[type])
            {
                var collection = _collections[child_type];
                result = result.Union(from x in  collection.GetAll() select (T)x);
            }

            
            return result;
        }

        

        public bool can_do_crazy { get; set; } = false;



        public T GetContent<T>(System.Guid guid) where T : IContent, new()
        {
            foreach (Type child_type in child_types[typeof(T)])
            {
                var result = _collections[child_type].GetContent(guid);
                if (result != null) return (T)result;
            }
            return default(T);

        }

        public void Add(IContent item)
        {
            if (item == null)
                return;
            Type content_type = item.GetType();
            item.seal();
            _collections[content_type].Add(item);
            
        }

        public void Clear()
        {
            if (!can_do_crazy)
                throw new CrazyImplementation("you can not wipe your database from the api");
            foreach (var coll in _collections.Values) {
                coll.Clear();
            }
        }

        public bool Contains(IContent item)
        {
            if (item == null)
                return false;
            return _collections[item.GetType()].Contains(item);
        }

        public void CopyTo(IContent[] array, int arrayIndex)
        {
            if (!can_do_crazy)
                throw new CrazyImplementation("come on you should really not take in account all the contents of the database");
            var all = new List<IContent>(GetAllContents());
            all.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return (int)(from x in _collections.Values select x.Count).Sum();
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IContent item)
        {
            if (item == null)
                return false;
            Type content_type = item.GetType();
            return _collections[content_type].Remove(item);
        }

        public IEnumerator<IContent> GetEnumerator()
        {
            if (!can_do_crazy)
                throw new CrazyImplementation("come on you should really not take in account all the contents of the database");
            return this.GetAll<BaseContent>().GetEnumerator();
        }

        public IEnumerator GetEnumerator1()
        {
            if (!can_do_crazy)
                throw new CrazyImplementation("come on you should really not take in account all the contents of the database");
            return this.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsType<T>() where T : IContent
        {
            return _collections.ContainsKey(typeof(T)) && !_collections[typeof(T)].isEmpty;
        }


        
        public IEnumerable<LightContentReferece> GetLightContentRereferece<T>() where T : IContent, new()
        {

            IEnumerable<LightContentReferece> result = new List<LightContentReferece>();

            foreach (Type child_type in child_types[typeof(T)])
            {
                var collection = _collections[child_type];
                result = result.Union(collection.GetLightContentRereferece());
            }

            return result;

        }


        private const int max_search_result = 20;
        public IEnumerable<LightContentReferece> Search(string search_text,int max_results)
        {
            var results = new List<List<LightContentReferece>>(_searchable.Count);
            foreach (var coll in _searchable) {
                results.Add(new List<LightContentReferece>(coll.Search(search_text, max_results)));
            }
            var all_togheter = new List<LightContentReferece>((from x in results select x.Count).Sum());
            foreach (var result in results)
                all_togheter.AddRange(result);
            return all_togheter.OrderByDescending(x => x.TextMatchScore).Take(max_results);
        }

        public IEnumerable<T> GetAllByFilter<T>(Expression<Func<T, bool>> filter) where T : IContent, new()
        {
            IEnumerable<T> result = new List<T>();

            foreach (Type child_type in child_types[typeof(T)])
            {
                var collection = _collections[child_type];
                result = result.Union(from x in collection.GetAllByFilter(filter) select (T)x);
            }

            return result;
        }

        public IEnumerable<IContent> GetAllContents()
        {
            if (!can_do_crazy)
                throw new CrazyImplementation("come on you should really not take in account all the contents of the database");
            return (from x in this.GetAll<BaseContent>() select (IContent)x);
        }
    }
}

