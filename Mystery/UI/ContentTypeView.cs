using Mystery.Register;

namespace Mystery.UI
{
    /// <summary>
    /// this attribute manage what to see when you access a content type
    /// by default it will be its table
    /// </summary>
    public class ContentTypeView : MysteryClassAttribute
    {
        public string templateUrl { get; set; }
        public string controller { get; set; }

        public override void setUp()
        {
            if (string.IsNullOrEmpty(templateUrl))
            {
                //if we have no template we need also to decide the controller name
                //as it is used in the std template
                templateUrl = "MysteryWebContent/MysteryContent/ContentTypeListRoute.html";
            }

            if (string.IsNullOrEmpty(controller))
                controller = "ContentTypeListRouteController";
        }
    }
}
