

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

    internal class ContextProcessorVGrain : VGrainBase<IContextProcessorVGrain>, IContextProcessorVGrain
    {
        private Config _config;
        private SessionManager _session;
        private Completion _completion;

        public bool ActivateMethod()
        {
            return true;
        }

        public ContextProcessorVGrain(ILogger<IContextProcessorVGrain> logger) : base(logger)
        {
            /// Initialization of the vgrain
        }

        public async Task<string> DoProcess(string completionId, IExecutionContext Context)
        {
            if (ActivateMethod()==false)
            {
                return completionId;
            }

            _config = ConfigurationManager.GetFromFile<Config>("config.json");
            _session = new SessionManager(_config);
            _session.LogSave("CONTEXTPROC - DoProcess", _config.AppName, "INFO");
            _completion = _session.CompletionLoad(completionId);

            if (_completion != null)
            {
                try
                {
                    Console.WriteLine($"DialogInboundVGrain: Completion loaded with Id {completionId}");
                    DoProcess();
                    _session.CompletionSave(_completion);
                    _session.LogSave($"CONTEXTPROC - Completion with Id {completionId} saved", _config.AppName, "INFO");
                }
                catch (Exception ex)
                {
                    _session.LogSave($"CONTEXTPROC - Error saving cache for completion Id {completionId}: {ex.Message}", _config.AppName, "ERROR");
                } 
            }
            else
            {
                _session.LogSave($"Completion with Id {completionId} not found", _config.AppName, "ERROR");
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
                    typeOfAction = "ContextProcessor",
                    timeOfAction = DateTime.Now,
                    ActionBy = _config.AppName
                });
            }
            catch (Exception ex)
            {
                _session.LogSave($"Error in DoProcess: {ex.Message}", _config.AppName, "ERROR");
                throw ex;
            }
        }
    }
}
