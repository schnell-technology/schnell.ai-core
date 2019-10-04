using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Schnell.Ai
{
    /// <summary>
    /// Schnell.Ai Main-Process
    /// </summary>
    public partial class Program
    {
        /// <summary>
        /// Is in development-mode?
        /// </summary>
        internal static bool DevelopmentMode { get; set; } = false;

        public static void Main(string[] args)
        {
#if DEBUG
            DevelopmentMode = true;
#endif

            var app = new CommandLineApplication();
            app.Name = "schnell.ai";
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", "-v");
            app.FullName = "schnell.ai " + ServiceEnvironment.Edition;

            app.ShortVersionGetter = () => { return typeof(Program).Assembly.GetName().Version.ToString(); };
            app.LongVersionGetter = () => { return typeof(Program).Assembly.GetName().Version.ToString(); };

            app.OnExecute(() =>
            {
                app.ShowVersion();
                return 0;
            });

            new CliCommands.CliCommandRegister().Register(app);
            Runtime.Repositories.RepositoryHandler.InitializeRepositories(null).GetAwaiter().GetResult();

            app.Execute(args);

            if (DevelopmentMode)
            {
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("Execution has been ended. Press enter to close.");
                Console.ReadLine();
            }
        }
    }
}
