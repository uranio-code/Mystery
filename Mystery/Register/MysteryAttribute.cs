using System;
using System.Reflection;

namespace Mystery.Register
{

    /// <summary>
    /// all mystery attributes inherits for this
    /// it contains a reference to the declaring type
    /// it is instanced only ones (per registrations)
    /// </summary>
    /// <remarks></remarks>
    public abstract class MysteryClassAttribute : Attribute
    {

        public Type used_in { get; set; }

        /// <summary>
        /// this method is called after instancing used in
        /// </summary>
        /// <remarks></remarks>
        public abstract void setUp();


    }

    /// <summary>
    /// attributes inhering for this one will be return for all class
    /// </summary>
    /// <remarks></remarks>
    public abstract class MysteryDefaultClassAttribute : MysteryClassAttribute
    {


    }


    public abstract class MysteryPropertyAttribute : Attribute
    {
        
        private PropertyInfo _used_in;
        public PropertyInfo used_in
        {
            get { return _used_in; }
            set
            {
                _used_in = value;
                if (value != null)
                {
                    _isReadOnly = value.GetSetMethod() == null;
                }
            }
        }
        public Func<object, object> retrive { get; set; }
        public Action<object, object> save { get; set; }
        private bool _isReadOnly;
        public bool isReadOnly
        {
            get { return _isReadOnly; }
        }

        public abstract void setup();

        public override int GetHashCode()
        {
            return used_in == null ? base.GetHashCode() : base.GetHashCode() + used_in.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            if (!(obj is MysteryPropertyAttribute))
                return false;
            var mpa = (MysteryPropertyAttribute)obj;

            return used_in == null ? true : used_in.Equals(mpa.used_in);
        }

    }


    public abstract class MysteryPropertyAttributeBaseHelper
    {
        public abstract object retrive(object o);
        public abstract void save(object o, object value);
    }


    public class MysteryPropertyAttributeHelper<objectType, propertyType> : MysteryPropertyAttributeBaseHelper
    {

        private Func<objectType, propertyType> typedRetrive { get; set; }
        private Action<objectType, propertyType> typedSave { get; set; }

        public MysteryPropertyAttributeHelper(PropertyInfo pi)
        {
            if (pi.GetGetMethod() != null)
            {
                typedRetrive = (Func<objectType, propertyType>)Delegate.CreateDelegate(typeof(Func<objectType, propertyType>), pi.GetGetMethod());
            }
            if (pi.GetSetMethod() != null)
            {
                typedSave = (Action<objectType, propertyType>)Delegate.CreateDelegate(typeof(Action<objectType, propertyType>), pi.GetSetMethod());
            }

        }

        public override object retrive(object o)
        {
            if (o == null)
                return null;
            return typedRetrive((objectType)o);
        }

        public override void save(object o, object value)
        {
            if (this.typedSave == null)
                return;
            if (o == null)
                return;
            this.typedSave((objectType)o, (propertyType)value);
        }

    }

}