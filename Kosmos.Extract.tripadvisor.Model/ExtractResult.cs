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
    }
}
