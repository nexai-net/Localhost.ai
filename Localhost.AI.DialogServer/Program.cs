namespace Localhost.AI.Dialog
{
    using Democrite.Framework.Builders;
    using Democrite.Framework.Core.Abstractions;
    using Localhost.AI.Core.Framework;
    using Localhost.AI.Core.Helpers;
    using Localhost.AI.Core.Models;
    using Localhost.AI.Core.Models.LLM;
    using Localhost.AI.Core.Models.Corpus;
    using Localhost.AI.Core.Models.Symbolic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    internal class Program
    {
        static private SessionManager _session;
        static private Config _config;

        static void Main(string[] args)
        {
            
            Config config = ConfigurationManager.GetFromFile<Config>("config.json");
            Console.WriteLine($"Starting {config.AppName} - {config.AppDescription}");
            Console.WriteLine($"Documentation: {config.AppDocumentationUrl}");
            Console.WriteLine("");
            string AppName = config.AppName ?? "APPx";

            _config = config;
            _session = new SessionManager(_config);
            _session.LogSave("Application started", AppName, "Info");

            var builder = WebApplication.CreateBuilder(args); 

            builder.Services.AddSwaggerGen(s => s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Nexai.net - Localhost.ai", Version = "v1" })).AddEndpointsApiExplorer();

            var DialogProcessSeq = Sequence.Build("JFO", fixUid: new Guid("D1F7C7EB-77F7-488A-91D7-77E4D5D16448"), metadataBuilder: m =>
                                         {
                                             m.Description("Dialog Sequence")
                                              .AddTags("chatbot", "nexai");
                                         })
                                         .RequiredInput<string>()
                                         .Use<IDialogInboundVGrain>().Configure("JFO-TABLE").Call((g, i, ctx) => g.InboundProcessing(i, ctx)).Return
                                         .Use<IContextProcessorVGrain>().Configure("JFO-TABLE").Call((g, i, ctx) => g.DoProcess(i, ctx)).Return
                                         .Use<IEmotionnalInboundProcessorVGrain>().Configure("JFO-TABLE").Call((g, i, ctx) => g.InboundEmotionnalProcessing(i, ctx)).Return
                                         .Use<IDialogCacheProcessorVGrain>().Configure("JFO-TABLE").Call((g, i, ctx) => g.DoProcess(i, ctx)).Return
                                         .Use<IDialogPreProcessorVGrain>().Configure("JFO-TABLE").Call((g, i, ctx) => g.PreProcessing(i, ctx)).Return
                                         .Use<IDialogProcessorVGrain>().Configure("JFO-TABLE").Call((g, i, ctx) => g.Processing(i, ctx)).Return
                                         .Use<IDialogPostProcessorVGrain>().Configure("JFO-TABLE").Call((g, i, ctx) => g.PostProcessing(i, ctx)).Return
                                         .Use<IEmotionnalOutboundVGrain>().Configure("JFO-TABLE").Call((g, i, ctx) => g.OutbountEmotionnalProcessing(i, ctx)).Return
                                         .Use<IDialogOutboundVGrain>().Configure("JFO-TABLE").Call((g, i, ctx) => g.InboundProcessing(i, ctx)).Return
                                         .Build();

            var ResearchSeq = Sequence.Build("JFO", fixUid: new Guid("D1F7C7EB-77F7-488A-91D7-77E4D5D16450"), metadataBuilder: m =>
            {
                m.Description("Research Sequence")
                 .AddTags("research", "nexai");
            })
                                         .RequiredInput<string>()
                                         .Use<IPreSearchVGrain>().Configure("JFO-TABLE2").Call((g, i, ctx) => g.DoPreProcess(i, ctx)).Return
                                         .Build();

            builder.Host.UseDemocriteNode(b =>
            {
                b.WizardConfig()
                .NoCluster()
                .ConfigureLogging(c => c.AddConsole())
                .AddInMemoryDefinitionProvider(d => d.SetupSequences(DialogProcessSeq));
                b.ManualyAdvancedConfig().UseDashboard(options =>
                {
                    options.HostSelf = true;  // host it inside the silo
                    options.Port = 9090;      // <-- change the default 8080 here
                });
            }                                                );
            var app = builder.Build();
            app.UseSwagger();

            app.MapPost("v1/chat/completions", async (Request d, [FromServices] IDemocriteExecutionHandler handler) =>
            {
                Console.WriteLine("Received request for /v1/chat/completions");
                //d.model = "mistral-small3.1";
                bool hassystem = d.messages.Exists(m => m.role == "system");
                Console.WriteLine($"System prompt present: {hassystem}");

                if (!hassystem)
                {
                    Message systemMessage = new Message
                    {
                        role = "system",
                        content = "be short"
                    };
                    d.messages.Insert(0, systemMessage);
                }

                Completion c = LanguageModelManager.InitializeCompletion();
                c.language = _config.Language;
                c.request = d;
                var id = _session.CompletionSave(c);
                var result = await handler.Sequence<string>(DialogProcessSeq.Uid)
                                       .SetInput(id)
                                       .RunAsync<string>();
                var content = result.SafeGetResult();
                Response rep = new Response();
                try
                {
                    rep = _session.CompletionLoad(content).response;
                }
                catch (Exception ex)
                {
                    _session.LogSave($"Error in /aiassistant: {ex.Message}", AppName, "ERROR");
                }
                return rep;
            });

            app.MapPut("/entities/save", (Entity p) =>
            {
                ServiceReturn result = new ServiceReturn();
                result.ReturnedId = "";
                result.Message = "";                
                try
                {
                    result = _session.EntitySave(p);         
                }
                catch (Exception ex)
                {
                    result.ReturnedId = "-1";
                    result.Message = ex.Message;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/entities/save", (Entity p) =>
            {
                ServiceReturn result = new ServiceReturn();
                result.ReturnedId = "";
                result.Message = "";
                try
                {
                    result = _session.EntitySave(p);
                }
                catch (Exception ex)
                {
                    result.ReturnedId = "-1";
                    result.Message = ex.Message;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/entities/search", (SearchId p) =>
            {

                List<Entity> result = new List<Entity>();
                try
                {
                    result = _session.EntitySearchByName(p.Criteria);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/entities/load", (SearchId p) =>
            {
                Entity result = new Entity();
                try
                {
                    result = _session.EntityLoad(p.Id);
                }
                catch (Exception ex)
                {
                    throw;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/caches/load", (SearchId p) =>
            {
                Cache result = new Cache();
                try
                {
                    result = _session.CacheLoad(p.Id);
                }
                catch (Exception ex)
                {
                    throw;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/caches/search", (SearchId p) =>
            {
                List<Cache> result = new List<Cache>();
                try
                {
                    result = _session.CacheSearchByValue(p.Criteria);
                }
                catch (Exception ex)
                {
                    throw;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/caches/save", (Cache p) =>
            {
                string result = "";
                try
                {
                    result = _session.CacheSave(p);
                }
                catch (Exception ex)
                {
                    throw;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/news", (NewsParamater mode) =>
            {
                ///TODO : implement a real news fetcher
                List<News> news = new List<News>()
                {
                    new News() { Title = "New version", Rating = 10, Url ="https://www.nexai.net" },
                    new News() { Title = "Virus risk", Rating = 8, Url ="https://www.nexai.net" },
                    new News() { Title = "Ma chaine Youtube", Rating = 5, Url ="https://www.nexai.net" },
                    new News() { Title = "Moi sur LinkedIn", Rating = -10, Url ="https://www.linkedin.com/in/jerome-fortias/" },
                    new News() { Title = "Tu l'as vu ? ", Rating = -5, Url ="https://www.linkedin.com/in/jerome-fortias/" },
                    new News() { Title = "TEST TEST ", Rating = -5, Url ="https://www.linkedin.com/in/jerome-fortias/" },
                };
                return Task.FromResult(news);
            });

            app.MapPost("/log", (Log p) =>
            {
                ServiceReturn result = new ServiceReturn();
                result.ReturnedId = "";
                result.Message = "";
                try
                {
                    result = _session.LogSave(p);

                }
                catch (Exception ex)
                {
                    result.ReturnedId = "-1";
                    result.Message = ex.Message;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/symbolicencoder/load", (SearchId p) =>
            {
                SymbolicEncoder result = new SymbolicEncoder();
                try
                {
                    result = _session.SymbolicEncoderLoad(p.Id);
                }
                catch (Exception ex)
                {
                    throw;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/symbolicdecoder/load", (SearchId p) =>
            {
                SymbolicDecoder result = new SymbolicDecoder();
                try
                {
                    result = _session.SymbolicDecoderLoad(p.Id);
                }
                catch (Exception ex)
                {
                    throw;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/symbolicencoder/save", (SymbolicEncoder p) =>
            {
                ServiceReturn result = new ServiceReturn();
                result.ReturnedId = "";
                result.Message = "";
                try
                {
                    result.ReturnedId = _session.SymbolicEncoderSave(p);

                }
                catch (Exception ex)
                {
                    result.ReturnedId = "-1";
                    result.Message = ex.Message;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/symbolicdecoder/save", (SymbolicDecoder p) =>
            {
                ServiceReturn result = new ServiceReturn();
                result.ReturnedId = "";
                result.Message = "";
                try
                {
                    result.ReturnedId = _session.SymbolicDecoderSave(p);
                }
                catch (Exception ex)
                {
                    result.ReturnedId = "-1";
                    result.Message = ex.Message;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/symbolicencoder/search", (SearchId p) =>
            {
                List<SymbolicEncoder> result = new List<SymbolicEncoder>();
                try
                {
                    result = _session.SymbolicEncoderSearch(p.Criteria);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return Task.FromResult(result);
            });

            app.MapPost("/symbolicdecoder/search", (SearchId p) =>
            {
                List<SymbolicDecoder> result = new List<SymbolicDecoder>();
                try
                {
                    result = _session.SymbolicDecoderSearch(p.Criteria);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return Task.FromResult(result);
            });

            app.MapGet("/", (request) =>
            {
                request.Response.Redirect("swagger");
                return Task.CompletedTask;
            });
            app.UseSwaggerUI();
            app.Run();
        }
    }
}
