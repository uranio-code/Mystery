using Mystery.Content;
using Mystery.Register;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace MysteryDMS.Model.Providers
{
    

    public class DMSUserGroupValuesProvider<DMSUSerGroup> : IMultiReferencePropertyValuesProvider
    {
        public PropertyInfo property { get; set; }



        public IEnumerable<LightContentReferece> getSuggestions(IContent item, string search_text)
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            var all = cd.GetLightContentRereferece<DMSUserGroup>();

            // remove the item itself DMS-89
            all = (from x in all where x.guid != item.guid select x);

            // here I have to remove the reference to the parent contents. 
            // 1) I need to get their guids from the item
            // 2) I need to remove the LightReferences from all where uid is in the one I found at step 1
            LinkedList<LightContentReferece> groups_to_exclude_reference = new LinkedList<LightContentReferece>();
            IEnumerable<DMSUserGroup> xx = ((DMSUserGroup)item).parent_groups();
            if (xx!=null)
            {
                foreach (DMSUserGroup group in xx)
                    groups_to_exclude_reference.AddFirst(new LightContentReferece(group));

                all = all.Except<LightContentReferece>(groups_to_exclude_reference);
            }
            
            
            if (string.IsNullOrEmpty(search_text)) return all;
            return (from x in all where x.ReferenceText.Contains(search_text) select x);
        }


        public bool validate(IContent item, IEnumerable<IContent> values)
        {
            if (values == null) return true;
            foreach (IContent v in values)
            {
                //we do not want any values to be null in multi references
                if (v == null) return false;
                if (!typeof(DMSUserGroup).IsAssignableFrom(v.GetType())) return false;
            }
            return true;
        }

    }
}