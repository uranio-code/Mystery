using Mystery.Content;
using Mystery.Register;
using System;
using System.Collections.Generic;

namespace Mystery.UI
{

    public interface IContentTypeButtonInputProvider
    {
        object getInput();
    }

    class ContentTypeButtonInputDefaultProvider : IContentTypeButtonInputProvider
    {

        private Type type;
        private ContentType content_type;
        public ContentTypeButtonInputDefaultProvider(Type type) {
            this.type = type;
            content_type = type.getMysteryAttribute<ContentType>();
        }

        public object getInput()
        {
            var result = new Dictionary<string,string>();
            result["huge"] = content_type == null ? type.Name : content_type.list_label;
            result["url"] = content_type == null ? string.Empty : "Type/" + content_type.name; 
            return result;
        }
    }


    public class ContentTypeButton : MysteryClassAttribute
    {

        private string _template_url;

        public string template_url {
            get {
                if (string.IsNullOrEmpty(_template_url))
                {
                    return "MysteryWebContent/MysteryContent/TypeButton.html";
                    ContentType ct = used_in.getMysteryAttribute<ContentType>();
                    if (ct != null) _template_url = "Directive/" + ct.name + "Button";
                    else throw new Exception(nameof(template_url) + " must be given for not ContentType Classes");
                }
                return _template_url;
            }
            set {
                _template_url = value;
            }
        }


        public Type implementing_type { get; set; }

        private IContentTypeButtonInputProvider _implemetation { get; set; } 

        public override void setUp()
        {
            
        }

        public object getInput() {
            if (_implemetation == null) {
                _implemetation = new ContentTypeButtonInputDefaultProvider(used_in);
                if (implementing_type != null)
                {
                    if (!typeof(IContentTypeButtonInputProvider).IsAssignableFrom(implementing_type))
                        throw new Exception(implementing_type.FullName + " must implement " + nameof(IContentTypeButtonInputProvider));
                    else
                        _implemetation = (IContentTypeButtonInputProvider)this.getGlobalObject<FastActivator>().createInstance(implementing_type);
                }
            }
            return _implemetation.getInput();
        }

    }
}
