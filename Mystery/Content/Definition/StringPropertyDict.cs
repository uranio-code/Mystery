
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Mystery.Content
{

    public interface IStringPropertyDict
    {
        IContent content { get; set; }
        IEnumerable<string> getValues();
        string getLabelFor(string value);
        string default_value { get; }
    }

    /// <summary>
    /// provide the template by decorating a dictionary
    /// </summary>
    /// <remarks></remarks>
    public class BaseStringSelection : IStringPropertyDict
    {


        protected Dictionary<string, string> _dictonary = new Dictionary<string, string>();
        public BaseStringSelection()
        {
            setUpDictornary();
        }

        public string getLabelFor(string value)
        {
            if (value == null)
                return string.Empty;
            return _dictonary.ContainsKey(value) ? _dictonary[value] : value;
        }

        //override to provide the right order
        public virtual System.Collections.Generic.IEnumerable<string> getValues()
        {

            return _dictonary.Keys;
        }

        /// <summary>
        /// override and setup Dictionary
        /// </summary>
        /// <remarks></remarks>

        protected virtual void setUpDictornary()
        {
        }

        public IContent content { get; set; }

        public virtual string default_value
        {
            get
            {
                string result = getValues().FirstOrDefault();
                return result == null ? string.Empty : result;
            }
        }
    }


    public class StringPropertyDictAttr : MysteryPropertyAttribute
    {

        public Type implementing_type { get; set; }


        private IActivator _activator;

        public override void setup()
        {
            if (implementing_type != null)
            {
                implementing_type.GetInterfaceMap(typeof(IStringPropertyDict));
                //crash if not right
            }
            else {
                implementing_type = typeof(BaseStringSelection);
            }

            _activator = this.getGlobalObject<FastActivator>().getActivator(implementing_type);
        }

        public IStringPropertyDict getDictionary(IContent content)
        {
            IStringPropertyDict result = (IStringPropertyDict)_activator.createInstance();
            result.content = content;
            return result;
        }


    } 
}
