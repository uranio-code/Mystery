using System;
using System.Collections.Generic;

namespace Mystery.Register
{

    public interface IActivator
    {
        object createInstance();
    }

    [GlobalAvalibleObjectImplementation(singleton = true)]
    public class FastActivator
    {


        private Dictionary<Type, IActivator> _types = new Dictionary<Type, IActivator>();
        private class privateFastActivator<T> : IActivator where T : new()
        {

            public object createInstance()
            {
                return new T();
            }
        }


        private Type generic = typeof(privateFastActivator<>);
        public object createInstance(Type type)
        {
            if (_types.ContainsKey(type))
                return _types[type].createInstance();
            Type this_type = generic.MakeGenericType(type);
            IActivator fast_activator = (IActivator)Activator.CreateInstance(this_type);
            _types[type] = fast_activator;
            return fast_activator.createInstance();

        }

        public IActivator getActivator(Type type)
        {
            if (_types.ContainsKey(type))
                return _types[type];
            Type this_type = generic.MakeGenericType(type);
            IActivator fast_activator = (IActivator)createInstance(this_type);
            _types[type] = fast_activator;
            return fast_activator;
        }

        public T createInstace<T>() {
            return (T)this.createInstance(typeof(T));
        }
        

    }


}
