using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.Register;
using Mystery.Users;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.UI
{
    
    public class AddToFavoriteAction : BaseContentAction
    {
        public AddToFavoriteAction(IContent content, User user) : base(content, user)
        {
        }

        public override string name { get { return "add_to_favorite"; } }
        public override string label { get { return "COMMON.ADD_TO_FAVORITE"; } }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "star-o";
                result.style = "warning btn-outline";
                return result;
            }
        }


        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            IContent content = cd.GetContent(input.getContenTypeName(), input.guid);

            if (content == null) return ActionResultTemplates<ContentActionOutput>.InvalidInput;

            var result = new ContentActionOutput();
            if (user.isFavorite(content))
                return result;

            var cc = this.getGlobalObject<IGlobalContentCreator>();
            var favorite = cc.getAndAddNewContent<UserFavorite>();
            favorite.user = user;
            favorite.content_reference = new ContentReference(content);
            result.new_contents.Add(favorite);
            //we add also the content so the ui will reload it with the new actions
            result.changed_contents.Add(content);

            return result;
        }

        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            IContent content = cd.GetContent(input.getContenTypeName(), input.guid);
            if (content == null) return false;
            return !user.isFavorite(content);
        }
    }

    public class RemoveFromFavoriteAction : BaseContentAction
    {
        public RemoveFromFavoriteAction(IContent content, User user) : base(content, user)
        {
        }

        public override string name { get { return "remove_from_favorite"; } }
        public override string label { get { return "COMMON.REMOVE_FROM_FAVORITE"; } }

        public override Button button
        {
            get
            {
                var result = base.button;
                result.font_awesome_icon = "star";
                result.style = "warning btn-outline";
                return result;
            }
        }


        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            IContent content = cd.GetContent(input.getContenTypeName(), input.guid);

            if (content == null) return ActionResultTemplates<ContentActionOutput>.InvalidInput;

            var result = new ContentActionOutput();
            if (!user.isFavorite(content))
                return result;

            var favorites = cd.GetAllByFilter<UserFavorite>(x => x.user.guid == user.guid && x.content_reference.guid == content.guid);
            cd.RemoveContents(favorites);
            result.deleted_contents.AddRange(favorites);
            //we add also the content so the ui will reload it with the new actions
            result.changed_contents.Add(content);
            return result;
        }

        protected override bool AuthorizeImplementation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            IContent content = cd.GetContent(input.getContenTypeName(), input.guid);
            if (content == null) return false;
            return user.isFavorite(content);
        }
    }

    [PublishedAction(input_type:null,url = nameof(GetFavorites))]
    public class GetFavorites : BaseMysteryAction<ContentActionOutput>
    {
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            var favorites = from x in  cd.GetAllByFilter<UserFavorite>(x => x.user.guid == user.guid )
                            where x.content_reference != null
                            select x.content_reference.getContent();

            var result = new ContentActionOutput();
            result.contents.AddRange(from x in favorites where x != null && x.canAccess(user) select x );
            return result;
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
}
