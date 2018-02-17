using System;

namespace Mystery.Register
{

    /// <summary>
    /// attribute to make the class or interface global available
    /// </summary>
    /// <remarks></remarks>
    public class GlobalAvalibleObject : MysteryClassAttribute
    {



        public override void setUp()
        {
        }
    }

    /// <summary>
    /// specificity an implementation of GlobalAvalibleObject
    /// </summary>
    /// <remarks></remarks>
    public class GlobalAvalibleObjectImplementation : MysteryClassAttribute
    {


        public bool overrides_exsisting { get; set; }

        public bool singleton { get; set; }
        public Type implementation_of { get; set; }


        public override void setUp()
        {
        }
    }

    /// <summary>
    /// a shared method in the class to call to instance the object
    /// </summary>
    /// <remarks></remarks>
    public class GlobalAvailableObjectConstructor : Attribute
    {

    }


    public class GlobalAvalibleObjectActivationException : Exception
    {

        public GlobalAvalibleObjectActivationException(string message, Exception inner) : base(message, inner)
        {
        }

    }


}