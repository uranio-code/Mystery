using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Mystery.Register
{


    public class AssemblyRegister
    {


        #region "GlobalAvalibleObject"

        private Dictionary<Type, Func<object>> _argumentless_retrivers = new Dictionary<Type, Func<object>>();
        public Func<object> getArgumentLessRetriver(Type type)
        {
            return _argumentless_retrivers[type];
        }

        public bool hasArgumentLessRetriver(Type type)
        {
            return _argumentless_retrivers.ContainsKey(type);
        }


        private void registerGlobalAvalibleObject(Type type)
        {
            foreach (GlobalAvalibleObjectImplementation implementation_attr in type.GetCustomAttributes(typeof(GlobalAvalibleObjectImplementation), false))
            {
                //checking if they are registering themselves 
                Type implementing = implementation_attr.implementation_of == null ? type : implementation_attr.implementation_of;
                //making sure then what they are implementing is already register
                if (!object.ReferenceEquals(implementing, type) && !_argumentless_retrivers.ContainsKey(implementing))
                {
                    registerGlobalAvalibleObject(implementing);
                    if (!_argumentless_retrivers.ContainsKey(implementing))
                        throw new Exception("the type " + type.FullName + " what to implement " + implementing.FullName + " which is not a " + typeof(GlobalAvalibleObject).Name);
                }

                Func<object> retriver = _argumentless_retrivers.ContainsKey(implementing) ? _argumentless_retrivers[implementing] : null;

                if (retriver == null || implementation_attr.overrides_exsisting)
                {
                    MethodInfo mi = type.getFirstMethodWithAttribute<GlobalAvailableObjectConstructor>();

                    if (mi == null && !implementation_attr.singleton)
                    {
                        throw new Exception(type.FullName + " is not a " + typeof(GlobalAvalibleObject).Name + " singleton object but I could not find a method marked with " + typeof(GlobalAvailableObjectConstructor).Name);
                    }

                    if (mi != null && !mi.IsStatic)
                    {
                        throw new Exception(mi.Name + " of " + type.FullName + " is marked with " + typeof(GlobalAvalibleObject).Name + " but is not static method");
                    }

                    Func<object> create_instance = default(Func<object>);

                    if (mi != null)
                    {
                        create_instance = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>), mi);
                    }
                    else
                    {
                        create_instance = () =>
                        {
                            try
                            {
                                return Activator.CreateInstance(type);
                            }
                            catch (Exception ex)
                            {
                                if (ex.InnerException != null && (ex.InnerException) is GlobalAvalibleObjectActivationException)
                                {
                                    throw ex.InnerException;
                                }
                            //the normal exception is not than useful
                            throw new GlobalAvalibleObjectActivationException("fail to instance " + type.FullName, ex.InnerException);
                            }

                        };
                    }

                    if (implementation_attr.singleton)
                    {
                        //let's not instance it now but as soon as it is called and only ones
                        retriver = () =>
                        {
                            object instance = create_instance();
                            Func<object> return_instance = () => { return instance; };
                            _argumentless_retrivers[implementing] = return_instance;
                            return instance;
                        };
                    }
                    else
                    {
                        retriver = create_instance;
                    }
                    _argumentless_retrivers[implementing] = retriver;
                }

            }
            GlobalAvalibleObject global_attr = (GlobalAvalibleObject)type.GetCustomAttributes(typeof(GlobalAvalibleObject), false).FirstOrDefault();
            if (global_attr == null)
                return;
            if (!_argumentless_retrivers.ContainsKey(type))
            {
                _argumentless_retrivers[type] = null;
            }

        }
        #endregion

        private IList<Assembly> _registered_assemblies = new List<Assembly>();
        public IEnumerable<Assembly> getAssemblyRegistered()
        {
            return new List<Assembly>(_registered_assemblies);
        }

        public void Register(Assembly ass)
        {
            _registered_assemblies.Add(ass);

            //first global objects
            foreach (Type type in ass.GetTypes())
            {
                registerGlobalAvalibleObject(type);
            }
            //all the rest
            HashSet<Type> actors = new HashSet<Type>();
            foreach (Type type in ass.GetTypes())
            {
                registerType(type);
                if (type.GetInterface(typeof(IRegistrationActor).FullName) != null)
                    actors.Add(type);
            }



            foreach (Type type in actors)
            {
                IRegistrationActor instance = (IRegistrationActor)Activator.CreateInstance(type);
                instance.act();
            }

        }

        public AssemblyRegister()
        {
            this.Register(this.GetType().Assembly);
        }

        private Dictionary<Type, HashSet<Type>> _attributes = new Dictionary<Type, HashSet<Type>>();
        private Dictionary<Type, HashSet<Type>> _childs = new Dictionary<Type, HashSet<Type>>();
        private Dictionary<Type, HashSet<MysteryClassAttribute>> _mystery_attributes = new Dictionary<Type, HashSet<MysteryClassAttribute>>();

        private Dictionary<Type, Dictionary<Type, MysteryClassAttribute>> _mystery_attributes_dict = new Dictionary<Type, Dictionary<Type, MysteryClassAttribute>>();
        private Dictionary<Type, HashSet<MysteryPropertyAttribute>> _mystery_property_attributes = new Dictionary<Type, HashSet<MysteryPropertyAttribute>>();

        private Dictionary<PropertyInfo, HashSet<MysteryPropertyAttribute>> _mystery_property_attributes_map = new Dictionary<PropertyInfo, HashSet<MysteryPropertyAttribute>>();
        public void addAttributeToClass<AttrType, ClassType>(AttrType attr = null) where AttrType : MysteryClassAttribute, new()
        {
            if (attr == null)
                attr = new AttrType();
            if (!_attributes.ContainsKey(typeof(AttrType)))
                _attributes[typeof(AttrType)] = new HashSet<Type>();
            _attributes[typeof(AttrType)].Add(typeof(ClassType));
            attr.used_in = typeof(ClassType);
            attr.setUp();
            _mystery_attributes[typeof(ClassType)].Add(attr);
            _mystery_attributes_dict[typeof(ClassType)][attr.GetType()] = attr;
        }

        public void addAttributeToProperty<AttrType>(PropertyInfo pi, AttrType attr = null) where AttrType : MysteryPropertyAttribute, new()
        {
            if (attr == null)
                attr = new AttrType();
            Type generic_helper_type = typeof(MysteryPropertyAttributeHelper<,>);

            if (!_mystery_property_attributes_map.ContainsKey(pi))
                _mystery_property_attributes_map[pi] = new HashSet<MysteryPropertyAttribute>();
            attr.used_in = pi;
            Type this_property = generic_helper_type.MakeGenericType(pi.DeclaringType, pi.PropertyType);
            MysteryPropertyAttributeBaseHelper helper = (MysteryPropertyAttributeBaseHelper)Activator.CreateInstance(this_property, pi);
            attr.save = helper.save;
            attr.retrive = helper.retrive;
            attr.setup();
            if (!_mystery_property_attributes.ContainsKey(pi.DeclaringType))
                _mystery_property_attributes[pi.DeclaringType] = new HashSet<MysteryPropertyAttribute>();
            //override declarations
            HashSet<MysteryPropertyAttribute> to_remove = new HashSet<MysteryPropertyAttribute>(
                from x in _mystery_property_attributes_map[pi] where (x) is AttrType select x
                );
            foreach (AttrType att_to_remove in to_remove)
            {
                _mystery_property_attributes[pi.DeclaringType].Remove(att_to_remove);
                _mystery_property_attributes_map[pi].Remove(attr);
            }
            _mystery_property_attributes[pi.DeclaringType].Add(attr);
            _mystery_property_attributes_map[pi].Add(attr);

        }



        private HashSet<Type> _registered_types = new HashSet<Type>();
        private IDictionary<string,Type> _types_by_name = new Dictionary<string, Type>();
        public void registerType(Type type)
        {
            if (_registered_types.Contains(type))
                return;
            _registered_types.Add(type);

            _types_by_name[type.FullName] = type;

            object[] CustomAttributes = type.GetCustomAttributes(false);
            _mystery_attributes[type] = new HashSet<MysteryClassAttribute>();
            _mystery_attributes_dict[type] = new Dictionary<Type, MysteryClassAttribute>();

            _mystery_property_attributes[type] = new HashSet<MysteryPropertyAttribute>();

            //registering the type attributes
            foreach (Attribute cs in CustomAttributes)
            {
                Type at = cs.GetType();
                if (!_attributes.ContainsKey(at))
                    _attributes[at] = new HashSet<Type>();
                _attributes[at].Add(type);
                if ((cs) is MysteryClassAttribute)
                {
                    MysteryClassAttribute ma = (MysteryClassAttribute)cs;
                    ma.used_in = type;
                    ma.setUp();
                    _mystery_attributes[type].Add(ma);
                    _mystery_attributes_dict[type][ma.GetType()] = ma;
                }
            }

            Type generic_helper_type = typeof(MysteryPropertyAttributeHelper<,>);

            foreach (PropertyInfo pi in type.GetProperties())
            {
                _mystery_property_attributes_map[pi] = new HashSet<MysteryPropertyAttribute>();
                foreach (Attribute attr in pi.GetCustomAttributes(false))
                {
                    if (!(attr is MysteryPropertyAttribute))
                        continue;
                    MysteryPropertyAttribute mi_attr = (MysteryPropertyAttribute)attr;
                    mi_attr.used_in = pi;
                    Type this_property = generic_helper_type.MakeGenericType(pi.DeclaringType, pi.PropertyType);
                    MysteryPropertyAttributeBaseHelper helper = (MysteryPropertyAttributeBaseHelper)Activator.CreateInstance(this_property, pi);
                    mi_attr.save = helper.save;
                    mi_attr.retrive = helper.retrive;
                    mi_attr.setup();
                    _mystery_property_attributes[type].Add(mi_attr);
                    _mystery_property_attributes_map[pi].Add(mi_attr);
                }
            }


            //inheritance recording
            Type parent = type.BaseType;
            while (parent != null)
            {
                if (!_childs.ContainsKey(parent))
                    _childs[parent] = new HashSet<Type>();
                _childs[parent].Add(type);
                parent = parent.BaseType;
            }

            foreach (Type implemented_interface in type.GetInterfaces())
            {
                if (!_childs.ContainsKey(implemented_interface))
                    _childs[implemented_interface] = new HashSet<Type>();
                _childs[implemented_interface].Add(type);
            }

        }

        #region "gets"

        public Type getTypeByFullName(string fullname) {
            if (_types_by_name.ContainsKey(fullname))
                return _types_by_name[fullname];
            return null;
        }

        private IEnumerable<Type> getTypesMarkedWith(Type attribute_type)
        {
            if (!_attributes.ContainsKey(attribute_type))
                return new List<Type>();
            return new HashSet<Type>(_attributes[attribute_type]);
        }

        public IEnumerable<Type> getTypesMarkedWith<T>() where T : Attribute
        {
            return getTypesMarkedWith(typeof(T));
        }

        /// <summary>
        /// return all registered types inheriting from T or implementing T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<Type> getChildTypes<T>()
        {
            return getChildTypes(typeof(T));
        }
        /// <summary>
        /// return all registered types inheriting from type or implementing type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Type> getChildTypes(Type type)
        {
            if (!_childs.ContainsKey(type))
                return new List<Type>();
            return new HashSet<Type>(_childs[type]);
        }

        public IEnumerable<MysteryClassAttribute> getMysteryClassAttribute(Type type)
        {
            if (!_mystery_attributes.ContainsKey(type))
                return new List<MysteryClassAttribute>();
            return new HashSet<MysteryClassAttribute>(_mystery_attributes[type]);
        }

        public IEnumerable<MysteryClassAttribute> getMysteryClassAttribute<T>()
        {
            Type type = typeof(T);
            return getMysteryClassAttribute(type);
        }

        public IEnumerable<MysteryPropertyAttribute> getMysteryPropertyAttributes(Type type)
        {
            if (!_mystery_property_attributes.ContainsKey(type))
                return new List<MysteryPropertyAttribute>();
            return new HashSet<MysteryPropertyAttribute>(_mystery_property_attributes[type]);
        }

        public IEnumerable<MysteryPropertyAttribute> getMysteryPropertyAttributes<T>()
        {
            Type type = typeof(T);
            return getMysteryPropertyAttributes(type);
        }

        public IEnumerable<TypeOfAttribute> getMysteryPropertyAttributes<TypeOfAttribute>(Type type, bool include_inherits = false) where TypeOfAttribute : MysteryPropertyAttribute
        {
            if (type == null)
                return new HashSet<TypeOfAttribute>();
            HashSet<TypeOfAttribute> result = new HashSet<TypeOfAttribute>(
                from x in getMysteryPropertyAttributes(type) where x is TypeOfAttribute select (TypeOfAttribute)x
                );

            if (include_inherits || (result.Count == 0 && !object.ReferenceEquals(type, typeof(object))))
            {
                foreach (TypeOfAttribute a in getMysteryPropertyAttributes<TypeOfAttribute>(type.BaseType))
                    result.Add(a);
            }
            return result;
        }

        public IEnumerable<TypeOfAttribute> getMysteryPropertyAttributes<TypeOfAttribute>(PropertyInfo property_info) where TypeOfAttribute : MysteryPropertyAttribute
        {
            HashSet<TypeOfAttribute> result = new HashSet<TypeOfAttribute>((from x in _mystery_property_attributes_map[property_info] where x is TypeOfAttribute select (TypeOfAttribute)x));
            return result;
        }

        public IEnumerable<TypeOfAttribute> getMysteryPropertyAttributes<TypeOfAttribute, InType>() where TypeOfAttribute : MysteryPropertyAttribute
        {
            Type type = typeof(InType);
            return getMysteryPropertyAttributes<TypeOfAttribute>(type);
        }

        private static object _lock = new object();
        private MysteryClassAttribute getMysteryDefaultClassAttribute(Type type, Type attr_type)
        {
            MysteryClassAttribute ma = (MysteryClassAttribute)this.getGlobalObject<FastActivator>().createInstance(attr_type);
            ma.used_in = type;
            ma.setUp();
            _mystery_attributes[type].Add(ma);
            _mystery_attributes_dict[type][ma.GetType()] = ma;
            return ma;
        }


        private HashSet<Type> _not_default = new HashSet<Type>();

        private static Type default_attr_type = typeof(MysteryDefaultClassAttribute);
        public T getMysteryClassAttribute<T>(Type type) where T : MysteryClassAttribute
        {
            if (!_mystery_attributes_dict.ContainsKey(type))
                return null;
            Type att_type = typeof(T);
            if (_mystery_attributes_dict[type].ContainsKey(att_type))
                return (T)_mystery_attributes_dict[type][att_type];
            if (_not_default.Contains(att_type))
                return null;

            if (default_attr_type.IsAssignableFrom(att_type))
            {

                //when this function get called the attribute is given at the run time
                //we shall avoid crashes due multi threading 
                lock (_lock) {
                    return (T)getMysteryDefaultClassAttribute(type, att_type);
                }
                
            }
            else
            {
                _not_default.Add(att_type);
                return null;
            }

        }

        public T getMysteryClassAttribute<T, InType>() where T : MysteryClassAttribute
        {
            return getMysteryClassAttribute<T>(typeof(InType));
        }
        #endregion

    }

}