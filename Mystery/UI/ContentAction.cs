using Mystery.Content;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.Users;
using System;
using System.Collections.Generic;

namespace Mystery.UI
{
    public class ContentInput { public string tiny_guid { get; set; } public string ContentType { get; set; } }

    public interface IContentActionButton
    {
        string name { get; }

        Button button { get; }

        Modal modal { get; }

    }

    public class ContentActionOutput
    {
        public List<IContent> new_contents { get; set; } = new List<IContent>();
        public List<IContent> changed_contents { get; set; } = new List<IContent>();
        public List<IContent> deleted_contents { get; set; } = new List<IContent>();
        public List<IContent> contents { get; set; } = new List<IContent>();
        public List<IContent> other_contents { get; set; } = new List<IContent>();
        public IContent main { get; set; }
        public string message { get; set; }
        public IMysteryUrl next_url { get; set; }
    }

    public abstract class BaseContentAction :
        BaseMysteryAction<IContent, ContentActionOutput>, IContentActionButton
    {

        public BaseContentAction(IContent content, User user)
        {
            this.user = user;
            this.input = content;
        }

        public abstract string label { get; }

        virtual public Button button
        {
            get
            {
                return new Button
                {
                    enabled = this.Authorize(),
                    label = this.label,
                };
            }
        }

        virtual public Modal modal
        {
            get
            {
                return null;
            }
        }

        virtual public string name
        {
            get
            {
                return this.GetType().FullName;
            }
        }
    }



    public interface IContentButtonProvider
    {
        IEnumerable<IContentActionButton> getActions(IContent content, User user);
    }

    public class DefaultContentActionMenuProvider<ContentType> : IContentButtonProvider where ContentType : IContent, new()
    {
        public IEnumerable<IContentActionButton> getActions(IContent content, User user)
        {
            var result = new List<IContentActionButton>();
            if (content == null || user == null)
                return result;
            var type = content.GetType();
            if (!content.canAccess(user))
                return result;
            if (type.getMysteryAttribute<ContentDeleteAttribute>().canDelete(content, user))
                result.Add(new ContentDeleteAction(content, user));
            if(user.isFavorite(content))
                result.Add(new RemoveFromFavoriteAction(content, user));
            else
                result.Add(new AddToFavoriteAction(content,user));
            
            return result;
        }
    }

    public class ContentAction : MysteryDefaultClassAttribute
    {
        public Type implementing_type { get; set; }

        private IContentButtonProvider _implemetation { get; set; }

        public override void setUp()
        {
            if (implementing_type == null) implementing_type = typeof(DefaultContentActionMenuProvider<>).MakeGenericType(used_in);
            if (!typeof(IContentButtonProvider).IsAssignableFrom(implementing_type))
            {
                throw new Exception(implementing_type.FullName + " must implement " + typeof(IContentButtonProvider).FullName);
            }
            _implemetation = (IContentButtonProvider)this.getGlobalObject<FastActivator>().createInstance(implementing_type);

        }

        public IEnumerable<IContentActionButton> getActions(IContent content, User user)
        {
            return _implemetation.getActions(content, user);
        }

    }

    /// <summary>
    /// By default a content in mystery has no workflow.
    /// </summary>
    /// <typeparam name="ContentType"></typeparam>
    public class DefaultContentWorkflowActionMenuProvider<ContentType> : IContentButtonProvider where ContentType : IContent, new()
    {
        public IEnumerable<IContentActionButton> getActions(IContent content, User user)
        {
            return new List<IContentActionButton>();            
        }
    }

    public class ContentWorkflowAction : MysteryDefaultClassAttribute
    {
        public Type implementing_type { get; set; }

        private IContentButtonProvider _implemetation { get; set; }

        public override void setUp()
        {
            if (implementing_type == null) implementing_type = typeof(DefaultContentWorkflowActionMenuProvider<>).MakeGenericType(used_in);
            if (!typeof(IContentButtonProvider).IsAssignableFrom(implementing_type))
            {
                throw new Exception(implementing_type.FullName + " must implement " + typeof(IContentButtonProvider).FullName);
            }
            _implemetation = (IContentButtonProvider)this.getGlobalObject<FastActivator>().createInstance(implementing_type);

        }

        public IEnumerable<IContentActionButton> getActions(IContent content, User user)
        {
            return _implemetation.getActions(content, user);
        }

    }



}
