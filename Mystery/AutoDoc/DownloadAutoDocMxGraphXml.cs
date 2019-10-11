using com.mxgraph;
using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.Register;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.AutoDoc
{

    public class DownloadAutoDocMxGcraphXmlOutput {
        public string xml { get; set; }
    }


    [PublishedAction(input_type: null, output_type: typeof(DownloadAutoDocMxGcraphXmlOutput), url = nameof(DownloadAutoDocMxGraphXml))]
    public class DownloadAutoDocMxGraphXml : BaseMysteryAction<DownloadAutoDocMxGcraphXmlOutput>
    {
        protected override ActionResult<DownloadAutoDocMxGcraphXmlOutput> ActionImplemetation()
        {
            mxGraph graph = new mxGraph();
            

            var table = new mxCell(new { name = "table" }, new mxGeometry(0, 0, 200, 28), "table");
            
            var column = new mxCell(new {name="Column"}, new mxGeometry(0, 0, 0, 26),"");


            // Adds vertices and edges to the graph.
            graph.Model.BeginUpdate();

            try
            {
                var all_content_type_names = ContentType.getAllContentTypeNames();
                var content_type_map = new Dictionary<string, Type>();
                var content_type_cell_map = new Dictionary<string, mxCell>();
                foreach (var content_type_name in all_content_type_names) {
                    content_type_map[content_type_name] = ContentType.getType(content_type_name);

                    var cell = new mxCell(content_type_name, new mxGeometry(0, 0, 200, 28), "table");
                    content_type_cell_map[content_type_name] = cell;
                    Object parent = graph.GetDefaultParent();
                    graph.Model. Add(parent, cell, graph.Model.GetChildCount(parent));
                    cell.Vertex = true;
                    cell.Geometry = new mxGeometry(0, 0, 200, 80);
                    break;
                }
                foreach (var content_type_name in all_content_type_names)
                {
                    Object parent = content_type_cell_map[content_type_name];
                    foreach (var content_property in content_type_map[content_type_name].getMysteryPropertyAttributes<ContentProperty>())
                    {
                        var cell = new mxCell(content_property.name, new mxGeometry(0, 28, 200, 26),"");
                        cell.Vertex = true;
                        graph.Model.Add(parent, cell, graph.Model.GetChildCount(parent));

                        //reference?
                        if (typeof(IReferenceProperty).IsAssignableFrom(content_property.used_in.PropertyType))
                        {
                            var reffered_content_type_name = content_property.used_in.PropertyType.
                                GenericTypeArguments.FirstOrDefault().getMysteryAttribute<ContentType>().name;
                            
                        }
                    }
                    break;
                    
                }

                //Object v1 = graph.InsertVertex(parent, null, "a," + System.Environment.NewLine + "something else" + System.Environment.NewLine + "something else", 20, 20, 80, 30);
                //Object v2 = graph.InsertVertex(parent, null, "b", 200, 150, 80, 30);
                //Object v3 = graph.InsertVertex(parent, null, "c", 0, 0, 80, 30);
                //Object e1 = graph.InsertEdge(parent, null, "ab", v1, v2);
                //Object e2 = graph.InsertEdge(parent, null, "ac", v1, v3);
                //Object e3 = graph.InsertEdge(parent, null, "bc", v2, v3);
            }
            finally
            {
                graph.Model.EndUpdate();
            }

            // Encodes the model into XML and passes the resulting XML string into a page
            // variable, so it can be read when the page is rendered on the server. Note
            // that the page instance is destroyed after the page was sent to the client.
            mxCodec codec = new mxCodec();
            var result = mxUtils.GetXml(codec.Encode(graph.Model));
            return new DownloadAutoDocMxGcraphXmlOutput() { xml = result };

        }

        protected override bool AuthorizeImplementation()
        {
            return user.account_type == Users.UserType.admin;
        }
    }
}
