namespace Localhost.AI.Dialog
{
    using Democrite.Framework.Core;
    using Democrite.Framework.Core.Abstractions;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Localhost.AI.Core.Models.LLM;
    using Localhost.AI.Core.Models;
    using Localhost.AI.Core.Helpers;
    using Localhost.AI.Core.Framework;
    using Action = Core.Models.LLM.Action;

    internal class EmotionnalOutboundVGrain : VGrainBase<IEmotionnalOutboundVGrain>, IEmotionnalOutboundVGrain
    {
        private Config _config;
        private SessionManager _session;
        private Completion _completion;

        public EmotionnalOutboundVGrain(ILogger<IEmotionnalOutboundVGrain> logger) : base(logger)
        {
            /// Initialization of the vgrain
        }

        public bool Activation()
        {
            return true;
        }

        public async Task<string> OutbountEmotionnalProcessing(string completionId, IExecutionContext Context)
        {

            if (Activation() == false)
            {
                return completionId;
            }

            _config = ConfigurationManager.GetFromFile<Config>("config.json");
            _session = new SessionManager(_config);
            _session.LogSave("OUTBOUNDPROC - EmotionnalInboundProcessor", _config.AppName, "INFO");
            _completion = _session.CompletionLoad(completionId);

            if (_completion != null)
            {
                Console.WriteLine($"DialogInboundVGrain: Completion loaded with Id {completionId}");                
                DoProcess();
                _session.CompletionSave(_completion);                
                _session.LogSave($"OUTBOUNDPROC - Completion with Id {completionId} saved", _config.AppName, "DONE");
            }
            else
            {
                _session.LogSave($"OUTBOUNDPROC - Completion with Id {completionId} not found", _config.AppName, "ERROR");
            }
            return completionId;
        }

        private void DoProcess()
        {
            try
            {
                Thread.Sleep(_config.LatencyMs);
                _completion.actions.Add(new Action
                {
                    typeOfAction = "EmotionnalOutboundProcessor",
                    timeOfAction = DateTime.Now,
                    ActionBy = _config.AppName
                });
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
    }
}
