using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Democrite.Framework.Core;
using Democrite.Framework.Core.Abstractions;
using Localhost.AI.Core.Framework;
using Localhost.AI.Core.Helpers;
using Localhost.AI.Core.Models;
using Localhost.AI.Core.Models.LLM;
using Microsoft.Extensions.Logging;
using Action = Localhost.AI.Core.Models.LLM.Action;

namespace Localhost.AI.Search
{
    internal class SearchProcessorVGrain : VGrainBase<ISearchProcessorVGrain>, ISearchProcessorVGrain
    {
        private Config _config;
        private SessionManager _session;
        private SearchFull _completion;
        public SearchProcessorVGrain(ILogger<ISearchProcessorVGrain> logger) : base(logger)
        {
            /// Initialization of the vgrain
        }

        public bool Activation()
        {
            return true;
        }
        public async Task<string> DoProcess(string completionId, IExecutionContext Context)
        {
            if (Activation() == false)
            {
                return completionId;
            }

            _config = Core.Helpers.ConfigurationManager.GetFromFile<Config>("config.json");
            _session = new SessionManager(_config);
            _session.LogSave("SEARCHPROC - DoProcess", _config.AppName, "INFO");
            _completion = _session.SearchProcessLoad(completionId);

            if (_completion != null)
            {
                try
                {

                    Console.WriteLine($"ContextProcessorVGrain: Completion loaded with Id {completionId}");
                    DoProcess(completionId);
                    _session.SearchProcessSave(_completion);
                    _session.LogSave($"SEARCHPROC - Search with Id {completionId} saved", _config.AppName, "DONE");
                }
                catch (Exception ex)
                {
                    _session.LogSave($"SEARCHPROC - Error saving cache for research Id {completionId}: {ex.Message}", _config.AppName, "ERROR");
                }
            }
            else
            {
                _session.LogSave($"SEARCHPROC - Search with Id {completionId} not found", _config.AppName, "ERROR");
            }
            return completionId;
        }

        private void DoProcess(string completionId)
        {
            try
            {
                Thread.Sleep(_config.LatencyMs);
                _completion.actions.Add(new Action
                {
                    typeOfAction = "SearchProcessor",
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
