using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localhost.AI.Core.Models.LLM
{
    public class Action
    {
        public string typeOfAction { get; set; } = "";
        public DateTime timeOfAction { get; set; } = DateTime.Now;
        public string ActionBy { get; set; } = "system";

    }
}
