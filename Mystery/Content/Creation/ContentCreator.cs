using Mystery.Register;
using System;
namespace Mystery.Content
{

    public interface IContentCreator
    {

        IContent create();

    }


    public class ContentCreatorAttribute : MysteryDefaultClassAttribute
    {


        public Type implementing_type { get; set; }


        Type generic_impl = typeof(BaseContentCreator<>);
        Type generic_with_uid = typeof(ContentWithUidCreator<>);
        Type generic_int = typeof(IContentCreator);

        IActivator _activator;

        public override void setUp()
        {
            if (implementing_type != null && !generic_int.IsAssignableFrom(implementing_type))
                throw new Exception(implementing_type.Name + " can not be used as " + generic_int.Name);

            if (implementing_type == null)
            {
                if (typeof(IContentWithUid).IsAssignableFrom(used_in))
                    implementing_type = generic_with_uid.MakeGenericType(used_in);
                else
                    implementing_type = generic_impl.MakeGenericType(used_in);
            }


            _activator = this.getGlobalObject<FastActivator>().getActivator(implementing_type);

        }

        public IContentCreator getCreator()
        {
            return (IContentCreator)_activator.createInstance();
        }

    }


}