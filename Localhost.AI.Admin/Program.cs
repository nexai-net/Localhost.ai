namespace Localhost.AI.Admin
{
    using Localhost.AI.Core.Helpers;
    using Localhost.AI.Core.Models.LLM;
    using Localhost.AI.Core.Models;
    using Localhost.AI.Core.Framework;
    using Localhost.AI.Core.Models.Corpus;

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
            /*
            // Search for entities by name as a demonstration
            List<Entity> entities = _session.EntitySearchByName("qui est Jerome Fortias ?");
            
            // Display found entities
            foreach(Entity e in entities)
            {
                Console.WriteLine($"Entity: {e.Name} - {e.Id}");
            }*/

            string filePath = @"N:\datasets.collection\dic\italian.txt";

            List<string> lines = System.IO.File.ReadAllLines(filePath).ToList();
            foreach (string line in lines)
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
                
                 string newid =  _session.DicoSave(dico);
                

            }

        }
    }
}
