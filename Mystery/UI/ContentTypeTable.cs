using Mystery.Content;
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mystery.UI
{

    public class ContentColumInfo {
        public string name { get; set; }
        public string label { get; set; }
        public string cell_template_url { get; set; }
    }

    public interface IContentTable {
        IEnumerable<ContentColumInfo> getColumns();
    }
    /// <summary>
    /// base class used for create tables of contents, it will return those content property which are marked
    /// as PropertyColumn
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseContentTypeTable<T> : IContentTable where T:IContent
    {
        public IEnumerable<ContentColumInfo> getColumns()
        {
            var columns = typeof(T).getMysteryPropertyAttributes<PropertyColumn>();
            var result = new List<ContentColumInfo>();
            foreach (var col in columns) {
                //we make sure they are content property too
                var cp = col.used_in.getMysteryAttribute<ContentProperty>();
                if (cp == null)
                    continue;
                result.Add(new ContentColumInfo()
                {
                    name = cp.name, label = cp.label, cell_template_url = col.template_url
                });
            }
            return result;
        }
    }

    public class ContentTypeTable : MysteryDefaultClassAttribute
    {

        public Type implementing_type { get; set; }

        private IContentTable instance;

        public override void setUp()
        {
            if (implementing_type == null)
                implementing_type =  typeof(BaseContentTypeTable<>).MakeGenericType(this.used_in);

            if (!typeof(IContentTable).IsAssignableFrom(implementing_type))
            {
                throw new Exception(implementing_type.FullName + " must implement " + typeof(IContentTable).FullName);
            }
            instance = (IContentTable)Activator.CreateInstance(implementing_type);
        }
        public IEnumerable<ContentColumInfo> getColumns() {
            return instance.getColumns();
        }
    }
}
