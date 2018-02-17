using MongoDB.Bson.Serialization.Attributes;
using Mystery.Register;
using System;
using System.ComponentModel;

namespace Mystery.Content
{

    [ContentType()]
    [Serializable()]
    public class BaseContent : IContent, ISupportInitialize
    {

        [BsonId]
        public Guid guid { get; set; } = Guid.NewGuid();

        public event ContentEventHandler contentSealed;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is IContent))
                return false;
            return guid == ((IContent)obj).guid;
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        /// <summary>
        /// make the content ready to be sent into a storage, network etc
        /// </summary>
        void IContent.seal()
        {
            seal();
            contentSealed?.Invoke(this);

        }

        protected virtual void seal() { }

        [BsonElement]
        public string ReferenceText
        {
            get
            {
                ReferenceText ref_text = this.GetType().getFirstAttribute<ReferenceText>();
                if (ref_text != null)
                    return ref_text.getReferenceText(this);
                return null;
            }
        }
        [BsonElement]
        public string SearchText
        {
            get
            {
                SearchText search_attr = this.GetType().getFirstAttribute<SearchText>();
                if (search_attr != null)
                    return search_attr.getSearchText(this);
                return ReferenceText;
            }
        }

        public override string ToString()
        {
            var ref_text = this.ReferenceText;
            if (!string.IsNullOrEmpty(ref_text) )
                return ref_text;
            return base.ToString();
        }

        protected bool Initializing;
        public void BeginInit()
        {
            Initializing = true;
        }

        public void EndInit()
        {
            Initializing = false;
        }

        public static bool operator ==(BaseContent x, IContent y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals( x , null) && ReferenceEquals( y , null)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
            return x.guid == y.guid;
        }
        public static bool operator !=(BaseContent x, IContent y)
        {
            if (ReferenceEquals(x, y)) return false;
            if (ReferenceEquals(x, null) && ReferenceEquals(y, null)) return false;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return true;
            return x.guid != y.guid;
        }

    }

}