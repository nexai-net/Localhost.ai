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

    internal class DialogCacheProcessorVGrain : VGrainBase<IDialogCacheProcessorVGrain>, IDialogCacheProcessorVGrain
    {
        private Config _config;
        private SessionManager _session;
        private Completion _completion;
        public DialogCacheProcessorVGrain(ILogger<IDialogCacheProcessorVGrain> logger) : base(logger)
        {
            /// Initialization of the vgrain
        }

        public bool Activation()
        {
            return true;
        }
        public async Task<string> DoProcess(string completionId, IExecutionContext Context)
        {
            if(Activation() == false)
            {
                return completionId;
            }   

            _config = ConfigurationManager.GetFromFile<Config>("config.json");
            _session = new SessionManager(_config);
            _session.LogSave("CACHEPROC - DoProcess", _config.AppName, "INFO");
            _completion = _session.CompletionLoad(completionId);

            if (_completion != null)
            {
                try
                {

                    Console.WriteLine($"ContextProcessorVGrain: Completion loaded with Id {completionId}");
                    DoProcess(completionId);
                    _session.CompletionSave(_completion);
                    _session.LogSave($"CACHEPROC - Completion with Id {completionId} saved", _config.AppName, "DONE");
                }
                catch (Exception ex)
                {
                    _session.LogSave($"CACHEPROC - Error saving cache for completion Id {completionId}: {ex.Message}", _config.AppName, "ERROR");
                } 
            }
            else
            {
                _session.LogSave($"CACHEPROC - Completion with Id {completionId} not found", _config.AppName, "ERROR");
            }
            return completionId;
        }

        private void DoProcess(string completionId)
        {
            try
            {
                Thread.Sleep(_config.LatencyMs);
                List<Cache> caches = _session.CacheEntriesSearch(_completion.prompt);
                if (caches.Count > 0)
                {
                    // for simplicity, we take the first cache entry found
                    _completion.isCached = true;
                    _completion.cachedCompletion = caches[0].completion;
                    _session.LogSave($"CACHEPROC - Cache hit for completion Id {completionId}", _config.AppName, "DONE");
                }
                else
                {
                    _completion.isCached = false;
                    _completion.cachedCompletion = "";
                    _session.LogSave($"CACHEPROC - No cache found for completion Id {completionId}", _config.AppName, "DONE");
                }
                _completion.actions.Add(new Action
                {
                    typeOfAction = "CacheProcessor",
                    timeOfAction = DateTime.Now,
                    ActionBy = _config.AppName
                });
            }
            catch (Exception ex)
            {
                _session.LogSave($"Error processing cache for completion Id {completionId}: {ex.Message}", "DIALOGSERVER-CACHE-PROCESSOR", "Error");
                throw ex;
            }
        }
    }
}
