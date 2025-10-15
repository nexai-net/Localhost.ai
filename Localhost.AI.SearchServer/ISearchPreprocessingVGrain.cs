using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Democrite.Framework.Core.Abstractions;
using Democrite.Framework.Core.Abstractions.Attributes;
using Democrite.Framework.Core.Abstractions.Attributes.MetaData;
using Localhost.AI.Core.Models.LLM;

namespace Localhost.AI.Search
{
    [VGrainCategory("ContextProcessor")]
    //[VGrainMetaData()]
    public interface ISearchPreprocessingVGrain : IVGrain
    {
        Task<string> DoProcess(string completionId, IExecutionContext Context);

    }
}
