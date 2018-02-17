using Mystery.Register;
using System;
using System.Collections.Generic;

namespace Mystery.Configuration
{
    [GlobalAvalibleObject()]
    public interface IConfigurationProvider
    {

        T getConfiguration<T>() where T : new();


        void setConfiguration<T>(T conf);

        void flush();
    }


    public class JustInstanceItConfigurationProvider : IConfigurationProvider
    {

        private Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        private static object _lock = new object();
        public T getConfiguration<T>() where T : new()
        {
            Type type = typeof(T);
            if (_instances.ContainsKey(type))
                return (T)_instances[type];
            lock (_lock)
            {
                if (_instances.ContainsKey(type))
                    return (T)_instances[type];
                T result = new T();
                _instances[type] = result;
                return result;
            }
        }

        public void setConfiguration<T>(T conf)
        {
            lock (_lock)
            {
                _instances[typeof(T)] = conf;
            }
        }

        public void flush()
        {
            lock (_lock) {
                _instances = new Dictionary<Type, object>();
            }
        }
    }
}

