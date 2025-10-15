using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localhost.AI.Core.Models.Symbolic
{
    public class SymbolicDecoder : EntityBase
    {
        public string Name { get; set; } = string.Empty;

        public List<string> Should { get; set; } = new List<string>();
        public List<string> ShouldNot { get; set; } = new List<string>();
        public List<string> Must { get; set; } = new List<string>();
        public List<string> MustNot { get; set; } = new List<string>();
        public string UserPrompt { get; set; } = string.Empty;
        public string PostCompletion { get; set; } = string.Empty;
        public string PreCompletion { get; set; } = string.Empty;
        public string SystemPrompt { get; set; } = string.Empty;

    }
}
