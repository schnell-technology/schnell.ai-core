using ConsoleTables;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schnell.Ai.CliCommands
{
    /// <summary>
    /// Manages repositories
    /// </summary>
    internal class Repository
    {
        internal static void Register(CommandLineApplication app)
        {
            app.Command("repo", (command) =>
            {
                command.Description = "Create or remove repositories";
                command.HelpOption("-?|-h|--help");

                var addArgument = command.Option("--add|-a <repo-url>", "Add a new repository", CommandOptionType.SingleValue);
                var removeArgument = command.Option("--rm|-r <repo-url>", "Remove a existing repository", CommandOptionType.SingleValue);
                var listArgument = command.Option("--ls", "List all repositories", CommandOptionType.NoValue);

                command.OnExecute(async () =>
                {
                    if (addArgument.HasValue())
                    {
                        await Runtime.Repositories.RepositoryHandler.InitializeRepositories(null);
                        await Runtime.Repositories.RepositoryHandler.CreateRepository(new Uri(addArgument.Value()));
                    }
                    else if (removeArgument.HasValue())
                    {
                        await Runtime.Repositories.RepositoryHandler.InitializeRepositories(null);
                        await Runtime.Repositories.RepositoryHandler.RemoveRepository(new Uri(removeArgument.Value()));
                    }
                    else if (listArgument.HasValue())
                    {
                        await Runtime.Repositories.RepositoryHandler.InitializeRepositories(null);
                        var table = new ConsoleTable("Repositories");

                        Runtime.Repositories.RepositoryHandler.Repositories.ToList().ForEach(re =>
                        {
                            table.AddRow(re.Path);
                        });

                        table.Write(Format.Minimal);
                    }
                    return 0;
                });
            });
        }
    }
}
