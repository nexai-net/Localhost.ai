using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localhost.AI.Admin.Models
{
    public class pfff
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string StoryTitle { get; set; }
        public string Sentence1 { get; set; }
        public string Sentence2 { get; set; }
        public string Sentence3 { get; set; }
        public string Sentence4 { get; set; }
        public string Sentence5 { get; set; }
        public string Completion { get; set; }
    }
}
