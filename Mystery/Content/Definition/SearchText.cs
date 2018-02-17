using Mystery.Register;
namespace Mystery.Content
{

    /// <summary>
    /// which property shall be the searchable text of the content
    /// </summary>
    /// <remarks></remarks>
    public class SearchText : MysteryPropertyAttribute
    {



        public override void setup()
        {
        }

        public string getSearchText(IContent content)
        {
            object value = this.retrive(content);
            if (value == null)
                return string.Empty;
            string as_string = (string)value;
            as_string = as_string.Trim().ToLower();
            return as_string;
        }

    } 
}