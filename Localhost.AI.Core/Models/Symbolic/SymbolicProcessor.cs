using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localhost.AI.Core.Models.Symbolic
{
    public class SymbolicProcessor : EntityBase
    {
        public string Name { get; set; } = string.Empty;
        public List<string> InboundShould { get; set; } = new List<string>();
        public List<string> InboundShouldNot { get; set; } = new List<string>();
        public List<string> InboundMust { get; set; } = new List<string>();
        public List<string> InboundMustNot { get; set; } = new List<string>();
        public List<string> OutboundShould { get; set; } = new List<string>();
        public List<string> OutboundShouldNot { get; set; } = new List<string>();
        public List<string> OutboundMust { get; set; } = new List<string>();
        public List<string> OutboundMustNot { get; set; } = new List<string>();
        public string SystemPrompt { get; set; } = string.Empty;
    }
}
