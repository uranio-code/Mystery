using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using Mystery.Register;
using Mystery.Web;
using MysteryDMS.Actions;
using MysteryDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    [TestClass()]
    public class PathTest
    {

        [TestMethod()]
        public void FloatingFolderShouldHaveOnly1PathWithThemInIt()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                var folder = cc.getAndAddNewContent<DMSFolder>();
                var input = new ContentReference(folder);
                var action = new GetDmsPathsAction();
                action.input = input;
                action.Authorize();
                var paths = action.Execute().output.paths;
                Assert.AreEqual(1, paths.Count());
                Assert.AreEqual(1, paths.FirstOrDefault().Count());
                Assert.AreEqual(folder.guid, paths.FirstOrDefault().FirstOrDefault().guid);
            }
        }
        [TestMethod()]
        public void FloatingVersionShouldHaveNoPaths()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                var version = cc.getAndAddNewContent<DMSVersion>();
                var input = new ContentReference(version);
                var action = new GetDmsPathsAction();
                action.input = input;
                action.Authorize();
                var paths = action.Execute().output.paths;
                Assert.AreEqual(0, paths.Count());
            }
        }
        [TestMethod()]
        public void FolderInFolderShouldGet1PathWith2Folder()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                var folder = cc.getAndAddNewContent<DMSFolder>();
                var parent_folder = cc.getAndAddNewContent<DMSFolder>();
                folder.parent_folders.Add(parent_folder);
                var input = new ContentReference(folder);
                var action = new GetDmsPathsAction();
                action.input = input;
                action.Authorize();
                var paths = action.Execute().output.paths;
                Assert.AreEqual(1, paths.Count());
                Assert.AreEqual(2, paths.FirstOrDefault().Count());
                Assert.AreEqual(parent_folder.guid, paths.FirstOrDefault().FirstOrDefault().guid);
                Assert.AreEqual(folder.guid, paths.FirstOrDefault().Skip(1).FirstOrDefault().guid);
            }
        }
        [TestMethod()]
        public void FolderIn2FolderShouldGet2PathWith2Folder()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                var folder = cc.getAndAddNewContent<DMSFolder>();
                var parent_folder1 = cc.getAndAddNewContent<DMSFolder>();
                var parent_folder2 = cc.getAndAddNewContent<DMSFolder>();
                folder.parent_folders.Add(parent_folder1);
                folder.parent_folders.Add(parent_folder2);
                var input = new ContentReference(folder);
                var action = new GetDmsPathsAction();
                action.input = input;
                action.Authorize();
                var paths = action.Execute().output.paths;
                Assert.AreEqual(2, paths.Count());
                Assert.AreEqual(2, paths.FirstOrDefault().Count());
                Assert.AreEqual(2, paths.Skip(1).FirstOrDefault().Count());



                //we don't know the order
                var parent_guids = new HashSet<Guid>() { parent_folder1.guid, parent_folder2.guid };
                foreach (var path in paths)
                {
                    Assert.IsTrue(parent_guids.Contains(path.FirstOrDefault().guid));
                }
            }
        }

        [TestMethod()]
        public void VersionIn2FolderShouldGet2PathWith1Folder()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                var version = cc.getAndAddNewContent<DMSVersion>();
                var parent_folder1 = cc.getAndAddNewContent<DMSFolder>();
                var parent_folder2 = cc.getAndAddNewContent<DMSFolder>();
                version.parent_folders.Add(parent_folder1);
                version.parent_folders.Add(parent_folder2);
                var input = new ContentReference(version);
                var action = new GetDmsPathsAction();
                action.input = input;
                action.Authorize();
                var paths = action.Execute().output.paths;
                Assert.AreEqual(2, paths.Count());
                Assert.AreEqual(1, paths.FirstOrDefault().Count());
                Assert.AreEqual(1, paths.Skip(1).FirstOrDefault().Count());
                //we don't know the order
                var parent_guids = new HashSet<Guid>() { parent_folder1.guid, parent_folder2.guid };
                foreach (var path in paths)
                {
                    Assert.IsTrue(parent_guids.Contains(path.FirstOrDefault().guid));
                }
            }
        }

        [TestMethod()]
        public void ParentFolderContainedInChildShouldNoGoRecursive()
        {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                var folder1 = cc.getAndAddNewContent<DMSFolder>();
                var folder2 = cc.getAndAddNewContent<DMSFolder>();
                folder1.parent_folders.Add(folder2);
                folder2.parent_folders.Add(folder1);
                var input = new ContentReference(folder1);
                var action = new GetDmsPathsAction();
                action.input = input;
                action.Authorize();
                var paths = action.Execute().output.paths;

                Assert.AreEqual(1, paths.Count());
                Assert.AreEqual(2, paths.FirstOrDefault().Count());

                input = new ContentReference(folder2);
                action = new GetDmsPathsAction();
                action.input = input;
                action.Authorize();
                paths = action.Execute().output.paths;

                Assert.AreEqual(1, paths.Count());
                Assert.AreEqual(2, paths.FirstOrDefault().Count());
            }
        }
    }
}
