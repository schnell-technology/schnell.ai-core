using Microsoft.Extensions.CommandLineUtils;
using Schnell.Ai.Runtime.Environment;

namespace Schnell.Ai.CliCommands
{
    /// <summary>
    /// Common actions for schnell.AI
    /// </summary>
    internal class Common
    {
        internal static void Register(CommandLineApplication app)
        {
            app.Command("cache-clr", (command) =>
            {
                command.Description = "Clear cache";
                command.HelpOption("-?|-h|--help");
                
                command.OnExecute(() =>
                {
                    System.IO.Directory.Delete(SchnellAiEnvironment.TempDir);
                    System.IO.Directory.CreateDirectory(SchnellAiEnvironment.TempDir);
                    return 0;
                });
            });
        }
    }
}
