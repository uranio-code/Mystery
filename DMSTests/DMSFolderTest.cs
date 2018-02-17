using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using Mystery.Register;
using Mystery.Users;
using MysteryDMS.Model;
using System.Collections.Generic;

namespace IntegrationTests
{
    [TestClass()]
    public class DMSFolderTest
    {

        [TestMethod()]
        public void UserGRoupAllMembers()
        {
            // This test checks if the recoursion works properly
            // I create groups 1,2,3 with User a,b,c stored in them respectively.
            // I then nest 2 into 1 and 3 into 2.
            // I need the content creator
            var cc = this.getGlobalObject<IGlobalContentCreator>();
            DMSUserGroup one = cc.getNewContent<DMSUserGroup>();
            DMSUserGroup two = cc.getNewContent<DMSUserGroup>();
            DMSUserGroup three = cc.getNewContent<DMSUserGroup>();

            var userA = new User();
            userA.fullname = "1";
            one.members.Add(userA);

            var userB = new User();
            userB.fullname = "2";
            two.members.Add(userB);

            var userC = new User();
            userC.fullname = "3";
            three.members.Add(userC);

            one.sub_groups.Add(two);
            two.sub_groups.Add(three);

            // all_members of 1 should contain a,b,c
            Assert.IsTrue(3 == one.all_members.Count);
            Assert.IsTrue(one.all_members.Contains(userA));
            Assert.IsTrue(one.all_members.Contains(userB));
            Assert.IsTrue(one.all_members.Contains(userC));

            // all_members of 2 should contain b,c
            Assert.IsTrue(2 == two.all_members.Count);
            Assert.IsTrue(two.all_members.Contains(userB));
            Assert.IsTrue(two.all_members.Contains(userC));

            // all_members of 3 should contain c
            Assert.IsTrue(1 == three.all_members.Count);
            Assert.IsTrue(three.all_members.Contains(userC));

        }

        [TestMethod()]
        public void ContentServiceHelloTest()
        {
            // I need the content creator
            var cc = this.getGlobalObject<IGlobalContentCreator>();

            // I create a folder
            var folder = cc.getNewContent<DMSFolder>();
            var guid = folder.guid;

            // I store it to the database
            var cd = this.getGlobalObject<IContentDispatcher>();
            cd.Add(folder);

            // I read the folder from the database
            var from_db = cd.GetContent<DMSFolder>(guid);

            // I check if it is the same folder
            Assert.IsTrue( folder.samePropertiesValue(from_db));
        }

        [TestMethod()]
        public void ViewPermissionsfolder()
        {
            //fail: DMS-57

            // I create a folder
            var cc = this.getGlobalObject<IGlobalContentCreator>();
            var folder = cc.getNewContent<DMSFolder>();
            
            // I create user1, user2, user3
            var userA = new User();
            userA.fullname = "1";

            var userB = new User();
            userB.fullname = "2";

            var userC = new User();
            userC.fullname = "3";

            // Icreate a group
            DMSUserGroup one = cc.getNewContent<DMSUserGroup>();

            // I put user1 in the group
            one.members.Add(userA);

            // I set user2 in the folder
            folder.view_permission_users.Add(userB);

            // I set the group in the folder
            folder.view_permission_groups.Add(one);

            // assert user 1 has view permisison
            Assert.IsTrue(folder.view_permission.Contains(userA));

            // assert user 2 has view permission
            Assert.IsTrue(folder.view_permission.Contains(userB));

            // assert user3 doesn't have view permission
            Assert.IsFalse(folder.view_permission.Contains(userC));
        }


        [TestMethod()]
        public void EditPermissionsfolder()
        {
            //fail: DMS-57

            // I create a folder
            var cc = this.getGlobalObject<IGlobalContentCreator>();
            var folder = cc.getNewContent<DMSFolder>();

            // I create user1, user2, user3
            var userA = new User();
            userA.fullname = "1";

            var userB = new User();
            userB.fullname = "2";

            var userC = new User();
            userC.fullname = "3";

            // Icreate a group
            DMSUserGroup one = cc.getNewContent<DMSUserGroup>();

            // I put user1 in the group
            one.members.Add(userA);

            // I set user2 in the folder
            folder.edit_permission_users.Add(userB);

            // I set the group in the folder
            folder.edit_permission_groups.Add(one);

            // assert user 1 has view permisison
            Assert.IsTrue(folder.edit_permission.Contains(userA));

            // assert user 2 has view permission
            Assert.IsTrue(folder.edit_permission.Contains(userB));

            // assert user3 doesn't have view permission
            Assert.IsFalse(folder.edit_permission.Contains(userC));
        }


        [TestMethod()]
        public void AddPermissionsfolder()
        {
            //fail: DMS-57

            // I create a folder
            var cc = this.getGlobalObject<IGlobalContentCreator>();
            var folder = cc.getNewContent<DMSFolder>();

            // I create user1, user2, user3
            var userA = new User();
            userA.fullname = "1";

            var userB = new User();
            userB.fullname = "2";

            var userC = new User();
            userC.fullname = "3";

            // Icreate a group
            DMSUserGroup one = cc.getNewContent<DMSUserGroup>();

            // I put user1 in the group
            one.members.Add(userA);

            // I set user2 in the folder
            folder.add_permission_users.Add(userB);

            // I set the group in the folder
            folder.add_permission_groups.Add(one);

            // assert user 1 has view permisison
            Assert.IsTrue(folder.add_permission.Contains(userA));

            // assert user 2 has view permission
            Assert.IsTrue(folder.add_permission.Contains(userB));

            // assert user3 doesn't have view permission
            Assert.IsFalse(folder.add_permission.Contains(userC));
        }


    }
}
