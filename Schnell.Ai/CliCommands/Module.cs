using ConsoleTables;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schnell.Ai.CliCommands
{
    internal class Module
    {
        internal static void Register(CommandLineApplication app)
        {
            app.Command("module-detail", (command) =>
            {
                command.Description = "Get informations about a specified module";
                command.HelpOption("-?|-h|--help");

                var nameArg = command.Argument("", "name");
                var info = command.Option("-i", "output module-information", CommandOptionType.NoValue);
                var contents = command.Option("-c", "get module-contents", CommandOptionType.NoValue);


                command.OnExecute(async () =>
                {
                    var module = new Runtime.RuntimeContextBuilder().Create("ai.project.json", new Runtime.RuntimeLogger("temp"));
                    var reference = module.Definition.References.FirstOrDefault(m => String.Equals(m.Reference, nameArg.Value));
                    var assembly = Runtime.ReferenceLoader.LoadAssemblyFile(reference.Reference, reference.Version);

                    if(reference != null && assembly != null)
                    {
                        if (info.HasValue())
                        {
                            var table = new ConsoleTable("Property", "Value");
                            table.AddRow("Name", reference.Reference);
                            table.AddRow("Version", reference.Version);

                            var fileInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                            table.AddRow("Publisher", fileInfo.CompanyName);
                            table.Write(Format.Minimal);
                        }

                        if(contents.HasValue())
                        {
                            var table = new ConsoleTable("Component", "Type");

                            var components = assembly.GetTypes().Where(t => t.IsClass).Select(t =>
                            {
                                if (typeof(Schnell.Ai.Sdk.Components.TrainerBase).IsAssignableFrom(t))
                                    return (type: t, component: "Action (Trainer)");
                                else if (typeof(Schnell.Ai.Sdk.Components.TesterBase).IsAssignableFrom(t))
                                    return (type: t, component: "Action (Tester)");
                                else if (typeof(Schnell.Ai.Sdk.Components.EvaluatorBase).IsAssignableFrom(t))
                                    return (type: t, component: "Action (Evaluator)");
                                else if (typeof(Schnell.Ai.Sdk.Actions.IActionBase).IsAssignableFrom(t))
                                    return (type: t, component: "Action");
                                else if (typeof(Schnell.Ai.Sdk.DataSets.IDataSetExporter).IsAssignableFrom(t))
                                    return (type: t, component: "Artifacts (Data-Set Exporter)");
                                else if (typeof(Schnell.Ai.Sdk.DataSets.IDataSetImporter).IsAssignableFrom(t))
                                    return (type: t, component: "Artifacts (Data-Set Importer)");
                                else return (type: null, component: null);
                            }).Where(t => t.type != null).ToList();

                            components.ForEach(c => table.AddRow(c.type.Name, c.component));
                            table.Write(Format.Minimal);
                        }

                        return 0;
                    }
                    else
                    {
                        Colorful.Console.WriteLine($"Specified module '{nameArg.Value}' not found", System.Drawing.Color.Red);
                        return -1;
                    }
                });
            });

            app.Command("module-ls", (command) =>
            {
                command.Description = "List referenced modules";
                command.HelpOption("-?|-h|--help");

                command.OnExecute(async () =>
                {
                    var table = new ConsoleTable("Name", "Version");

                    var module = new Runtime.RuntimeContextBuilder().Create("ai.project.json", new Runtime.RuntimeLogger("temp"));
                    var references = module.Definition.References.ToList();                    

                    references.ForEach(r => {
                        table.AddRow(r.Reference, r.Version);
                    });
                    
                    table.Write(Format.Minimal);
                    return 0;
                });
            });

            app.Command("module-i", (command => {
                command.Description = "Install module and add it to the ai.project.json";
                command.HelpOption("-?|-h|--help");

                var moduleName = command.Argument("module", "module-name", false);
                var version = command.Argument("version", "version of module", false);

                command.OnExecute(async () =>
                {
                    if(moduleName.Value != null)
                    {
                        var file = "ai.project.json";
                        if (!System.IO.File.Exists(file))
                        {
                            return 2;
                        }

                        var projectFile = Shared.Helper.Json.Deserialize<Sdk.Definitions.AiProjectDefinition>(System.IO.File.ReadAllText(file));
                        var logger = new Runtime.RuntimeLogger("module-install");
                        Console.WriteLine("Please wait while loading and installing the module... this can take a while...");
                        var result = await Runtime.ReferenceLoader.LoadReference(logger, moduleName.Value, version.Value);
                        if(result == false)
                        {
                            Console.WriteLine("Installing the module failed.");
                            return -1;
                        }

                        var references = projectFile.References?.ToList() ?? new List<Sdk.Definitions.ReferenceDefinition>();

                        if (projectFile.References != null && projectFile.References.Any(r => r.Reference == moduleName.Value))
                        {
                            //module already exists
                            var existingReference = references.FirstOrDefault(r => r.Reference == moduleName.Value);
                            existingReference.Version = version.Value;
                        }
                        else
                        {
                            //newly install
                            references.Add(new Sdk.Definitions.ReferenceDefinition() { Reference=moduleName.Value, Version=version.Value });                            
                        }

                        projectFile.References = references;
                        System.IO.File.WriteAllText(file, Shared.Helper.Json.Serialize<Sdk.Definitions.AiProjectDefinition>(projectFile));
                    }
                    return 0;
                });
            }));

            app.Command("module-rm", (command => {
                command.Description = "Remove existing module";
                command.HelpOption("-?|-h|--help");

                var moduleName = command.Argument("module", "module-name", false);                

                command.OnExecute(async () =>
                {
                    if (moduleName.Value != null)
                    {
                        var file = "ai.project.json";
                        if (!System.IO.File.Exists(file))
                        {
                            return 2;
                        }

                        var projectFile = Shared.Helper.Json.Deserialize<Sdk.Definitions.AiProjectDefinition>(System.IO.File.ReadAllText(file));                        

                        var references = projectFile.References?.ToList() ?? new List<Sdk.Definitions.ReferenceDefinition>();

                        if (projectFile.References != null && projectFile.References.Any(r => r.Reference == moduleName.Value))
                        {
                            //module already exists
                            var existingReference = references.FirstOrDefault(r => r.Reference == moduleName.Value);
                        }

                        projectFile.References = references;
                        System.IO.File.WriteAllText(file, Shared.Helper.Json.Serialize<Sdk.Definitions.AiProjectDefinition>(projectFile));
                    }
                    Console.WriteLine($"Module '{moduleName.Value}' has been removed.");
                    return 0;
                });
            }));
        }
    }
}
