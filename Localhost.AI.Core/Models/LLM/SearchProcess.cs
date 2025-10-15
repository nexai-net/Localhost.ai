using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localhost.AI.Core.Models.LLM
{
    public class SearchFull : EntityBase
    {
        public SearchRequest searchRequest { get; set; } = new SearchRequest();
        public List<Result> results { get; set; } = new List<Result>();
        public List<Action> actions { get; set; } = new List<Action>();
    }

    public class Result
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public int Relevance { get; set; }
    }
}
