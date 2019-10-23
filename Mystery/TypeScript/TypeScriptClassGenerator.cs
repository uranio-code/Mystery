using Mystery.Content;
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.UI;

namespace Mystery.TypeScript
{

    public interface ITypeScriptClassGenerator {
        string getTypeScriptClass();
        string name { get; }
        Dictionary<string, Type> other_types { get; }
    }


    public class TypeScriptClassGenerator<T>: ITypeScriptClassGenerator
    {

        private static Dictionary<Type, string> type_map = new Dictionary<Type, string>();

        private Type type = typeof(T);

        private bool isContentType() {
            return type.getMysteryAttribute<ContentType>() != null;
        }

        public string name { get {
                return getTypeScriptType(type);
            } }

        public Dictionary<string, Type> other_types { get; private set; } = new Dictionary<string, Type>();

        static TypeScriptClassGenerator()
        {
            type_map[typeof(string)] = "string";
            type_map[typeof(bool)] = "boolean";
            type_map[typeof(double)] = "number";
            type_map[typeof(int)] = "number";
            type_map[typeof(long)] = "number";
            type_map[typeof(float)] = "number";
            type_map[typeof(Guid)] = "string";
            type_map[typeof(Object)] = "any";

        }

        /// <summary>Finds the type of the element of a type. Returns null if this type does not enumerate.</summary>
        /// <param name="type">The type to check.</param>
        /// <returns>The element type, if found; otherwise, <see langword="null"/>.</returns>
        private static Type FindElementType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            // type is IEnumerable<T>;
            if (ImplIEnumT(type))
                return type.GetGenericArguments().First();

            // type implements/extends IEnumerable<T>;
            var enumType = type.GetInterfaces().Where(ImplIEnumT).Select(t => t.GetGenericArguments().First()).FirstOrDefault();
            if (enumType != null)
                return enumType;

            // type is IEnumerable
            if (IsIEnum(type) || type.GetInterfaces().Any(IsIEnum))
                return typeof(object);

            return null;

            bool IsIEnum(Type t) => t == typeof(System.Collections.IEnumerable);
            bool ImplIEnumT(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private string getTypeScriptType(Type type)
        {
            string result = null;
            if (type_map.ContainsKey(type))
            {
                return type_map[type];
            }

            //reference will be converted in the reffered type
            if (typeof(IReferenceProperty).IsAssignableFrom(type))
            {
                var is_multi = ReferenceEquals(type.GetGenericTypeDefinition(), typeof(MultiContentReference<>));
                var target_type = type.GenericTypeArguments.FirstOrDefault().getMysteryAttribute<ContentType>().name;
                result = target_type + (is_multi ? "[]" : "");
                type_map[type] = result;
                return result;
            }

            //content type will be converted to their names
            result = type.getMysteryAttribute<ContentType>()?.name;
            if (!string.IsNullOrWhiteSpace(result))
            {
                type_map[type] = result;
                return result;
            }

            //dict type we can't know
            if (typeof(IDictionary).IsAssignableFrom(type)) {
                result = "any";
                type_map[type] = result;
                return result;
            }

            //arrays enum etc..
            var element_type = FindElementType(type);
            if (element_type != null)
            {
                result = getTypeScriptType(element_type) + "[]";
                type_map[type] = result;
                return result;
            }


            //at this point it should be the type it self
            result = type.Name;
            type_map[type] = result;
            return result;

        }
        public string getTypeScriptClass()
        {
            var type = typeof(T);
            //content type go down different
            if (isContentType())
            {
                return getTypeScriptForContentType();
            }

            if (type.IsEnum) {
                return getTypeScriptForEnum();
            }

            StringBuilder builder = new StringBuilder();
            builder.Append($@"
export class {getTypeScriptType(type)}
");
            builder.Append(@"{
");
            foreach (var p in type.GetProperties())
            {
                other_types[getTypeScriptType(p.PropertyType)] = p.PropertyType;
                builder.Append($"{p.Name}: {getTypeScriptType(p.PropertyType)};" + System.Environment.NewLine);
            }
            builder.Append(@"
public constructor(init?: Partial<" + getTypeScriptType(type) + @">) {
Object.assign(this, init);
}" + System.Environment.NewLine);
            builder.Append(@"}
");

            var result = builder.ToString();
            return result;
        }

        private string getTypeScriptForEnum()
        {
            var type = typeof(T);

            StringBuilder builder = new StringBuilder();
            builder.Append($@"
export enum {getTypeScriptType(type)}
");
            builder.Append(@"{
");


            foreach (var element in Enum.GetNames(type)) {
                builder.Append(element+"='"+ element+ "', " + System.Environment.NewLine);
            }

            
builder.Append(@"}
");

            var result = builder.ToString();
            return result;
        }


        private string getTypeScriptForContentType()
        {
            var type = typeof(T);

            StringBuilder builder = new StringBuilder();
            builder.Append($@"
export class {getTypeScriptType(type)} extends IContent
");
            builder.Append(@"{
");

            builder.Append("ContentType: string;" + System.Environment.NewLine);
            builder.Append("tiny_guid: string;" + System.Environment.NewLine);
            builder.Append("data_url: string;" + System.Environment.NewLine);
            builder.Append("MysteryUiContentConverter: string;" + System.Environment.NewLine);

            foreach (var p in type.getMysteryPropertyAttributes<PropertyView>())
            {
                other_types[getTypeScriptType(p.used_in.PropertyType)] = p.used_in.PropertyType;
                builder.Append($"{p.used_in.getMysteryAttribute<ContentProperty>().name}: {getTypeScriptType(p.used_in.PropertyType)};" + System.Environment.NewLine);
            }
            builder.Append(@"
public constructor(init?: Partial<" + getTypeScriptType(type) + @">) {
super(init);
Object.assign(this, init);
}" + System.Environment.NewLine);
            builder.Append(@"}
");

            var result = builder.ToString();
            return result;
        }

    }
}
