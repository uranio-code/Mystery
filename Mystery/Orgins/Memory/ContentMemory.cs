
using Mystery.Content;
using Mystery.Json;
using Mystery.Register;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// at this stage I want to try without cache
/// </summary>
/// <remarks></remarks>
[GlobalAvalibleObjectImplementation(implementation_of = typeof(IContentDispatcher), overrides_exsisting = false, singleton = true)]
public class ContentMemory : IContentDispatcher
{

	private IContentDispatcher _db ;

	private ConcurrentDictionary<Guid, IContent> _cache;

    public void Dispose()
    {
        _db = null;
        _cache = null;
    }

    public ContentMemory()
	{
        _cache = new ConcurrentDictionary<Guid, IContent>();
        _db = this.getGlobalObject<ContentsDatabase>();
    }



	public void AddContents(IEnumerable<IContent> contents)
	{
		_db.AddContents(contents);
        foreach (var item in contents)
            if (item != null)
                _cache[item.guid] = item;
	}

    public void RemoveContents(IEnumerable<IContent> contents)
    {
        _db.RemoveContents(contents);
        IContent actually_removed;
        foreach (var item in contents)
            if (item != null)
                _cache.TryRemove(item.guid,out actually_removed);
    }


    public IEnumerable<T> GetAll<T>() where T : IContent, new()
    {
        //we take them from the db
        var result = new List<T>( _db.GetAll<T>());
        //but for each of them if we have it in memory we return our
        for (int i = 0; i < result.Count; i++) {
            if (_cache.ContainsKey(result[i].guid))
                result[i] = (T)_cache[result[i].guid];
        }
        return result;
	}
    public IEnumerable<IContent> GetAllContents()
    {
        return (from x in this.GetAll<BaseContent>() select (IContent)x);
    }

    public IEnumerable<T> GetAllByFilter<T>(Expression<Func<T, bool>> filter) where T : IContent, new()
    {
        //we take them from the db
        var result = new List<T>(_db.GetAllByFilter(filter));
        //but for each of them if we have it in memory we return our
        for (int i = 0; i < result.Count; i++)
        {
            if (_cache.ContainsKey(result[i].guid))
                result[i] = (T)_cache[result[i].guid];
        }
        return result;
	}

	public T GetContent<T>(System.Guid guid) where T : IContent, new()
    {
		bool in_cache = _cache.ContainsKey(guid);
		if (in_cache)
			return (T)_cache[guid];
		IContent c = _db.GetContent<T>(guid);
		_cache[guid] = c;
		return (T)c;
	}

	public void Add(IContent item)
	{
		_db.Add(item);
        if (item != null) {
            _cache[item.guid] = item;
        }
	}

	public void Clear()
	{
        _cache.Clear();
        _db.Clear();
    }

	public bool Contains(IContent item)
	{
        if (item == null) return false;
		return _cache.ContainsKey(item.guid) || _db.Contains(item);
	}

	public void CopyTo(IContent[] array, int arrayIndex)
	{
        _db.CopyTo(array, arrayIndex);
	}

	public int Count {
		get { return _db.Count; }
	}

	public bool IsReadOnly {
		get { return false; }
	}

	public bool Remove(IContent item)
	{
        IContent in_cache;
        _cache.TryRemove(item.guid, out in_cache);
        return _db.Remove(item);
	}

	public System.Collections.Generic.IEnumerator<IContent> GetEnumerator()
	{
		return _db.GetEnumerator();
	}

	public System.Collections.IEnumerator GetEnumerator1()
	{
		return _db.GetEnumerator();
	}
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return GetEnumerator1();
	}

	public bool ContainsType<T>() where T : IContent
    {
		return _db.ContainsType<T>();
	}
	

	public IEnumerable<LightContentReferece> GetLightContentRereferece<T>() where T : IContent, new()
    {
		return _db.GetLightContentRereferece<T>();
	}

	public IEnumerable<LightContentReferece> Search(string search_text, int max_results)
	{
		return _db.Search(search_text,max_results);
	}

    
}
