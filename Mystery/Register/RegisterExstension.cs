using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mystery.Register
{

    public static class RegisterExstension
    {
        private static MysteryApp _mystery = new MysteryApp();

        private static Type _mystery_type = typeof(MysteryApp);


        public static MysteryApp getMystery(this object current)
        {
            //making sure it is instanced
            AssemblyRegister ar = _mystery.AssemblyRegister;
            return _mystery;
        }



        private static Dictionary<Type, object> _overrides = new Dictionary<Type, object>();

        public static T getGlobalObject<T>(this object current) 
        {
            Type type = typeof(T);
            if (object.ReferenceEquals(type, _mystery_type))
                throw new Exception("use this.getMystery");
            if (_overrides.ContainsKey(type))
                return (T)_overrides[type];
            if (!_mystery.AssemblyRegister.hasArgumentLessRetriver(type))
            {
                if (type.GetCustomAttributes(typeof(GlobalAvalibleObject), false).FirstOrDefault() != null
                    ||
                    type.GetCustomAttributes(typeof(GlobalAvalibleObjectImplementation), false).FirstOrDefault() != null)
                {
                    _mystery.AssemblyRegister.Register(type.Assembly);
                }
                else
                {
                    throw new Exception(type.FullName + " does seems to be a valid global type");
                }
            }
            Func<object> retriver = _mystery.AssemblyRegister.getArgumentLessRetriver(type);
            if (retriver != null)
            {
                return (T)retriver();
            }
            else
            {
                return default(T);
            }

        }


        public static void setGlobalObject<T>(this object current, T new_instance)
        {
            if (new_instance != null)
                _overrides[typeof(T)] = new_instance;
            else
                _overrides.Remove(typeof(T));
        }
        public static void resetGlobalObjects(this object current)
        {
            _overrides = new Dictionary<Type, object>();
        }


        public static MethodInfo getFirstMethodWithAttribute<T>(this Type current) where T : Attribute
        {
            MethodInfo mi = (from x in current.GetMethods()
                             where x.GetCustomAttributes(typeof(T), true).FirstOrDefault() != null
                             select x).FirstOrDefault();
            return mi;
        }

        public static T getFirstAttribute<T>(this Type current) where T : MysteryPropertyAttribute
        {
            return _mystery.AssemblyRegister.getMysteryPropertyAttributes<T>(current).FirstOrDefault();
        }


        public static T getMysteryAttribute<T>(this Type type) where T : MysteryClassAttribute
        {
            return _mystery.AssemblyRegister.getMysteryClassAttribute<T>(type);
        }


        public static IEnumerable<T> getMysteryPropertyAttributes<T>(this Type type, bool include_inherits = false) where T : MysteryPropertyAttribute
        {
            return _mystery.AssemblyRegister.getMysteryPropertyAttributes<T>(type, include_inherits);
        }
        public static T getMysteryAttribute<T>(this PropertyInfo property_info) where T : MysteryPropertyAttribute
        {
            return _mystery.AssemblyRegister.getMysteryPropertyAttributes<T>(property_info).FirstOrDefault();
        }

        public static IEnumerable<Type> getChilds(this Type type) {
            return _mystery.AssemblyRegister.getChildTypes(type);
        }


    }


}