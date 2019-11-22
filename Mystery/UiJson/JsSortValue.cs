using Mystery.Content;
using Mystery.Files;
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.UiJson
{
    /// <summary>
    /// provide a value which can be used in js to sort.
    /// </summary>
    [GlobalAvalibleObjectImplementation(singleton = true)]
    public class JsSortValue
    {
        
        static public object getSortValue(object value)
        {
            if (value == null) return 0;
            //basic types
            if (value is string ||
                value is double ||
                value is bool ||
                value is long)
                return value;
            if (value is DateTime)
                return ((DateTime)value).Ticks;

            Type type = value.GetType();
            //enums
            if (type.IsEnum)
                return (int)value;

            //embedded
            if (typeof(IContent).IsAssignableFrom(type))
                return ((IContent)value).ReferenceText;
            
            //reference
            if (typeof(IContentReference).IsAssignableFrom(type))
            {
                var content_reference = (IContentReference)value;
                if (content_reference.isNullOrEmpty())
                    return string.Empty;
                var c = content_reference.getContent();
                if(c == null)
                    return string.Empty;
                return c.ReferenceText;
            }
            
            //files
            if (typeof(MysteryFile).IsAssignableFrom(type))
                return ((MysteryFile)value).filename;

            //multi reference, as we ruled out the single one already
            if (typeof(IReferenceProperty).IsAssignableFrom(type))
            {
                var contents = new List<string>(from x in  ((IReferenceProperty)value).getAsContentEnum()
                                                select x.ReferenceText);
                if (contents.Count == 0) return string.Empty;
                contents.Sort();
                //we are going to sort by number and then by reference text
                string result = contents.Count.ToString().PadLeft(5, '0') + " ";
                result += string.Join(" ", contents.ToArray());
                return result;
            }
            //list of contents 
            if (typeof(IEnumerable<IContent>).IsAssignableFrom(type))
            {
                var contents = new List<string>(from x in ((IEnumerable<IContent>)value)
                                                select x.ReferenceText);
                if (contents.Count == 0) return string.Empty;
                contents.Sort();
                //we are going to sort by number and then by reference text
                string result = contents.Count.ToString().PadLeft(5, '0') + " ";
                result += string.Join(" ", contents.ToArray());
                return result;
            }

            //all right, we don't care?
            return 0;

        }
    }
}
