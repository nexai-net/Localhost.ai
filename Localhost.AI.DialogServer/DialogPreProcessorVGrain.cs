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
    using Localhost.AI.Core.Models.Corpus;
    using Action = Core.Models.LLM.Action;

    internal class DialogPreProcessorVGrain : VGrainBase<IDialogPreProcessorVGrain>, IDialogPreProcessorVGrain
    {
        private Config _config;
        private SessionManager _session;
        private Completion _completion;

        public DialogPreProcessorVGrain(ILogger<IDialogPreProcessorVGrain> logger) : base(logger)
        {
            /// Initialization of the vgrain
        }

        public bool ActivateMethod()
        {
            return true;
        }

        public async Task<string> PreProcessing(string completionId, IExecutionContext Context)
        {
            if(ActivateMethod() == false)
            {
                return completionId;
            }

            _config = ConfigurationManager.GetFromFile<Config>("config.json");
            _session = new SessionManager(_config);
            _session.LogSave("PREPROC - DialogPreProcessor ", _config.AppName, "INFO");
            _completion = _session.CompletionLoad(completionId);


            if (_completion != null)
            {
                try
                {
                    Console.WriteLine($"DialogInboundVGrain: Completion loaded with Id {completionId}");
                    DoProcess();
                    _session.CompletionSave(_completion);
                    _session.LogSave($"PREPROC - Completion with Id {completionId} saved", _config.AppName, "DONE");
                }
                catch (Exception ex)
                {
                    _session.LogSave($"PREPROC - Error saving cache for completion Id {completionId}: {ex.Message}", _config.AppName, "ERROR");
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

                List<Entity> entities = _session.EntitySearchByName("");
                List<EntityItem> entityItems = new List<EntityItem>();
                foreach (Entity e in entities)
                {
                    foreach (string alias in e.AlternativeNames)
                    {
                        entityItems.Add(new EntityItem { id = e.Id, criteria = alias });
                    }
                }

                List<Entity> contextEntities = new List<Entity>();

                foreach (EntityItem ei in entityItems)
                {
                    if (_completion.prompt.Contains(ei.criteria, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Write("Prompt {_completion.prompt} contains criteria {ei.criteria} or not! ");
                        contextEntities.Add(_session.EntityLoad(ei.id));
                    }
                }

                foreach (Entity ce in contextEntities)
                {
                    _completion.prompt = $"\nSachant que pour {ce.Name} on sait que : {ce.LongDescription}. \n" + _completion.prompt;
                }
                if (_completion.request.messages != null)
                {
                    foreach (Message m in _completion.request.messages)
                    {
                        if (m.role == "system")
                        {
                            m.content = "Context :" + _completion.prompt;
                        }
                    }
                }

                _completion.actions.Add(new Action
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
