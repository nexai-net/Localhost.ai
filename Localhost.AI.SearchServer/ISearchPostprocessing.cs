using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Democrite.Framework.Core.Abstractions;
using Democrite.Framework.Core.Abstractions.Attributes;
using Democrite.Framework.Core.Abstractions.Attributes.MetaData;

namespace Localhost.AI.Search
{
    [VGrainCategory("InboundConnector")]
    //[VGrainMetaData()]
    public interface ISearchPostprocessing : IVGrain
    {
        Task<string> InboundProcessing(string completionId, IExecutionContext Context);

    }
}
