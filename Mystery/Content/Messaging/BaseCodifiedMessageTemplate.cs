using Mystery.Content;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Messaging
{
    /// <summary>
    /// This class implements a codified message template. 
    /// A template is formed by:
    /// the language: the id of the supported language
    /// the code: this identifies the message we wnat to send. Two templates with different languages and same code are considered a translation.
    /// the text: this is the actual template. 
    /// examples:
    /// template{
    ///   code: 1,
    ///   language: EN,
    ///   text: @user created a folder in @parent_folder
    /// }
    /// template{
    ///   code: 1,
    ///   languase: IT,
    ///   text: @user ha creato una cartella in @parent_folder
    /// }
    /// The two objects above are considered a translation of the message 1.
    /// </summary>
    [ContentType(label = "MESSANGING.TEMPLATE", list_label = "MESSANGING.TEMPLATES")]
    [ContentTypeButton(template_url = "MysteryWebContent/MysteryContent/TypeButton.html")]
    [ContentTypeView]
    [ContentView]
    public class BaseCodifiedMessageTemplate : BaseContent
    {
        [ContentProperty(label = "MESSAGING.CODE")]
        [PropertyView]
        [PropertyEdit]
        [PropertyColumn]
        public string code { get; set; }

        [ContentProperty(label = "MESSAGING.LANGUAGE")]
        [PropertyView]
        [PropertyEdit]
        [PropertyColumn]
        public string language { get; set; }

        [ContentProperty(label = "MESSAGING.TEMPLATE")]
        [PropertyView]
        [PropertyEdit]
        [PropertyColumn]
        public string template { get; set; }
    }
}
