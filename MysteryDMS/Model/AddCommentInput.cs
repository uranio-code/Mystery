using Mystery.Content;
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryDMS.Model
{
    public class AddCommentInput
    {
        public Guid dms_version_guid { get; set; }
        public DMSVersion content {
            get
            {
                if (dms_version_guid == Guid.Empty) return null;
                return this.getGlobalObject<IContentDispatcher>().GetContent<DMSVersion>(dms_version_guid);
            }
            set
            {
                if (value == null)
                    dms_version_guid = Guid.Empty;
                else
                    dms_version_guid = value.guid;
            }
        }
        public string comment_text { get; set; }
    }
}
