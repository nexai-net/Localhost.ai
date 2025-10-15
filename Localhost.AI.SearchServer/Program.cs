using Democrite.Framework.Builders;
using Democrite.Framework.Core.Abstractions;
using Localhost.AI.Core.Framework;
using Localhost.AI.Core.Helpers;
using Localhost.AI.Core.Models;
using Localhost.AI.Core.Models.LLM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ConfigurationManager = Localhost.AI.Core.Helpers.ConfigurationManager;

namespace Localhost.AI.Search
{
    /// <summary>
    /// Main entry point for the DialogServer application.
    /// Configures and starts the web API server with dialog processing capabilities.
    /// </summary>
    internal class Program
    {
        static private SessionManager _session;
        static private Config _config;

        /// <summary>
        /// Main entry point for the DialogServer application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
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

            builder.Services.AddSwaggerGen(s => s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Tuto Demo", Version = "v1" })).AddEndpointsApiExplorer();

            var SearchProcessSeq = Sequence.Build("JFO", fixUid: new Guid("D1F7C7EB-77F7-488A-91D7-77E4D5D16448"), metadataBuilder: m =>
                                         {
                                             m.Description("Search Sequence")
                                              .AddTags("search", "nexai");
                                         })
                                         .RequiredInput<string>()
                                         .Use<ISearchPostprocessing>().Configure("JFO-TABLE").Call((g, i, ctx) => g.InboundProcessing(i, ctx)).Return
                                         .Use<ISearchPreprocessingVGrain>().Configure("JFO-TABLE").Call((g, i, ctx) => g.DoProcess(i, ctx)).Return                                         
                                         .Use<ISearchProcessorVGrain>().Configure("JFO-TABLE").Call((g, i, ctx) => g.DoProcess(i, ctx)).Return
                                         .Build();

          

            builder.Host.UseDemocriteNode(b =>
            {
                b.WizardConfig()
                .NoCluster()
                .ConfigureLogging(c => c.AddConsole())
                .AddInMemoryDefinitionProvider(d => d.SetupSequences(SearchProcessSeq));
                b.ManualyAdvancedConfig().UseDashboard(options =>
                {
                    options.HostSelf = true;  // host it inside the silo
                    options.Port = 9090;      // <-- change the default 8080 here
                });
            }
                                                );
            var app = builder.Build();
            app.UseSwagger();

            app.MapPost("v1/search", async (SearchRequest d, [FromServices] IDemocriteExecutionHandler handler) =>
            {


                Console.WriteLine("Received request for /v1/chat/completions");
                //d.model = "mistral-small3.1";
               
                SearchFull search = new SearchFull();
                search.UserName = Environment.UserName;
                search.Date = DateTime.Now;
                search.MachineName = Environment.MachineName;
                search.Id = Guid.NewGuid().ToString();
                search.searchRequest = d;


                var id = _session.SearchProcessSave(search);

                /*_session.LogSave($"Request to /aiassistant: {d.messages[d.messages.Count - 1].content}", "DIALOGSERVER_AGENT_PROCESSOR", "Info");*/

                var result = await handler.Sequence<string>(SearchProcessSeq.Uid)
                                       .SetInput(id)
                                       .RunAsync<string>();
                var content = result.SafeGetResult();
                SearchFull rep = new SearchFull();

                try
                {
                    rep = _session.SearchProcessLoad(content);

                }
                catch (Exception ex)
                {
                    _session.LogSave($"Error in /search: {ex.Message}", AppName, "ERROR");
                }
                return rep;
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
