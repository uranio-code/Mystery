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
    [ContentType(label = "DMS.TRAININGFOLDER", list_label = "DMS.TRAININGFOLDER")]
    [ContentView]
    [ContentTypeView]
    [GlobalAvalibleObjectImplementation(singleton = true)]
    class DMSTrainingFolder : DMSFolder
    {

        [GlobalAvailableObjectConstructor]
        public static DMSTrainingFolder DMSTrainingFolderContructor()  
        {
            IContentDispatcher cd = (new object()).getGlobalObject<IContentDispatcher>();
            DMSTrainingFolder ret = cd.GetAll<DMSTrainingFolder>().FirstOrDefault();

            if (ret == null)
            {
                throw new System.Exception("DMS not installed");
            }

            return ret;
        }
    }
}
