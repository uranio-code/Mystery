using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mystery.Content
{

    /// <summary>
    /// at this stage I want to try without cache
    /// </summary>
    /// <remarks></remarks>
    public class ContentContainerCache : IContentContainer
    {

        private IContentContainer _storage;

        private ConcurrentDictionary<Guid, IContent> _cache;

        public void Dispose()
        {
            _storage = null;
            _cache = null;
        }

        public ContentContainerCache(IContentContainer storage)
        {
            _storage = storage;
            _cache = new ConcurrentDictionary<Guid, IContent>();
        }



        public void AddContents(IEnumerable<IContent> contents)
        {
            _storage.AddContents(contents);
        }



        public IEnumerable<T> GetAll<T>() where T : IContent, new()
        {
            return _storage.GetAll<T>();
        }

        public IEnumerable<T> GetAllByFilter<T>(Expression<Func<T, bool>> filter) where T : IContent, new()
        {
            return _storage.GetAllByFilter<T>(filter);
        }

        public T GetContent<T>(System.Guid guid) where T : IContent, new()
        {
            bool in_cache = _cache.ContainsKey(guid);
            if (in_cache)
                return (T)_cache[guid];
            IContent c = _storage.GetContent<T>(guid);
            _cache[guid] = c;
            return (T)c;
        }

        public void Add(IContent item)
        {
            _storage.Add(item);
            
        }

        public void Clear()
        {
            _cache = new ConcurrentDictionary<Guid, IContent>();
            _storage.Clear();
        }

        public bool Contains(IContent item)
        {
            return _cache.ContainsKey(item.guid) || _storage.Contains(item);
        }

        public void CopyTo(IContent[] array, int arrayIndex)
        {
            _storage.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _storage.Count ; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IContent item)
        {
            return _storage.Remove(item);
        }
        public void RemoveContents(IEnumerable<IContent> contents)
        {
            _storage.RemoveContents(contents);
        }


        public IEnumerator<IContent> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        public IEnumerator GetEnumerator1()
        {
            return _storage.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }

        public bool ContainsType<T>() where T : IContent
        {
            return _storage.ContainsType<T>();
        }


        public IEnumerable<LightContentReferece> GetLightContentRereferece<T>() where T : IContent, new()
        {
            return _storage.GetLightContentRereferece<T>();
        }

        public IEnumerable<LightContentReferece> Search(string search_text, int max_results)
        {
            return _storage.Search(search_text,max_results);
        }

        public IEnumerable<IContent> GetAllContents()
        {
            return _storage.GetAllContents();
        }
    }
}


