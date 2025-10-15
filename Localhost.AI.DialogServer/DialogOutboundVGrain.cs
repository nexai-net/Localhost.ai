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

    internal class DialogOutboundVGrain : VGrainBase<IDialogOutboundVGrain>, IDialogOutboundVGrain
    {
        private Config _config;
        private SessionManager _session;
        private Completion _completion;

        public DialogOutboundVGrain(ILogger<IDialogOutboundVGrain> logger) : base(logger)
        {
            /// Initialization of the vgrain
        }

        public bool Activation()
        {
            return true;
        }

        public async Task<string> InboundProcessing(string completionId, IExecutionContext Context)
        {
            if(Activation() == false)
            {
                return completionId;
            }

            _config = ConfigurationManager.GetFromFile<Config>("config.json");
            _session = new SessionManager(_config);
            _session.LogSave("DialogOutbound ", _config.AppName, "Info");
            _completion = _session.CompletionLoad(completionId);

            if (_completion != null)
            {                
                try
                {
                    Console.WriteLine($"DialogOutboundVGrain: Completion loaded with Id {completionId}");
                    DoProcess();
                    _session.CompletionSave(_completion);
                }

                catch (Exception ex)
                {
                    _session.LogSave($"Error saving cache for completion Id {completionId}: {ex.Message}", "DIALOGSERVER-AGENT-OUTBOUND", "Error");
                }

            }
            else
            {
                _session.LogSave($"Completion with Id {completionId} not found", _config.AppName, "Error");
            }
            return completionId;
        }

        private void DoProcess()
        {
            try
            {
                Thread.Sleep(_config.LatencyMs);
                _completion.Status = "Completed";
                _completion.actions.Add(new Action
                {
                    typeOfAction = "OutboundProcessor",
                    timeOfAction = DateTime.Now,
                    ActionBy = _config.AppName
                });

                Cache c = new Cache();
                c.completion = _completion.response.choices[0].message.content;
                c.prompt = _completion.request.messages.Where(m => m.role == "user").FirstOrDefault().content;
                c.language = _completion.language;
                c.model = _completion.request.model;
                c.Date = DateTime.Now;
                c.MachineName = Environment.MachineName;
                c.UserName = Environment.UserName;
                c.Comment = "cache done for completin of " + c.prompt;
                _session.CacheSave(c);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
