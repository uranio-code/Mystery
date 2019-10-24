using Mystery.Content;
using Mystery.Json;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Mystery.Routes;
using System.Linq;

namespace Mystery.TypeScript
{

    public class TypeScriptRouteHandler : IRouteHandler
    {

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new TypeScriptRouteHttpHandler();
        }
    }

    public class TypeScriptRouteHttpHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        private static Type base_class = typeof(TypeScriptClassGenerator<>);

       

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            response.ContentType = "text/plain; charset=utf-8";

            var definitions = new List<string>();

            var generators = new Dictionary<string, ITypeScriptClassGenerator>();
            generators["string"] = null;
            generators["boolean"] = null;
            generators["number"] = null;
            generators["any"] = null;

            var todo = new HashSet<Type>();
            // content types
            todo.AddRange(ContentType.getAllContentTypes());

            //actions
            foreach (var published_action in this.getMystery().AssemblyRegister.getTypesMarkedWith<PublishedAction>())
            {
                var input_type = published_action.getMysteryAttribute<PublishedAction>().input_type;
                todo.Add(input_type);
                var output_type = published_action.getMysteryAttribute<PublishedAction>().output_type;
                todo.Add(output_type);
            }

            todo.Remove(null);

            // at the moment types contains the one which we want, 
            // but they might need more nested types, so we shall generate and add those we need
            // and then go recursive
            var done = new HashSet<Type>();

            //first defintion we need is IContent
            //but we do it our way

            var icontent_generator = new TypeScriptClassGenerator<IContent>();
            generators[icontent_generator.name] = icontent_generator;
            var type_script_def = icontent_generator.getTypeScriptClass();
            //actually we want some more in the ui
            type_script_def = @"
export class IContent {
  guid: string;
  ContentType: string;
  tiny_guid: string;
  data_url: string;
  MysteryUiContentConverter: boolean;
  ReferenceText: string;
  SearchText: string;

  public constructor(init?: Partial<IContent>) {
    Object.assign(this, init);
  }
}

";
            definitions.Add(type_script_def);

            foreach (var ts_name in icontent_generator.other_types.Keys)
            {
                if (generators.ContainsKey(ts_name))//already done
                    continue;
                todo.Add(icontent_generator.other_types[ts_name]);
            }

            todo.Remove(typeof(IContent));
            done.Add(typeof(IContent));

            while (todo.Count> 0) {
                var next_round = new HashSet<Type>();//here I store those type necessary from this cycle
                foreach (var type in todo) {
                    if (done.Contains(type))
                        continue;
                    done.Add(type);
                    //we contruct the generation class
                    var generation_class = base_class.MakeGenericType(new List<Type> { type }.ToArray());
                    var generator = (ITypeScriptClassGenerator)this.getGlobalObject<FastActivator>().createInstance(generation_class);
                    if (generators.ContainsKey(generator.name)) // a different type map to the same already encountered
                        continue;
                    if (generator.name.Contains("[]"))
                        continue; //fake todo, it is an array not a type
                    generators[generator.name] = generator;
                    type_script_def = generator.getTypeScriptClass();
                    definitions.Add(type_script_def);
                    foreach (var ts_name in generator.other_types.Keys) {
                        if (generators.ContainsKey(ts_name))//already done
                            continue;
                        next_round.Add(generator.other_types[ts_name]);
                    }
                }
                todo = new HashSet<Type>(next_round.Except(done));
            }


            string result = string.Join(System.Environment.NewLine, definitions.ToArray());
            
            response.Write(result);
            
        }
    }


}
