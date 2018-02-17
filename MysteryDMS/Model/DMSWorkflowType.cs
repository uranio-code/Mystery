using Mystery.Content;
using Mystery.UI;

namespace MysteryDMS.Model
{

    /// <summary>
    /// DMS Allows you to define the workflow type to stick to a version. It will be defined by the users
    /// from the UI itself.
    /// </summary>
    [ContentType(label = "DMS.VERSION_WORKFLOW", list_label = "DMS.VERSION_WORKFLOWS")]
    [ContentTypeButton(template_url = "MysteryWebContent/MysteryContent/TypeButton.html")]
    [ContentTypeView]
    public class DMSWorkflowType : BaseContent
    {
        [ContentProperty(label = "DMS.WORKFLOW.TITLE"), ReferenceText()]
        [PropertyColumn(template_url = "MysteryWebContent/MysteryContent/Properties/StringPropertyLinkedCell.html")]
        [PropertyView]
        [PropertyEdit]
        public string title { get; set; }

        [ContentProperty(label = "DMS.WORKFLOW.DESCRIPTION")]
        [PropertyView(template_url = "MysteryWebContent/MysteryContent/Properties/TextPropertyNonModal.html")]
        [PropertyEdit]
        public string description { get; set; }

    }

}
