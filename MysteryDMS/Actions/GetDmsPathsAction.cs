using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.Register;
using MysteryDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryDMS.Actions
{

    public class GetDmsPathsActionOutput {
        public IEnumerable<IEnumerable<DMSFolder>> paths = new List<IEnumerable<DMSFolder>>();
    }

    public class GetDmsPathsAction : BaseMysteryAction<ContentReference, GetDmsPathsActionOutput>
    {

        private IEnumerable<ICollection<DMSFolder>> getPathsForFolder(DMSFolder folder, DMSFolder orign) {
            if (folder == null || !folder.canAccess(user))
                return new List<ICollection<DMSFolder>>();
            var parents = (from x in  folder.parent_folders.getEnum() where x.canAccess(user) select x);
            ICollection<ICollection<DMSFolder>> result = new LinkedList<ICollection<DMSFolder>>();
            foreach (var parent in parents) {
                if (parent.Equals(orign)) {
                    //we do have recursion
                    continue;
                }
                var from_parent = getPathsForFolder(parent,orign);
                foreach (var path in from_parent) {
                    path.Add(folder);
                }
                result.AddRange(from_parent);
            }
            //if I have no parents (or none accessible) my result will be empty
            //in this case the result became a single path with the current folder only
            if (result.FirstOrDefault() == null)
            {
                var me_path = new List<DMSFolder>() { folder };
                result.Add(me_path);
            }

            return result;
        }

        private IEnumerable<ICollection<DMSFolder>> getPathsForDocument(DMSVersion version)
        {
            if (version == null || !version.canAccess(user))
                return new List<ICollection<DMSFolder>>();
            var parents = (from x in version.parent_folders.getEnum() where x.canAccess(user) select x);
            ICollection<ICollection<DMSFolder>> result = new LinkedList<ICollection<DMSFolder>>();
            foreach (var parent in parents)
            {
                var from_parent = getPathsForFolder(parent, parent);
                result.AddRange(from_parent);
            }
            //if I have no parents (or none accessible) my result will be empty
            return result;
        }

        protected override ActionResult<GetDmsPathsActionOutput> ActionImplemetation()
        {

            if (input == null)
                return ActionResultTemplates<GetDmsPathsActionOutput>.InvalidInput;

            var content = input.getContent();
            if (content == null)
                return ActionResultTemplates<GetDmsPathsActionOutput>.InvalidInput;

            var result = new GetDmsPathsActionOutput();

            if (content is DMSFolder)
            {
                result.paths = getPathsForFolder((DMSFolder)content, (DMSFolder)content);
            }
            else if (content is DMSVersion)
            {
                result.paths = getPathsForDocument((DMSVersion)content);
            }
            else {
                return ActionResultTemplates<GetDmsPathsActionOutput>.InvalidInput;
            }

            return result;

        }

        protected override bool AuthorizeImplementation()
        {
            
            //user need access to the content
            if (input == null)
                return true;
            var content = input.getContent();
            if (content == null)
                return true;
            var result = content.canAccess(user);
            return result;
        }
    }
}
