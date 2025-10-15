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

    internal class DialogProcessorVGrain : VGrainBase<IDialogProcessorVGrain>, IDialogProcessorVGrain
    {
        private Config _config;
        private SessionManager _session;
        private Completion _completion;

        public DialogProcessorVGrain(ILogger<IDialogProcessorVGrain> logger) : base(logger)
        {
            /// Initialization of the vgrain
        }

        public bool Activation()
        {
            return true;
        }

        public async Task<string> Processing(string completionId, IExecutionContext Context)
        {
            if(Activation() == false)
            {
                return completionId;
            }
            
            _config = ConfigurationManager.GetFromFile<Config>("config.json");
            _session = new SessionManager(_config);
            _session.LogSave("DIALOGPROC - DialogProcessor ", _config.AppName, "INFO");
            _completion = _session.CompletionLoad(completionId);

            if (_completion != null)
            {
               Console.WriteLine($"DialogInboundVGrain: Completion loaded with Id {completionId}");
                await DoProcessAsync();
                _session.CompletionSave(_completion);
                _session.LogSave($"DIAGPROC - Completion with Id {completionId} saved", _config.AppName, "DONE");
            }
            else
            {
                _session.LogSave($"DIAGPROC - Completion with Id {completionId} not found", _config.AppName, "ERROR");
            }            
            return completionId;
        }


        private async Task DoProcessAsync()
        {
            try
            {
                string completionId = _completion.Id;
                Thread.Sleep(_config.LatencyMs);
                // Add an action to track the main processing step
                _completion.actions.Add(new Action
                {
                    typeOfAction = "MainProcessor",
                    timeOfAction = DateTime.Now,
                    ActionBy = _config.AppName
                });

                // Check if we have a cached response for this completion
                if (_completion.isCached)
                {
                    // Create a choice object with the cached completion
                    Choice choice = new Choice
                    {
                        index = 0,
                        message = new Message
                        {
                            role = "assistant",
                            content = _completion.cachedCompletion
                        },
                        finish_reason = "stop"
                    };

                    // Set up the response with the cached choice
                    _completion.response.choices = new List<Choice>();
                    _completion.response.choices.Add(choice);
                }
                else
                {
                    // Call the Ollama API to get a fresh response
                    Response r = await OllamaManager.CallOllamaAsync(_config.OllamaUrl, _completion.request);
                    _completion.response = r;
                }

                // Save the updated completion back to the repository
                _session.CompletionSave(_completion);

                // Log successful completion of processing
               
            }
            catch (Exception ex)
            {
                 throw ex;
            }
        }
    }
}
