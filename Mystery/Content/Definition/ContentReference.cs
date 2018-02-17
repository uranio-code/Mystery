
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Linq;
using Mystery.Register;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Mystery.Users;
using System.Collections;

namespace Mystery.Content
{
    public interface IReferenceProperty {
        IEnumerable<IContent> getAsContentEnum();
    }

    public interface IContentReference: IReferenceProperty
    {
        Guid? guid { get; set; }
        IContent getContent();
        bool isNullOrEmpty();
    }
    
    public class ContentReference<T> : IContentReference where T : IContent, new()
    {

        private T _value;

        public Guid? guid { get; set; }

        [BsonIgnore, JsonIgnore]
        public T value
        {
            get
            {
                if (_value != null) {
                    return _value;
                }
                if (guid != null)
                {
                    _value = this.getGlobalObject<IContentDispatcher>().GetContent<T>(guid.Value);
                }
                return _value;
            }
            set
            {
                _value = value;
                if (_value == null) guid = null;
                else guid = _value.guid;
            }
        }


        public ContentReference()
        {
        }

        public ContentReference(T value)
        {
            this.value = value;
        }

        public static implicit operator ContentReference<T>(T initialData)
        {
            return new ContentReference<T> { value = initialData };
        }

        public static implicit operator T(ContentReference<T> initialData)
        {
            if (initialData == null)
                return default(T);
            return initialData.value;
        }


        public bool isNullOrEmpty() {
            return guid == null || guid.Value == Guid.Empty;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return isNullOrEmpty();
            if (obj is IContentReference) {
                var obj_null = ((IContentReference)obj).isNullOrEmpty();
                if (obj_null) return isNullOrEmpty();
                return ((IContentReference)obj).guid.Equals(guid);
            }
            if (obj is IContent) {
                if (isNullOrEmpty()) return false;
                return ((IContent)obj).guid == guid;
            }
            if (obj is Guid) {
                if ((Guid)obj == Guid.Empty)
                    return isNullOrEmpty();
                return (Guid)obj == guid;
            }
            return false;
        }
        public override int GetHashCode()
        {
            if (isNullOrEmpty())
                return 0;
            return guid.GetHashCode();
        }

        public override string ToString()
        {
            if (value == null)
                return null;
            return value.ToString();
        }

        public IContent getContent()
        {
            return value;
        }

        public IEnumerable<IContent> getAsContentEnum()
        {
            if (this.isNullOrEmpty())
                return new List<IContent>(0);
            var c = this.getContent();
            if(c ==null)
                return new List<IContent>(0);
            return new List<IContent>() { c };
        }

    }


    public class MultiContentReference<T> : List<ContentReference<T>>, IReferenceProperty where T : IContent, new()
    {


        public MultiContentReference() : base()
        { }

        public MultiContentReference(IEnumerable<ContentReference<T>> enumerable):base(enumerable)
        {}

        public MultiContentReference(IEnumerable<T> enumerable) 
            : base(from x in enumerable select new ContentReference<T>(x))
        { }


        public bool Contains(T item) {
            var is_null = item == null || item.guid == Guid.Empty;
            if (is_null)
            {
                foreach (var r in this)
                {
                    if (r == null || r.isNullOrEmpty()) return true;
                }
                return false;
            }
            else { //not null
                foreach (var r in this)
                {
                    if (r.Equals(item)) return true;
                }
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return Count == 0;
            if (!(obj is IEnumerable)) return false;

            var mine_guids = new HashSet<Guid>(from x in this
                                               where x != null && !x.isNullOrEmpty()
                                               select x.guid.Value);

            var other_list = new HashSet<object>(from object x in (IEnumerable)obj where x!=null select x);
            if(other_list.Count==0) return mine_guids.Count == 0;
            //we can have a list of content or guid
            //we are going to detect that by the first element
            var first = other_list.FirstOrDefault();


            //it can not be null or we would have already handled it
            HashSet<Guid> other_guids;

            if (first is IContent)
            {
                other_guids = new HashSet<Guid>(from x in other_list select ((IContent)x).guid);
            }
            else if (first is Guid)
            {
                other_guids = new HashSet<Guid>(from x in other_list
                                                where (Guid)x != Guid.Empty
                                                select (Guid)x);
            }
            else if (first is IContentReference)
            {
                other_guids = new HashSet<Guid>(
                    from x in other_list
                    where !((IContentReference)x).isNullOrEmpty()
                    select ((IContentReference)x).guid.Value);
            }
            else
                return false;

            if (other_guids.Count != mine_guids.Count)
                return false;

            return other_guids.Intersect(mine_guids).Count() == mine_guids.Count;

        }

        public IEnumerable<IContent> getAsContentEnum()
        {
            return (from x in this where !x.isNullOrEmpty()
                    where x.getContent() != null
                    select x.getContent()
                    );
        }

        public IEnumerable<T> getEnum() {
            return (from x in this
                    where !x.isNullOrEmpty()
                    select x.value
                    );
        }

        public override int GetHashCode()
        {
            var mine_guids = new HashSet<Guid>(from x in this
                                               where x!=null && !x.isNullOrEmpty()
                                               select x.guid.Value);
            return (from x in mine_guids select x.GetHashCode()).Sum();
        }

        /// <summary>
        /// to allow easier code query using guids
        /// </summary>
        [BsonElement]
        public IEnumerable<Guid> Guids
        {
            get
            {
                return new List<Guid>(from x in this where !x.isNullOrEmpty() select x.guid.Value);
            }
        }

    }


    public class ContentReference
    {
        public string ContentType { get; set; }
        public Guid guid { get; set; }

        public ContentReference(IContent content)
        {
            if (content != null)
            {
                guid = content.guid;
                ContentType = content.GetType().getMysteryAttribute<ContentType>().name;
            }
        }

        public ContentReference() { }

        public ContentReference(string content_type_name, Guid guid)
        {
            if (Content.ContentType.getType(content_type_name) == null)
                throw new InvalidConstraintException(content_type_name + "  is not a content type");
            this.ContentType = content_type_name;
            if (guid == Guid.Empty)
                throw new InvalidConstraintException(guid + "  must be given");
            this.guid = guid;
        }

        public static ContentReference tryGetContentReferece(string content_type_name, string guid_or_tiny)
        {
            if (Content.ContentType.getType(content_type_name) == null)
                return null;
            Guid parsed = guid_or_tiny.fromTiny();
            if (parsed == Guid.Empty && !Guid.TryParse(guid_or_tiny, out parsed))
                return null;
            if (parsed == Guid.Empty)
                return null;
            return new ContentReference(content_type_name, parsed);
        }

        public IContent getContent() {
            return this.getGlobalObject<IContentDispatcher>().GetContent(ContentType, guid);
        }

    }

}