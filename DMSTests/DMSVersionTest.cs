using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using Mystery.Json;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using MysteryDMS;
using MysteryDMS.Actions;
using MysteryDMS.Model;
using System;
using System.Linq;

namespace IntegrationTests
{

    [TestClass()]
    public class DMSVersionTest
    {
        /// <summary>
        /// Create the version 1 inside a folder
        /// </summary>
        [TestMethod()]
        public void CreateNewDocumentTest()
        {


            using (WebActionExecutor e = new WebActionExecutor())
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                var folder = cc.getNewContent<DMSFolder>();
                var guid = folder.guid;
                var cd = this.getGlobalObject<IContentDispatcher>();
                cd.Add(folder);

                var addDocument = new DMSCreateNewDocument(folder, null);

                WebActionResult result = e.executeAction(addDocument);
                Assert.IsTrue(result.isSuccessfull);
                
                IDMSContent created = folder.children.FirstOrDefault();

                Assert.IsNotNull(created);


                Assert.IsTrue(((DMSVersion)created).version_number == 1);
            }

        }

        /// <summary>
        /// Creates the version 2.
        /// </summary>
        [TestMethod()]
        public void CheckAddVersionTest()
        {

            using (WebActionExecutor e = new WebActionExecutor())
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                // create a folder
                var folder = cc.getNewContent<DMSFolder>();
                folder.title = "the folder";

                var cd = this.getGlobalObject<IContentDispatcher>();
                cd.Add(folder);

                // create a version inside the folder
                DMSVersion created = cc.getNewContent<DMSVersion>();
                created.title = "Pippo";
                created.parent_folders.Add(folder);
                cd.Add(created);

                // add a new version
                var addNewVersion = new DMSAddVersion(created, null);

                WebActionResult result = e.executeAction(addNewVersion);
                Assert.IsTrue(result.isSuccessfull);

                // reload the original version from the database to get the next version
                Guid version_guid = created.guid;
                created = cd.GetContent<DMSVersion>(version_guid);
                DMSVersion new_version = created.next_version;

                // exist the next version to the original one
                Assert.IsNotNull(new_version);

                // exist a previous version to the new version
                Assert.IsNotNull(new_version.previous_version);

                // created and the previous of the next are the same versions
                Assert.IsTrue(created.samePropertiesValue(new_version.previous_version.value));

                // new version and the next of the previous are the same
                Assert.IsTrue(new_version.samePropertiesValue(created.next_version.value));

                // new version and original are in the same folder
                folder = cd.GetContent<DMSFolder>(folder.guid);
                DMSFolder new_folder = created.parent_folders.FirstOrDefault();
                
                Assert.IsTrue(folder.samePropertiesValue(new_folder));

                // the new version has version number == 2
                Assert.IsTrue(new_version.version_number == 2);
            }

        }

        

        [TestMethod()]
        public void check_document_deletion()
        {
            using (WebActionExecutor e = new WebActionExecutor())
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                var cd = this.getGlobalObject<IContentDispatcher>();

                // get the RecycleBin, also making sure it exist
                DMSRecycleBin bin = this.getGlobalObject<DMSRecycleBin>();

                // create a version
                DMSVersion created = cc.getNewContent<DMSVersion>();
                var folder = cc.getNewContent<DMSFolder>();
                created.title = "Pippo";
                created.parent_folders.Add(folder);
                cd.Add(folder);
                cd.Add(created);

                var add_version_action = new DMSAddVersion(created, null);

                WebActionResult result = e.executeAction(add_version_action);
                created = cd.GetContent<DMSVersion>(created.guid);
                DMSVersion created_2 = created.next_version;
                Assert.IsNotNull(created_2);

                result = e.executeAction(add_version_action);
                created_2 = cd.GetContent<DMSVersion>(created_2.guid);
                DMSVersion created_3 = created_2.next_version;
                Assert.IsNotNull(created_3);
                                
                // delete action
                var delete_document_action = new DMSDeleteDocument(created, null);
                result = e.executeAction(delete_document_action);
                Assert.IsTrue(result.isSuccessfull);

                created = cd.GetContent<DMSVersion>(created.guid);
                Assert.IsTrue(created.parent_folders.FirstOrDefault().guid == bin.guid);

                created_2 = cd.GetContent<DMSVersion>(created_2.guid);
                Assert.IsTrue(created_2.parent_folders.FirstOrDefault().guid == bin.guid);

                created_3 = cd.GetContent<DMSVersion>(created_3.guid);
                Assert.IsTrue(created_3.parent_folders.FirstOrDefault().guid == bin.guid);

            }
        }


        [TestMethod()]
        public void check_workflow_actions_excecution()
        {

            using (WebActionExecutor e = new WebActionExecutor())
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                var cd = this.getGlobalObject<IContentDispatcher>();

                // create a version
                DMSVersion created = cc.getNewContent<DMSVersion>();
                created.title = "Pippo";
                cd.Add(created);

                // sign action
                var sign_version_action = new DMSSignAction(created, null);

                WebActionResult result = e.executeAction(sign_version_action);
                Assert.IsTrue(result.isSuccessfull);

                created = cd.GetContent<DMSVersion>(created.guid);
                Assert.IsTrue(created.wf_Status == DMSConstants.wf_signed);

                // approve action
                var approve_version_action = new DMSVersionApproveAction(created, null);

                result = e.executeAction(approve_version_action);
                Assert.IsTrue(result.isSuccessfull);

                created = cd.GetContent<DMSVersion>(created.guid);
                Assert.IsTrue(created.wf_Status == DMSConstants.wf_approved);


                // under review action
                var under_review_version_action = new DMSVersionUnderReviewAction(created, null);

                result = e.executeAction(under_review_version_action);
                Assert.IsTrue(result.isSuccessfull);

                created = cd.GetContent<DMSVersion>(created.guid);
                Assert.IsTrue(created.wf_Status == DMSConstants.wf_under_review);


                // disapprove action
                var disapprove_version_action = new DMSVersionDisapproveAction(created, null);

                result = e.executeAction(disapprove_version_action);
                Assert.IsTrue(result.isSuccessfull);

                created = cd.GetContent<DMSVersion>(created.guid);
                Assert.IsTrue(created.wf_Status == DMSConstants.wf_disapproved);


                // obsolete action
                var obsolete_version_action = new DMSObsoleteVersionAction(created, null);

                result = e.executeAction(obsolete_version_action);
                Assert.IsTrue(result.isSuccessfull);

                created = cd.GetContent<DMSVersion>(created.guid);
                Assert.IsTrue(created.wf_Status == DMSConstants.wf_obsolete);


                // delete action
                var delete_version_action = new DMSDeleteVersion(created, null);

                result = e.executeAction(delete_version_action);
                Assert.IsTrue(result.isSuccessfull);

                created = cd.GetContent<DMSVersion>(created.guid);
                Assert.IsTrue(created.status==DMSConstants.obsolete);
            }

        }


        [TestMethod()]
        public void getComments()
        {
            using (WebActionExecutor e = new WebActionExecutor())
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                var cd = this.getGlobalObject<IContentDispatcher>();

                // create a version
                DMSVersion created = cc.getNewContent<DMSVersion>();
                created.title = "Pippo";
                cd.Add(created);

                // sign action
                var addCommentAction = new DMSAddCommentAction(created, null);
                var input = new AddCommentInput();
                input.comment_text = "ciao";
                input.content = created;

                addCommentAction.input = input;

                WebActionResult result = e.executeAction(addCommentAction);
                Assert.IsTrue(result.isSuccessfull);
                                
                Assert.IsTrue(created.comments.Count() == 1);


            }
        }
    }
}
