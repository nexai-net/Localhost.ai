using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localhost.AI.Core.Models.LLM
{
    public class SearchRequest
    {
        public string Query { get; set; }
        public List<string> Tags { get; set; } = new List<string>();


    }
}
