using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.Content;
using System.Collections;
using MysteryWebLogic.Authetication;
using Mystery.Json;
using Mystery.History;
using Mystery.Web;
using System.Linq.Expressions;

namespace MysteryWebLogic.Content
{



    [GlobalAvalibleObjectImplementation(implementation_of = typeof(IContentDispatcher), overrides_exsisting = true)]
    public class WebContentDispatcher : IContentDispatcher
    {

        IContentContainer shared_container;

        private IContentContainer _my_storage = new InMemoryContainer();
        private IContentContainer _graveyard = new InMemoryContainer();

        private bool _disposing = false;


        private void SuccessfullDispose()
        {
            //time to write history
            MysterySession session = this.getGlobalObject<MysterySession>();
            HistoryEntry he = new HistoryEntry()
            {
                id = WebActionExecutor.current.uid,
                autheticated_user = session.authenticated_user,
                working_for = session.user,
                date = DateTime.Now,
                logs = WebActionExecutor.current.logs,
            };
            he.Removed = new List<IContent>(_graveyard.GetAllContents());
            he.Added = new List<IContent>(_my_storage.Count);


            foreach (IContent c in _my_storage)
            {
                //new? changed? or just accessed?
                IContent orginal = shared_container.GetContent(c.getContenTypeName(), c.guid);
                if (orginal != null)
                {
                    //either is changed or it was not
                    if (!orginal.isJsonEquivalent(c))
                    {
                        //yup it is changed
                        he.NewValues[c.guid.ToString()] = c;
                        he.PreviousValues[c.guid.ToString()] = orginal;
                    }
                    //well it was not changed, so we be being only accessing it
                    //or it was changed to the same values in a different thread
                    //in the case we will ignore it as undoing the action shall not undo that action
                }
                else
                    he.Added.Add(c);
            }

            //it is time to write history
            //always wanted to said that :D

            if (!he.IsEmpty)
                this.getGlobalObject<IHistoryRepository>().Add(he);


            //all right history done, now we need to update for real
            shared_container.AddContents(he.Added);
            //since we computed what really changed we can save some work to the shared container
            shared_container.AddContents(he.NewValues.Values);
            shared_container.RemoveContents(he.Removed);
        }


        private void ErroneousDispose()
        {
            //fun fact
            //in case of exception we don't actuall need to do anything (at the moment)
            //the WebContentDispatcher is going to dispose and none of its contents are going to be in the db
        }

        public void Dispose()
        {
            _disposing = true;
            switch (WebActionExecutor.current.status)
            {
                case WebActionExecutorStatus.done:
                case WebActionExecutorStatus.authorizing:
                    SuccessfullDispose();
                    break;
                case WebActionExecutorStatus.error:
                    ErroneousDispose();
                    break;
                default:
                    throw new Exception("the " + nameof(WebActionExecutor) + " is disposing with status=" + WebActionExecutor.current.status.ToString() + " don't know what is happing here, please debug");
                    break;
            }


            //see you next request, thanks for using us
            _original_values = null;
            shared_container = null;
            _my_storage = null;
            _graveyard = null;
        }



        public int Count
        {
            get
            {
                int only_mines = (from IContent x in _my_storage
                                  where shared_container.GetContent(x.getContenTypeName(), x.guid) == null
                                  select 1).Count();
                return shared_container.Count + only_mines - _graveyard.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _disposing || shared_container.IsReadOnly;
            }
        }

        public WebContentDispatcher(IContentContainer shared_container)
        {
            this.shared_container = shared_container;
        }

        public void Add(IContent item)
        {
            if (WebActionExecutor.current.status != WebActionExecutorStatus.executing)
                throw new UnauthorizedAccessException();
            if (_disposing)
                throw new ObjectDisposedException(nameof(WebContentDispatcher));
            _graveyard.Remove(item);
            _my_storage.Add(item);
        }

        public void AddContents(IEnumerable<IContent> contents)
        {
            if (_disposing)
                throw new ObjectDisposedException(nameof(WebContentDispatcher));
            //we need to always pass by add and remove to feed our container correctly
            foreach (IContent c in contents) Add(c);
        }

        public void Clear()
        {
            if (_disposing)
                throw new ObjectDisposedException(nameof(WebContentDispatcher));
            _my_storage.Clear();
            _graveyard.Clear();
        }

        public bool Contains(IContent item)
        {
            return !_graveyard.Contains(item) && (_my_storage.Contains(item) || shared_container.Contains(item));
        }

        public bool ContainsType<T>() where T : IContent
        {
            //it would be a terrible performance loss to count the _graveyard here..
            return _my_storage.ContainsType<T>() || shared_container.ContainsType<T>();
        }

        public void CopyTo(IContent[] array, int arrayIndex)
        {
            HashSet<IContent> all_contents = new HashSet<IContent>(_my_storage.GetAllContents()
                .Union(shared_container.GetAllContents()).Except(_graveyard.GetAllContents()));
            all_contents.CopyTo(array, arrayIndex);
        }

        public IEnumerable<T> GetAll<T>() where T : IContent, new()
        {
            //we first create a collection out of our composition
            //then we return a linq enum, so it will pass back here if really necessary
            //getting the content one by one it essential to create a copy in our storage

            IEnumerable<T> result = new HashSet<T>(_my_storage.GetAll<T>()
                .Union(shared_container.GetAll<T>()).Except(_graveyard.GetAll<T>()));

            return (from x in result select (T)this.GetContent(x.getContenTypeName(), x.guid));
        }

        public IEnumerable<T> GetAllByFilter<T>(Expression<Func<T, bool>> filter) where T : IContent, new()
        {
            // we first create a collection out of our composition
            //then we return a linq enum, so it will pass back here if really necessary
            //getting the content one by one it essential to create a copy in our storage
            var in_my_storage = new HashSet<T>(_my_storage.GetAllByFilter(filter));
            var in_db = new HashSet<T>(shared_container.GetAllByFilter(filter));
            var in_graveyard = new HashSet<T>(_graveyard.GetAllByFilter(filter));

            IEnumerable<T> result = in_my_storage.Union(in_db).Except(in_graveyard);

            return (from x in result select this.GetContent<T>(x.guid));
        }

        private Dictionary<Guid, string> _original_values = new Dictionary<Guid, string>();

        public T GetContent<T>(Guid guid) where T : IContent, new()
        {
            T mine = _my_storage.GetContent<T>(guid);
            if (mine != null) return mine;

            //we what a "local" copy to have the current action edit this copy
            T common = shared_container.GetContent<T>(guid);
            if (common == null) return default(T);

            //we use json as history tracking, since we do that
            //we can also use it to instance the new content
            MysteryJsonConverter js = this.getGlobalObject<MysteryJsonConverter>();
            string json = js.getJson(common);
            _original_values[common.guid] = json;
            mine = js.readJson<T>(json);

            //from now on we should hit our storage for this content
            _my_storage.Add(mine);
            return mine;
        }

        public IEnumerator<IContent> GetEnumerator()
        {
            throw new NotImplementedException("don't");
        }

        public IEnumerable<LightContentReferece> GetLightContentRereferece<T>() where T : IContent, new()
        {
            //for now let's ignore local light reference
            return shared_container.GetLightContentRereferece<T>();
        }

        public void RemoveContents(IEnumerable<IContent> items)
        {
            if (_disposing)
                throw new ObjectDisposedException(nameof(WebContentDispatcher));
            //we have to feed the different containers
            //webcontentdispatcher can bulk removed
            foreach (var c in items)
                Remove(c);
        }

        public bool Remove(IContent item)
        {
            if (_disposing)
                throw new ObjectDisposedException(nameof(WebContentDispatcher));
            //already removed?
            if (_graveyard.Contains(item)) return false;
            //do we have it in general, if we do let's store its last json
            item = this.GetContent(item.getContenTypeName(), item.guid);
            if (item != null)
            {
                //to and it into the _graveyard we have to make sure it was
                //in the shared container in the first place, otherwise it was added and removed
                //in the same action
                if (shared_container.Contains(item))
                    _graveyard.Add(item);

                _my_storage.Remove(item);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<LightContentReferece> Search(string search_text, int max_result)
        {
            IEnumerable<LightContentReferece> result =
                new HashSet<LightContentReferece>(
                    _my_storage.Search(search_text, max_result)
                    .Union(shared_container.Search(search_text, max_result))
                    .Except(_graveyard.Search(search_text, _graveyard.Count)));

            return result.Take(max_result);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {

            throw new NotImplementedException("don't");
        }


        #region Factory
        private static object _lock = new object();
        private static IContentContainer static_container;

        private class Factory : WebActionLinkedObjectFactory<IContentDispatcher>
        {
            protected override IContentDispatcher makeInstance()
            {
                return new WebContentDispatcher(static_container);
            }
        }

        private static Factory _factory = new Factory();


        [GlobalAvailableObjectConstructor()]
        public static IContentDispatcher getDispatcher()
        {
            if (WebActionExecutor.current == null)
            {
                throw new UnauthorizedAccessException("you can't call " + nameof(IContentDispatcher) + " outside a " + nameof(WebActionExecutor) + " using block");
            }
            //makeing sure the base is there
            if (static_container == null)
            {
                lock (_lock)
                {
                    if (static_container == null)
                    {
                        static_container = new ContentMemory();
                    }
                }
            }

            return _factory.getInstance();
        }


        #endregion

        public IEnumerable<IContent> GetAllContents()
        {
            throw new NotImplementedException("don't");
        }


    }
}
