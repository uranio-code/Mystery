using Mystery.Register;

namespace Mystery.UI
{
    public class ContentView : MysteryClassAttribute
    {

        public string templateUrl {get; set; }
        public string controller { get; set; }
         
        
        public override void setUp()
        {
            if (string.IsNullOrEmpty(templateUrl))
            {
                //if we have no template we need also to decide the controller name
                //as it is used in the std template
                templateUrl = "MysteryWebContent/MysteryContent/View.html";
            }
                
            if (string.IsNullOrEmpty(controller))
                controller = "ContentViewController";
        }
    }
    
}
