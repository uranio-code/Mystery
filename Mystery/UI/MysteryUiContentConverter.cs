using Mystery.Content;
using Mystery.Json;
using Mystery.Register;
using Mystery.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mystery.UI
{

    public class MonoPropertyContentJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            //we get something as
//            {
//                "guid": "3b18bf6b-0d6d-4d51-9644-ea55969f07fe",
//  "a_string": "d0ed5b2e-a0a1-4ab1-a1b7-a0a52f51e0a7",
//}

            //to parse we will add the content type and set only the given property

            var result = new MonoPropertyContent();
            if (reader.TokenType == JsonToken.Null)
                return result;
            JObject jo = JObject.Load(reader);
            var converter = this.getGlobalObject<IMysteryJsonConverter>();
            result.content_reference = ContentReference.tryGetContentReferece(
                jo.GetValue(nameof(ContentType)).Value<string>(),
                jo.GetValue(nameof(IContent.guid)).Value<string>());


            var content = result.content_reference == null ? null : result.content_reference.getContent();

            foreach (var jp in jo.Children<JProperty>()) {
                if(jp.Name == nameof(IContent.guid))
                    continue;
                result.property_name = jp.Name;
                if (content == null)
                    continue;
                var property = (from x in content.GetType().getMysteryPropertyAttributes<ContentProperty>()
                                where x.name == result.property_name
                                select x).FirstOrDefault();
                if (property == null)
                    continue;

                IContent content_from_json = (IContent)converter.readJson(jo.ToString(), content.GetType());

                result.property_value = property.retrive(content_from_json);

                return result;
            }

            return result;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            var mono_property_content = (MonoPropertyContent)value;
            JObject jo = new JObject();
            jo.Add(nameof(IContent.guid), mono_property_content.content_reference.guid.ToString());
            jo.Add(nameof(ContentType), mono_property_content.content_reference.ContentType);
            var converter = this.getGlobalObject<MysteryJsonUiConverter>();

            jo.Add(mono_property_content.property_name, 
                JToken.Parse(converter.getJson(mono_property_content.property_value)));
            var sort_value = JsSortValue.getSortValue(mono_property_content.property_value);
            if(sort_value != null)
                jo.Add(mono_property_content.property_name + "_sort_value",
                JToken.FromObject(sort_value));
            else
                jo.Add(mono_property_content.property_name + "_sort_value",
                null);

            jo.WriteTo(writer);
        }
    }

    /// <summary>
    /// class which is used to be serialized as json and act like a content with 1 property
    /// </summary>
    [JsonConverter(typeof(MonoPropertyContentJsonConverter))]
    public class MonoPropertyContent
    {

        public ContentReference content_reference { get; set; }

        
        public string property_name { get; set; }

        public object property_value { get; set; }

        public MonoPropertyContent() { }

        public MonoPropertyContent(IContent content, ContentProperty property) {
            content_reference = new ContentReference(content);
            property_name = property.name;
            property_value = property.retrive(content);
        }
    }

    public class PropertyUI {

        public string uid = Guid.NewGuid().Tiny();
        public MonoPropertyContent content{ get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public string template_url { get; set; }
        public string help_html { get; set; }
        public bool can_edit { get; set; }
    }

    public class ContentUi {
        public bool authorized { get; set; }
        public Dictionary<string, PropertyUI> propertiesUi = new Dictionary<string, PropertyUI>();
    }

    
    public class MysteryUiContentConverter: JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IContent).IsAssignableFrom(objectType);
        }

        public ContentUi getContentUi(IContent content,Users.User user)
        {

            var result = new ContentUi();
            if (content == null) return result;
            
            var type = content.GetType();

            foreach (PropertyView property_view in type.getMysteryPropertyAttributes<PropertyView>()) {
                IPropertyAccess pa = property_view.getPropertyAccess();
                if (!pa.canAccess(content, user))
                    continue;

                var cp = property_view.used_in.getMysteryAttribute<ContentProperty>();
                if (cp == null) continue;
                
                var pe = property_view.used_in.getMysteryAttribute<PropertyEdit>();

                result.propertiesUi[cp.name] = new PropertyUI
                {
                    content = new MonoPropertyContent(content,cp),
                    name = cp.name,
                    label = cp.label,
                    template_url = pa.template_url,
                    can_edit = pe==null?false:pe.getPropertyEdit().canEdit(content,user),
                    help_html = property_view.help_html,
                };
            }

            result.authorized = content.canAccess(user);

            return result;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //ui json reading will use the same as BLL json
            var converter = new ContentConverter();
            return converter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var c = (IContent)value;
            MysterySession session = this.getGlobalObject<MysterySession>();
            var content_ui = this.getContentUi(c, session.user);
            JObject jo = JObject.FromObject(content_ui, serializer);
            //let's make our json object having all the property values as well
            foreach (string pn in content_ui.propertiesUi.Keys)
                jo.Add(pn, jo[nameof(ContentUi.propertiesUi)][pn][nameof(PropertyUI.content)][pn]);
            //and the basic content info
            jo.Merge(ReferencePropertyValueJsonUiConverter.getJsonObject(c));
            //and one last thing, we want to mark this content json as completed
            //as it generated from the whole content
            //the client angular will know than "this is it" it has the whole content and it doesn't need
            //to download it again
            jo.Add(nameof(MysteryUiContentConverter), true);
            jo.WriteTo(writer);
        }
    }
}
