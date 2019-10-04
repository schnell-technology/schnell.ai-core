using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schnell.Ai.CliCommands
{
    /// <summary>
    /// Runs a Ai-Project and given pipeline
    /// </summary>
    public class Runner
    {
        internal static void Register(CommandLineApplication app)
        {
            var logger = new Runtime.RuntimeLogger("schnell.ai");
            app.Command("run", (command) =>
            {
                command.Description = "Run a schnell.AI pipeline";
                command.HelpOption("-?|-h|--help");
                                
                var pipelineArgument = command.Option("--pipeline|-p <pipename>", "Pipeline-name", CommandOptionType.SingleValue);
                var variableArgument = command.Option("--var|-v <json>", "Variables as JSON", CommandOptionType.SingleValue);
                var variableFileArgument = command.Option("--varfile|-vf <filename>", "Variables as JSON-file", CommandOptionType.SingleValue);
                var devArgument = command.Option("--dev|-d", "Developer Mode", CommandOptionType.NoValue);                
                var actionArgument = command.Option("--action|-a <actionname>", "Run single step", CommandOptionType.SingleValue);
                var attachArgument = command.Option("--attach", "Attach .NET Assembly", CommandOptionType.MultipleValue);
                var waitForArgument = command.Option("--waitfor", "Set 'x' for x ms to wait, or 'debug' to wait for a attached debugger", CommandOptionType.SingleValue);

                command.OnExecute(async () =>
                {
                    try
                    {
                        logger.Write(Sdk.Logging.LogEntry.LogType.Info, "Welcome to schnell.ai");
                        logger.Write(Sdk.Logging.LogEntry.LogType.Info, $"Your current directory is: {Environment.CurrentDirectory}");

                        Program.DevelopmentMode = devArgument.HasValue();

                        if(waitForArgument.HasValue())
                        {
                            int msToWait = 0;
                            if(int.TryParse(waitForArgument.Value(), out msToWait))
                            {
                                logger.Write(Sdk.Logging.LogEntry.LogType.Info, $"Waiting for {msToWait}ms before start.");
                                System.Threading.Thread.Sleep(msToWait);
                                logger.Write(Sdk.Logging.LogEntry.LogType.Info, $"Waited {msToWait}ms.");
                            }
                            else if(String.Equals("debug", waitForArgument.Value(), StringComparison.InvariantCultureIgnoreCase))
                            {
                                while(!System.Diagnostics.Debugger.IsAttached)
                                {
                                    logger.Write(Sdk.Logging.LogEntry.LogType.Info, "Waiting for debugger to be attached...");
                                    System.Threading.Thread.Sleep(20);
                                }
                                logger.Write(Sdk.Logging.LogEntry.LogType.Info, $"Debugger has been attached.");
                            }
                        }

                        var file = "ai.project.json";
                        var variableFile = variableFileArgument.Value() ?? "ai.vars.json";
                        IDictionary<string, object> variables = null;

                        if (variableArgument.Value() != null)
                        {
                            variables = Shared.Helper.Json.Deserialize<IDictionary<string, object>>(variableArgument.Value());
                        }

                        if (!System.IO.File.Exists(file))
                        {
                            logger.Write(Sdk.Logging.LogEntry.LogType.Fatal, $"Project-file '{file}' not found.");

                            return 2;
                        }

                        var bld = new Runtime.RuntimeContextBuilder();
                        if(System.IO.File.Exists(variableFile))
                        {
                            var jsonVariables = Shared.Helper.Json.Deserialize<IDictionary<string, object>>(System.IO.File.ReadAllText(variableFile));
                            bld.SetVariables(jsonVariables);
                        }

                        if(variables != null)
                            bld.SetVariables(variables);

                        var ctx = bld.Create(file, logger.CreateChild("project: " + System.IO.Path.GetFileName(file)));

                        if (attachArgument.HasValue())
                        {
                            attachArgument.Values.ForEach(v =>
                            {
                                Runtime.ReferenceLoader.LoadAssemblyFile(v);
                            });
                        }

                        if (ctx != null)
                        {
                            if (pipelineArgument.Value() != null)
                            {
                                ctx.Build(pipelineArgument.Value());
                            }
                            else if (actionArgument.Value() != null)
                            {
                                var pipelineDef = new Sdk.Definitions.PipelineDefinition();
                                pipelineDef.Stages = new[] { new Sdk.Definitions.PipeStageDefinition() { RunAction = actionArgument.Value() } };
                                ctx.Build(pipelineDef);
                            }

                            await ctx.Run();
                        }

                        return 0;
                    }
                    catch (Exception ex)
                    {
                        logger.Write(Sdk.Logging.LogEntry.LogType.Fatal, $"Unhandled Exception '{ex.Message}' has been thrown.");
                        if (Program.DevelopmentMode)
                            throw;
                        return 99;
                    }
                });
            });

           
        }
    }
}

