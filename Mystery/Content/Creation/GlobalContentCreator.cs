using Mystery.Register;
using System;
using System.Collections.Generic;
namespace Mystery.Content
{

    [GlobalAvalibleObjectImplementation(implementation_of = typeof(IGlobalContentCreator), overrides_exsisting = false, singleton = true)]
    public class GlobalContentCreator : IGlobalContentCreator
    {


        private Dictionary<Type, IContentCreator> _creators = new Dictionary<Type, IContentCreator>();
        public T getNewContent<T>() where T : IContent
        {
            return (T)this.getNewContent(typeof(T));
        }

        public IContent getNewContent(Type type)
        {
            if (!_creators.ContainsKey(type))
            {
                _creators[type] = type.getMysteryAttribute<ContentCreatorAttribute>().getCreator();
            }
            return _creators[type].create();
        }

        public T getAndAddNewContent<T>() where T : IContent
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            var result = this.getNewContent<T>();
            cd.Add(result);
            return result;
        }

        public IContent getAndAddNewContent(Type type)
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            var result = this.getNewContent(type);
            cd.Add(result);
            return result;
        }
    }

}