using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localhost.AI.Core.Models.Symbolic
{
    public class SymbolicEncoder : EntityBase
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Regex { get; set; } = new List<string>();
        public List<string> Terms { get; set; } = new List<string>();
        public List<string> Should { get; set; } = new List<string>();
        public List<string> ShouldNot { get; set; } = new List<string>();
        public List<string> Must { get; set; } = new List<string>();
        public List<string> MustNot { get; set; } = new List<string>();
        public string SystemPrompt { get; set; } = string.Empty;
    }
}
