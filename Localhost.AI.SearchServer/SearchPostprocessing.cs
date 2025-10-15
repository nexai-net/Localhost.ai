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
using ConfigurationManager = Localhost.AI.Core.Helpers.ConfigurationManager;

namespace Localhost.AI.Search
{
    internal class SearchPostprocessing : VGrainBase<ISearchPostprocessing>, ISearchPostprocessing
    {

        private Config _config;
        private SessionManager _session;
        private SearchFull _completion;

        public SearchPostprocessing(ILogger<ISearchPostprocessing> logger) : base(logger)
        {
            /// Initialization of the vgrain
        }

        public bool Activation()
        {
            return true;
        }

        public async Task<string> InboundProcessing(string searchprocessId, IExecutionContext Context)
        {
            if (Activation() == false)
            {
                return searchprocessId;
            }
            _config = ConfigurationManager.GetFromFile<Config>("config.json");
            _session = new SessionManager(_config);
            _session.LogSave("SEARCHPOSTPROC - InboundProcessing", _config.AppName, "INFO");
            _completion = _session.SearchProcessLoad(searchprocessId);

            if (_completion != null)
            {
                try
                {
                    Console.WriteLine($"DialogInboundVGrain: Completion loaded with Id {searchprocessId}");
                    DoProcess();
                    _session.SearchProcessSave(_completion);
                    _session.LogSave($"SEARCHPOSTPROC - Completion with Id {searchprocessId} saved", _config.AppName, "DONE");
                }
                catch (Exception ex)
                {
                    _session.LogSave($"SEARCHPOSTPROC - Error saving cache for completion Id {searchprocessId}: {ex.Message}", _config.AppName, "ERROR");
                }
            }
            else
            {
                _session.LogSave($"SEARCHPOSTPROC - Completion with Id {searchprocessId} not found", _config.AppName, "ERROR");
            }
            return searchprocessId;
        }

        private void DoProcess()
        {
            try
            {
                Thread.Sleep(_config.LatencyMs);
                _completion.actions.Add(new Action
                {
                    typeOfAction = "PostProcessor",
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
