using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Users;
using MysteryDMS.Model;

namespace MysteryDMS.Actions
{
    public class DMSDeleteDocument : BaseContentAction
    {
        public DMSDeleteDocument(IContent content, User user) : base(content, user)
        {
        }

        public override string label { get { return "DMS.VERSION.DELETEDOCUMENT"; } }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "file-o";
                result.font_awesome_icon_2 = "trash";
                result.style = "danger btn-outline";
                return result;
            }
        }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            ContentActionOutput result = new ContentActionOutput();

            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);
            DMSVersion pointer = version;


            DMSRecycleBin bin = cd.getGlobalObject<DMSRecycleBin>();

            while (pointer.previous_version.value != null)
            {
                pointer = pointer.previous_version.value;
                pointer.parent_folders.Clear();
                pointer.parent_folders.Add(bin);
                cd.Add(pointer);
            }

            pointer = version;
            while (pointer.next_version.value != null)
            {
                pointer = pointer.next_version.value;
                pointer.parent_folders.Clear();
                pointer.parent_folders.Add(bin);
                cd.Add(pointer);
            }

            version.parent_folders.Clear();
            version.parent_folders.Add(bin);
            cd.Add(version);
            result.message = "Document moved to the bin";
            
            return result;
        }

        /// <summary>
        /// only the author can add a new version.
        /// S-DMS-REQ-35
        /// The S-DMS shall allow Users to create, edit and delete any content inside the “Training” folder.
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            DMSVersion version = cd.GetContent<DMSVersion>(this.input.guid);
            if (version == null) return false;

            foreach(DMSFolder folder in version.parent_folders)
            {
                if (folder.GetType() == typeof(DMSTrainingFolder)) { return true; }
            }

            User author = version.author;
            if (author == null) return false;

            return author.Equals(this.user);

        }
    }
    }
