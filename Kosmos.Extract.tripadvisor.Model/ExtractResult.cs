using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kosmos.Extract.tripadvisor.Model
{
    public class ExtractResult
    {
        public string HashCode { get; set; }
        public string Result { get; set; }
        public DateTime? ExtractData { get; set; }
        public string Domain { get; set; }
        public string Url { get; set; }
        public int? Depth { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (ReferenceEquals(this, null))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            var o = obj as ExtractResult;
            if (HashCode.Equals(o.HashCode))
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.GetHashCode();
        }
    }
}
