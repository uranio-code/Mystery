using Mystery.Content;
using Mystery.Register;
using Mystery.UI;
using System.Linq;

namespace MysteryDMS.Model
{
    /// <summary>
    /// S-DMS-REQ-30
    /// The S-DMS shall have a folder called “Recycle Bin”.
    /// </summary>
    [ContentType(label = "DMS.RECYCLEBIN", list_label = "DMS.RECYCLEBIN")]
    [ContentView]
    [ContentTypeView]
    [GlobalAvalibleObjectImplementation(singleton = true)]
    public class DMSRecycleBin : DMSFolder
    {

        [GlobalAvailableObjectConstructor]
        static DMSRecycleBin getDMSRecycleBin() {
            var helper = new object();
            var cd = helper.getGlobalObject<IContentDispatcher>();
            var bin = cd.GetAll<DMSRecycleBin>().FirstOrDefault();
            if (bin == null) {
                var cc = helper.getGlobalObject<IGlobalContentCreator>();
                bin = cc.getNewContent<DMSRecycleBin>();
                cd.Add(bin);
            }
            return bin;
        }
    }
}
