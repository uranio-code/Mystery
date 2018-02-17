using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Files
{
    public class MysteryFile
    {
        public Guid guid { get; set; }

        public string filename { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return string.IsNullOrEmpty(filename) && guid == Guid.Empty;
            if (!(obj is MysteryFile))
                return false;
            var other = (MysteryFile)obj;
            return other.filename == filename && other.guid == guid;
        }
        public override int GetHashCode()
        {
            return guid.GetHashCode()+ (string.IsNullOrEmpty(filename)? 0:filename.GetHashCode());
        }
    }
}
