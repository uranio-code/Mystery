using Mystery.Register;

namespace Mystery.Content
{

    /// <summary>
    /// which property shall display as "title" of a content
    /// </summary>
    /// <remarks></remarks>
    public class ReferenceText : MysteryPropertyAttribute
    {



        public override void setup()
        {
        }

        public string getReferenceText(IContent content)
        {
            string value = (string)this.retrive(content);
            if (value == null)
                return string.Empty;
            return value;
        }

    } 
}