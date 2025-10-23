using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localhost.AI.Core.Models.Symbolic
{
    public class CvRewriteRequest
    {
        public string id { get; set; }
        public string prompt { get; set; }
        public string model { get; set; }
    }
}
