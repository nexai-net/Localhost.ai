namespace Localhost.AI.Admin
{
    using Elasticsearch.Net;
    using Localhost.AI.Core.Framework;
    using Localhost.AI.Core.Helpers;
    using Localhost.AI.Core.Models;
    using Localhost.AI.Core.Models.Corpus;
    using Localhost.AI.Core.Models.LLM;
    using System.Text.Json;

    /// <summary>
    /// Main entry point for the Localhost.AI Admin application.
    /// Provides administrative functionality for managing the AI system.
    /// </summary>
    internal class Program
    {
        static private SessionManager _session;
        static private Config _config;
        
        /// <summary>
        /// Main entry point for the Admin application.
        /// Initializes the system and performs administrative operations.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            // Load configuration from file
            Config config = ConfigurationManager.GetFromFile<Config>("config.json");
            
            // Display application startup information
            Console.WriteLine($"Starting {config.AppName} - {config.AppDescription}");
            Console.WriteLine($"Documentation: {config.AppDocumentationUrl}");

            // Set up application name with fallback
            string AppName = config.AppName ?? "APPx";
            _config = config;
            
            // Initialize session manager with the configuration
            _session = new SessionManager(_config);
            
            // Log that the application has started
            _session.LogSave("Application started", AppName, "Info");
            string[] Files = System.IO.Directory.GetFiles(@"\\Spinoza\Data\Sources\data.llm.explorer\PromptCollections\BigOne", "*.json");

            foreach(string file in Files)
            {
                Console.WriteLine($"Processing file: {file}");
                try
                {
                    Models.pfff story = GenerateStoryFromJson(file);
                    Console.WriteLine($"Story Title: {story.StoryTitle}");
                    Cache cache = new Cache();
                    cache.Id = Guid.NewGuid().ToString();
                    cache.prompt = $"{story.Sentence1}\n{story.Sentence2}\n{story.Sentence3}\n{story.Sentence4}\n{story.Sentence5}\n";
                    cache.completion = story.Completion;
                    cache.language = "en";
                    cache.model = "none";
                    cache.Date = DateTime.Now;
                    cache.UserName = Environment.UserName;
                    cache.MachineName = Environment.MachineName;
                    cache.Comment = $"Imported from {file}";
                    cache.tagsMustNot = new List<string>();
                    cache.tagsMust = new List<string>();
                    cache.tagsShould = new List<string>();
                    cache.generatedTags = new List<string>();
                    cache.generatedSystemPrompt = "";
                    cache.chatmode = "archive";
                    cache.ParentCacheId = "";      
                    string newid = _session.CacheSave(cache);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error saving cache for story {ex.Message}");
                }
            }
        }

        public static Models.pfff GenerateStoryFromJson(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"File not found: {filePath}");

            string json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Models.pfff story = JsonSerializer.Deserialize<Models.pfff>(json, options)
                           ?? throw new InvalidOperationException("Failed to deserialize JSON into Story.");
            return story;
        }

        private static void ExtractDico(string line)
        {
            Dico dico = new Dico();
            dico.Word = line.Trim();
            dico.FirstLetter = dico.Word.Substring(0, 1).ToLower();
            dico.Length = dico.Word.Length;
            dico.Language = "it";
            dico.Source = "Github-italian.txt";
            dico.Acounts = TextHelper.CountCharInString(dico.Word, 'a');
            dico.Bcounts = TextHelper.CountCharInString(dico.Word, 'b');
            dico.Ccounts = TextHelper.CountCharInString(dico.Word, 'c');
            dico.Dcounts = TextHelper.CountCharInString(dico.Word, 'd');
            dico.Ecounts = TextHelper.CountCharInString(dico.Word, 'e');
            dico.Fcounts = TextHelper.CountCharInString(dico.Word, 'f');
            dico.Gcounts = TextHelper.CountCharInString(dico.Word, 'g');
            dico.Hcounts = TextHelper.CountCharInString(dico.Word, 'h');
            dico.Icounts = TextHelper.CountCharInString(dico.Word, 'i');
            dico.Jcounts = TextHelper.CountCharInString(dico.Word, 'j');
            dico.Kcounts = TextHelper.CountCharInString(dico.Word, 'k');
            dico.Lcounts = TextHelper.CountCharInString(dico.Word, 'l');
            dico.Mcounts = TextHelper.CountCharInString(dico.Word, 'm');
            dico.Ncounts = TextHelper.CountCharInString(dico.Word, 'n');
            dico.Ocounts = TextHelper.CountCharInString(dico.Word, 'o');
            dico.Pcounts = TextHelper.CountCharInString(dico.Word, 'p');
            dico.Qcounts = TextHelper.CountCharInString(dico.Word, 'q');
            dico.Rcounts = TextHelper.CountCharInString(dico.Word, 'r');
            dico.Scounts = TextHelper.CountCharInString(dico.Word, 's');
            dico.Tcounts = TextHelper.CountCharInString(dico.Word, 't');
            dico.Ucounts = TextHelper.CountCharInString(dico.Word, 'u');
            dico.Vcounts = TextHelper.CountCharInString(dico.Word, 'v');
            dico.Wcounts = TextHelper.CountCharInString(dico.Word, 'w');
            dico.Xcounts = TextHelper.CountCharInString(dico.Word, 'x');
            dico.Ycounts = TextHelper.CountCharInString(dico.Word, 'y');
            dico.Zcounts = TextHelper.CountCharInString(dico.Word, 'z');
            dico.Id = Guid.NewGuid().ToString();
            dico.Comment = $"{dico.Word} Imported from {dico.Source}";
            dico.UserName = Environment.UserName;
            dico.Date = DateTime.Now;
            dico.MachineName = Environment.MachineName;

            string newid = _session.DicoSave(dico);
        }
    }
}
