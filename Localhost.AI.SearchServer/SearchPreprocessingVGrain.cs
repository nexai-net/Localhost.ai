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
    internal class SearchPreprocessingVGrain : VGrainBase<ISearchPreprocessingVGrain>, ISearchPreprocessingVGrain
    {
        private Config _config;
        private SessionManager _session;
        private SearchFull _searchprocessId;

        public bool ActivateMethod()
        {
            return true;
        }

        public SearchPreprocessingVGrain(ILogger<ISearchPreprocessingVGrain> logger) : base(logger)
        {
            /// Initialization of the vgrain
        }

        public async Task<string> DoProcess(string completionId, IExecutionContext Context)
        {
            if (ActivateMethod() == false)
            {
                return completionId;
            }

            _config = Core.Helpers.ConfigurationManager.GetFromFile<Config>("config.json");
            _session = new SessionManager(_config);
            _session.LogSave("SEARCHPREPROC - DoProcess", _config.AppName, "INFO");
            _searchprocessId = _session.SearchProcessLoad(completionId);

            if (_searchprocessId != null)
            {
                try
                {
                    Console.WriteLine($"SEARCHPREPROC - SearchPreprocessing: Research loaded with Id {completionId}");
                    DoProcess();
                    _session.SearchProcessSave(_searchprocessId);
                    _session.LogSave($"SEARCHPREPROC - Search with Id {completionId} saved", _config.AppName, "INFO");
                }
                catch (Exception ex)
                {
                    _session.LogSave($"SEARCHPREPROC - Error saving cache for search Id {completionId}: {ex.Message}", _config.AppName, "ERROR");
                }
            }
            else
            {
                _session.LogSave($"SEARCHPREPROC - Search with Id {completionId} not found", _config.AppName, "ERROR");
            }
            return completionId;
        }

        private void DoProcess()
        {
            try
            {
                Thread.Sleep(_config.LatencyMs);
                _searchprocessId.actions.Add(new Action
                {
                    typeOfAction = "PreProcessor",
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
