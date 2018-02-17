using Mystery.Content;
using Mystery.Files;
using Mystery.Register;
using Mystery.UI;
using Mystery.Users;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Instance
{
    /// <summary>
    /// singleton content type representing the current instance
    /// </summary>
    [ContentType(label = "Instance", list_label = "Instances")]
    [GlobalAvalibleObjectImplementation()]
    public class MysteryInstance:BaseContent
    {

        static private Guid instance_guid { get; set; } = Guid.Empty;
        static private object _lock = new object();

        [GlobalAvailableObjectConstructor]
        public static MysteryInstance getMysteryInstance()
        {
            var helper = new object();
            var cd = helper.getGlobalObject<IContentDispatcher>();
            if (instance_guid != Guid.Empty) {
                return cd.GetContent<MysteryInstance>(instance_guid);
            }
            var instance = cd.GetAll<MysteryInstance>().FirstOrDefault();
            if (instance == null)
            {
                //we shall avoid double creations
                lock (_lock) {
                    instance = cd.GetAll<MysteryInstance>().FirstOrDefault();
                    if (instance == null) {
                        var cc = helper.getGlobalObject<IGlobalContentCreator>();
                        instance = cc.getNewContent<MysteryInstance>();
                        cd.Add(instance);
                    }
                }
            }
            //if we the owner property is empty it means then this instance
            //is yet to be configured, let's assume the current user is doing just that
            if (instance.owner.isNullOrEmpty()) {
                var session = helper.getGlobalObject<MysterySession>();
                if (session.authenticated_user != null)
                    instance.owner = session.authenticated_user;
            }
            instance_guid = instance.guid;
            return instance;
        }


        [ContentProperty, PropertyView]
        public string name { get; set; }

        [ContentProperty, PropertyView]
        public MysteryFile logo { get; set; }


        [ContentProperty]
        public MultiContentReference<User> admistrators { get; set; }

        [ContentProperty]
        public ContentReference<User> owner { get; set; }

    }
}
