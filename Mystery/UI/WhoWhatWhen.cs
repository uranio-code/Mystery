using Mystery.Content;
using Mystery.Register;
using Mystery.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.UI
{
    public class WhoWhatWhen
    {
        public Guid who { get; set; }
        public ContentReference what { get; set; }

        public DateTime when { get; set; }
    }
}
