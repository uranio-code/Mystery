using System;
using Mystery.Content;
using Mystery.Users;
using Mystery.UI;
using System.Collections.Generic;
using Mystery.Register;
using System.Linq;
using Mystery.Files;
using Mystery.Messaging;
using Newtonsoft.Json;
using MysteryDMS.Actions;

namespace MysteryDMS.Model
{

    /// <summary>
    /// Document_type and version. The version defines the properties available, the document_type let the user set which properties can be used and 
    /// which must be required. My idea is to have a DMSProperty content.A multi reference to the DMSProperty content in the Document_Type.
    /// A third object which creates/updates the DMSProperties instances reading the Version class (let say the installer).
    /// </summary>
    [ContentType(label = "DMS.VERSION", list_label = "DMS.VERSIONS")]
    [ContentTypeButton(template_url = "MysteryWebContent/MysteryContent/TypeButton.html")]
    [ContentTypeView]
    [ContentAction(implementing_type = typeof(DMSVersionActionProvider))]
    [ContentWorkflowAction(implementing_type = typeof(DMSVersionWorkflowActionProvider))]
    [ContentListAction(implementing_type = typeof(DMSVersionActionListProvider))]
    [ContentView(controller = "ContentViewController", templateUrl = "DMS/dms_version_view.html")]
    public class DMSVersion : BaseContent, IDMSContent
    {

        [ContentProperty(label = "DMS.VERSION.TITLE"), ReferenceText()]
        [PropertyColumn(template_url = "MysteryWebContent/MysteryContent/Properties/StringPropertyLinkedCell.html")]
        [PropertyView(help_html = "<b style=\"color: red\">I can</b> have <div class=\"label label-success\">HTML</div> content")]
        [PropertyEdit]
        public string  title { get; set; } 

        [ContentProperty(label = "DMS.VERSION.DESCRIPTION")]
        [PropertyView(template_url = "MysteryWebContent/MysteryContent/Properties/TextPropertyNonModal.html")]
        [PropertyEdit]
        public string  description { get; set; } 

        [ContentProperty(label = "DMS.VERSION.CHANGEDESCRIPTION")]
        [PropertyView]
        [PropertyEdit]
        public string  Version_change_description { get; set; } 

        [ContentProperty(label = "DMS.VERSION.ORIGINAL_FILE")]
        [PropertyView]
        [PropertyEdit]
        public MysteryFile original_file { get; set; }

        [ContentProperty(label = "DMS.VERSION.ATTACHMENTS")]
        [PropertyView]
        [PropertyEdit]
        public MultiContentReference<DMSAttachment> attachments { get; set; } = new MultiContentReference<DMSAttachment>();

        [ContentProperty(label = "DMS.VERSION.VERSION_NUMBER")]
        [PropertyView]
        [PropertyColumn]
        public long version_number { get; set; } = 1;

        [ContentProperty(label = "DMS.VERSION.AUTHOR")]
        [PropertyView]
        [PropertyEdit]
        [PropertyColumn]
        [SingleReferencePropertyValuesProviderAtt]
        public ContentReference<User> author { get; set; } = new ContentReference<User>();

        [ContentProperty(label = "DMS.VERSION.CO_AUTHORS")]
        [PropertyEdit]
        [PropertyView]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<User> co_authors { get; set; } = new MultiContentReference<User>();

        [ContentProperty(label = "DMS.VERSION.REVIEWERS")]
        [PropertyEdit]
        [PropertyView]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<User> reviewers { get; set; } = new MultiContentReference<User>();

        [ContentProperty(label = "DMS.VERSION.SIGNATORIES")]
        [PropertyEdit]
        [PropertyView]
        public string  signatories { get; set; }

        [ContentProperty(label = "DMS.VERSION.WORKFLOW_TYPE_PROPERTY")]
        [PropertyEdit]
        [PropertyView]
        [SingleReferencePropertyValuesProviderAtt]
        public ContentReference<DMSWorkflowType> workflow_type { get; set; } = new ContentReference<DMSWorkflowType>();

        private MultiContentReference<User> _approvers;
        [ContentProperty(label = "DMS.VERSION.APPROVERS")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<User> approvers
        {
            get
            {
                return _approvers;
            }
            set
            {
                _approvers = value;
                if (!active) return;
                if (wf_Status != DMSConstants.wf_signed) return;
                next_actors = _approvers;
            }
        } 

        [ContentProperty(label = "DMS.VERSION.EXTERNAL_REFERENCE_ID")]
        [PropertyView]
        [PropertyEdit]
        public string  external_reference_id { get; set; } 

        [ContentProperty(label = "DMS.VERSION.VISIBLE")]
        [PropertyView]
        [PropertyEdit]
        public bool  visible { get; set; } 

        
        [ContentProperty(label = "DMS.VERSION.TAGS")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<DMSTag> tags { get; set; } 

        
        /// <summary>
        /// # S-DMS-REQ-46        The S-DMS shall assign one of the following Version Status to each document version:
        /// * Draft
        /// * Current
        /// * Obsolete
        /// </summary>
        [ContentProperty(label = "DMS.VERSION.STATUS")]
        public string  status { get; set; }

        private Boolean _active;
        [ContentProperty(label = "DMS.VERSION.ACTIVE")]
        [PropertyView]
        public bool active {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                // logic for next_actors described above the property.
                if(_active)
                {
                    _next_actors = approvers;
                }
                else
                {
                    _next_actors = new MultiContentReference<User>();
                }
            }
        }

        private string _wf_status;
        /// <summary>
        /// * In Work
        /// * Under review
        /// * Disapproved
        /// * Disapproved with comments
        /// * Approved
        /// * Approved with comments
        /// * obsolete
        /// </summary>
        [ContentProperty(label = "DMS.VERSION.WF_STATUS")]
        [PropertyView]
        [PropertyEdit(implementing_type = typeof(ReadOnlyProperty))]
        [PropertyColumn]
        public string  wf_Status
        {
            get
            {
                return _wf_status;
            }
            set
            {
                if (_wf_status == value) return;
                _wf_status = value;
                if (Initializing) return;

                INotifier notifier = this.getGlobalObject<INotifier>();
                
                IMessageManager mm = this.getGlobalObject<IMessageManager>();
                ICodifiedMessage cmessage = mm.getCodifiedMessage(DMSEnumerations.wf_change_message_code, DMSEnumerations.language);
                IUserMessage message = cmessage.message;
                if (message != null)
                {
                    message.body = message.body.Replace("#wf_status", _wf_status);
                    message.body = message.body.Replace("#guid", guid.Tiny());
                    message.to = getSubscribers();
                    notifier.sendMessage(message);

                }


            }
        } 
        
        [JsonIgnore]
        public IEnumerable<DMSComment> comments { get {
                var cd = this.getGlobalObject<IContentDispatcher>();
                return cd.GetAllByFilter<DMSComment>(x => x.parent_dms_content.guid == this.guid );
            } } 

        [ContentProperty(label = "DMS.VERSION.ALLOW_COMMENTS")]
        [PropertyView]
        [PropertyEdit]
        public bool  allow_comments { get; set; } 

        [ContentProperty(label = "DMS.VERSION.VISIBLE_COMMENTS")]
        [PropertyView]
        [PropertyEdit]
        public bool  comments_visible { get; set; } 


        [ContentProperty(label = "DMS.VERSION.NEXT_VERSION")]
        [PropertyView]
        [PropertyEdit(implementing_type = typeof(ReadOnlyProperty))] //managed with actions
        public ContentReference<DMSVersion> next_version { get; set; } = new ContentReference<DMSVersion>();

        [ContentProperty(label = "DMS.VERSION.PREVIOUS_VERSION")]
        [PropertyView]
        [PropertyEdit(implementing_type = typeof(ReadOnlyProperty))] //managed with actions
        public ContentReference<DMSVersion> previous_version { get; set; } = new ContentReference<DMSVersion>();

        public ContentReference<DMSVersion> _superseeded_by = new ContentReference<DMSVersion>();

        [ContentProperty(label = "DMS.VERSION.SUPERSEEDED_BY")]
        public ContentReference<DMSVersion> superseeded_by
        {
            get
            {
                return _superseeded_by;
            }
            set
            {
                if (_superseeded_by.isSameAs(value))
                    return;
                _superseeded_by = value;
                if (Initializing)//we are loading this from json
                    return;

                IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
                
                DMSVersion tmp = this;
                while(tmp.previous_version.value != null)
                {
                    tmp = tmp.previous_version.value;                    
                }

                do
                {
                    tmp._superseeded_by = value; // to avoid loops I set the internal variable
                    cd.Add(tmp);
                    tmp = tmp.next_version.value;                    
                } while (tmp != null);
                
            }
        } 


        /// <summary>
        /// Editable to implement 
        /// S-DMS-REQ-29
        /// The S-DMS shall allow the Owner of a content to move that content into any folder or sub-folder where the Owner has Add access.
        /// </summary>
        [ContentProperty(label = "DMS.VERSION.PARENT_FOLDER")]
        public MultiContentReference<DMSFolder> parent_folders { get; set; } = new MultiContentReference<DMSFolder>();

        public User owner { get; set; }

        /// <summary>
        /// attachments and comments.
        /// </summary>
        public IEnumerable<IContent> children
        {
            get
            {
                List<IContent> ret = new List<IContent>();
                ret.AddRange(comments);
                ret.AddRange(from x in attachments select x.value);
                return ret;
            }
        }

        
        [ContentProperty(label = "DMS.FOLDER.ADD_PERMISSION_USERS")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<User> add_permission_users { get; set; }
        
        [ContentProperty(label = "DMS.FOLDER.EDIT_PERMISSION_USERS")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<User> edit_permission_users { get; set; }

        [ContentProperty(label = "DMS.FOLDER.VIEW_PERMISSION_USERS")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<User> view_permission_users { get; set; }

        [ContentProperty(label = "DMS.FOLDER.ADD_PERMISSION_GROUP")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<DMSUserGroup> add_permission_groups { get; set; } = new MultiContentReference<DMSUserGroup>();

        [ContentProperty(label = "DMS.FOLDER.EDIT_PERMISSION_GROUP")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<DMSUserGroup> edit_permission_groups { get; set; } = new MultiContentReference<DMSUserGroup>();

        [ContentProperty(label = "DMS.FOLDER.VIEW_PERMISSION_GROUP")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<DMSUserGroup> view_permission_groups { get; set; } = new MultiContentReference<DMSUserGroup>();

        
        public IEnumerable<User> edit_permission
        {
            get
            {
                var result = new HashSet<User>();
                foreach (DMSUserGroup group in edit_permission_groups.getAsContentEnum())
                {
                    result.AddRange(group.all_members.getEnum());
                }
                result.AddRange(edit_permission_users.getEnum());
                result.Add(author);
                return result;
            }
        }

        public IEnumerable<User> add_permission
        {
            get
            {
                var result = new HashSet<User>();
                foreach (DMSUserGroup group in add_permission_groups.getAsContentEnum())
                {
                    result.AddRange(group.all_members.getEnum());
                }
                result.AddRange(add_permission_users.getEnum());
                result.Add(author);
                result.AddRange(approvers.getEnum());
                return result;
            }
        }

        public IEnumerable<User> view_permission
        {
            get
            {
                var result = new HashSet<User>();
                foreach (DMSUserGroup group in view_permission_groups.getAsContentEnum())
                {
                    result.AddRange(group.all_members.getEnum());
                }
                result.AddRange(view_permission_users.getEnum());
                return result;
            }
        }

        private MultiContentReference<User> _next_actors;
        /// <summary>
        /// DMS-31: pending actions.
        /// The idea is to implement a multireference to person that includes all the actors invlved for the enxt step.
        /// The first logic will be: the approver is in once the document is signed and active.
        /// Case 1: an approver is added. If the version is signed and active then the approver is added to this list
        /// Case 1b: an approver is removed. If the version is signed and active then the list is updated.
        /// Case 2: a version is marked as active. The list is updated.
        /// Case 2b: a version is marked as inactive. The list is empty.
        /// Case 3: a version is signed. The list is updated. 
        /// Case 4: a version is obsoleted. The list is empty.
        /// Case 5: a version is approved/disapproved. The list is empty.
        /// </summary>
        [ContentProperty(label = "DMS.VERSION.NEXT_ACTORS")]
        public MultiContentReference<User> next_actors { get; set; }


        [ContentProperty(label = "DMS.VERSION.WATCHERS")]
        [PropertyView]
        [PropertyEdit]
        [MultiReferencePropertyValuesProviderAtt]
        public MultiContentReference<User> watchers { get; set; }



        public IEnumerable<User> getSubscribers()
        {
            // by default the roles are watching this.
            List<User> ret = new List<User>();
            if(!author.isNullOrEmpty()) ret.Add(author.value);
            ret.AddRange(co_authors.getEnum());
            ret.AddRange(watchers.getEnum());
            return ret;
        }


        IEnumerable<IDMSContent> IDMSContent.children
        {
            get
            {
                return new List<IDMSContent>();
            }
        }
    }
}
