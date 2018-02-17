using Mystery.Register;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mystery.Content
{
    public class InMemoryContainer : IContentContainer
    {
        private IDictionary<Guid, IContent> _storage = new ConcurrentDictionary<Guid, IContent>();

        public void Dispose()
        {
            _storage = null;
        }

        public int Count{get{return _storage.Count;}}

        public bool IsReadOnly { get { return false; } }

        public void Add(IContent item)
        {
            IContent current = _storage.ContainsKey(item.guid) ? _storage[item.guid] : null;

            //we already have it
            if (ReferenceEquals(item, current))
                return;

            //we don't have it
            if(current == null){
                _storage[item.guid] = item;
                return;
            }

            //we had a different instance, and the values are different, 
            //we update to the given one
            if (!item.samePropertiesValue(current)) {
                //we have it but the property have changed
                _storage[item.guid] = item;
                return;
            }

            //we had a different instance, and the values are the same, 
            //we update to the given one
            _storage[item.guid] = item;
        }

        public void AddContents(IEnumerable<IContent> contents)
        {
            foreach (IContent c in contents) this.Add(c);
        }

        public void Clear()
        {
            _storage = new Dictionary<Guid, IContent>();
        }

        public bool Contains(IContent item)
        {
            return _storage.ContainsKey(item.guid);
        }

        public bool ContainsType<T>() where T : IContent
        {
            foreach (IContent c in _storage.Values)
                if (c is T) return true;
            return false;
        }

        public void CopyTo(IContent[] array, int arrayIndex)
        {
            _storage.Values.CopyTo(array, arrayIndex);
        }

        

        public IEnumerable<T> GetAll<T>() where T : IContent, new()
        {
            return (from IContent c in _storage.Values where c is T select (T)c);
        }

        public IEnumerable<IContent> GetAllContents()
        {
            return _storage.Values;
        }

        public IEnumerable<T> GetAllByFilter<T>(Expression<Func<T, bool>> filter) where T : IContent, new()
        {
            var function = filter.Compile();
            return (from IContent c in _storage.Values
                    where typeof(T).IsAssignableFrom(c.GetType()) && function((T)c) select (T)c);
        }

        public T GetContent<T>(Guid guid) where T : IContent, new()
        {
            if (!_storage.ContainsKey(guid)) return default(T);
            return (T)_storage[guid];
        }

        public IEnumerator<IContent> GetEnumerator()
        {
            return _storage.Values.GetEnumerator();
        }

        public IEnumerable<LightContentReferece> GetLightContentRereferece<T>() where T : IContent, new()
        {
            IEnumerable<T> contents = this.GetAll<T>();
            ICollection<LightContentReferece> result = new LinkedList<LightContentReferece>();
            foreach (T c in contents)
            {
                string rt = c.ReferenceText;
                if (string.IsNullOrEmpty(rt)) continue;
                result.Add(new LightContentReferece
                {
                    guid = c.guid,
                    ReferenceText = rt
                });
            }
            return result;
        }

        public bool Remove(IContent item)
        {
            return _storage.Remove(item.guid);
        }
        public void RemoveContents(IEnumerable<IContent> contents)
        {
            foreach (var item in contents)
                _storage.Remove(item.guid);
        }

        /// <summary>
        /// return by matching all the given words
        /// </summary>
        /// <param name="search_text"></param>
        /// <param name="max_results">ignored</param>
        /// <returns></returns>
        public IEnumerable<LightContentReferece> Search(string search_text, int max_results)
        {
            if (string.IsNullOrEmpty(search_text)) return new List<LightContentReferece>();
            search_text = search_text.Trim().ToLower();
            List<string> words = new List<string>(search_text.Split(' '));
            if (string.IsNullOrEmpty(search_text)) return new List<LightContentReferece>();
            ICollection<LightContentReferece> result = new LinkedList<LightContentReferece>();
            foreach (IContent c in _storage.Values) {
                string rt = c.ReferenceText;
                if (string.IsNullOrEmpty(rt)) continue;
                bool mach = (from string x in words where rt.Contains(x) select x).Count() == words.Count;
                if (mach)
                    result.Add(new LightContentReferece(c));
            }
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _storage.Values.GetEnumerator();
        }
    }
    
}
