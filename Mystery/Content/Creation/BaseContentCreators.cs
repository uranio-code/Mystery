using Mystery.Register;
using System.ComponentModel;

namespace Mystery.Content
{


    public class BaseContentCreator<T> : IContentCreator where T : IContent, new()
    {

        public IContent create()
        {
            var result = new T();

            //instance all reference property so we can avoid a bunch of null checks
            var activator = this.getGlobalObject<FastActivator>();
            foreach (ContentProperty content_property in typeof(T).getMysteryPropertyAttributes<ContentProperty>())
            {
                if (!typeof(IReferenceProperty).IsAssignableFrom(content_property.used_in.PropertyType))
                    continue; //not a reference
                if (content_property.retrive(result) != null)
                    continue; //instanced by the class definition we mind our business 

                content_property.save(result, activator.createInstance(content_property.used_in.PropertyType));
                
            }

            if (result is ISupportInitialize)
                ((ISupportInitialize)result).EndInit();
            return result;
        }
    }

    public class ContentWithUidCreator<T> : IContentCreator where T : IContentWithUid, new()
    {

        private void ContentSealed(IContent item)
        {
            IContentWithUid content = (IContentWithUid)item;
            IUidProvider provider = this.getGlobalObject<IUidProvider>();
            provider.registerUid((string)content.uid, item.guid);
            content.contentSealed -= ContentSealed;
        }



        public IContent create()
        {
            T result = new T();
            IUidProvider provider = this.getGlobalObject<IUidProvider>();
            result.uid = provider.getUid();
            result.contentSealed += ContentSealed;
            return result;
        }

    }


}