using ConsoleTables;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Schnell.Ai.CliCommands
{
    /// <summary>
    /// Manages the project and variables in current directory
    /// </summary>
    internal class Project
    {
        private const string TEMPLATE_URL = "https://raw.githubusercontent.com/schnell-technology/schnell.ai-templates/master/index.json";

        internal static void Register(CommandLineApplication app)
        {
            app.Command("project-mk", (command) =>
            {
                command.Description = "Create new project";
                command.HelpOption("-?|-h|--help");

                var templateArg = command.Option("-t", "template-name", CommandOptionType.SingleValue);
                
                command.OnExecute(async () =>
                {

                    var file = "ai.project.json";
                    var varfile = "ai.vars.json";

                    if(File.Exists(file))
                    {
                        Colorful.Console.WriteLine("ERROR: Current directory has already a project file. Please remove 'ai.project.json' and try again.", System.Drawing.Color.Red);
                        return -1;
                    }

                    string content = null;
                    Colorful.Console.WriteLine("Loading available templates...");
                    using(var client = new HttpClient())
                    {
                        try
                        {
                            content = await client.GetStringAsync(TEMPLATE_URL + $"?dt={DateTime.Now.Ticks}");
                            
                        }catch(Exception ex)
                        {
                            Colorful.Console.WriteLine("Couldn't retrieve template list.");
                            return -1;
                        }                        
                    }

                    string templateName = null;
                    var templates = new List<DTO.ProjectTemplateDefinition>();
                    if (!String.IsNullOrEmpty(content))
                    {
                        templates = Shared.Helper.Json.Deserialize<List<DTO.ProjectTemplateDefinition>>(content);

                        if (!templateArg.HasValue() || (templateArg.HasValue() && !templates.Any(t => t.Name == templateArg.Value())))
                        {
                            var table = new ConsoleTable("Name", "Description");
                            templates.ForEach(t => table.AddRow(t.Name, t.Description));
                            table.Write(Format.Minimal);
                        }
                        else
                        {
                            templateName = templateArg.Value();
                        }
                    }

                    if(templateName == null)
                    {
                        Colorful.Console.WriteLine("Please type the template-name you want to use:");
                        templateName = Console.ReadLine();
                    }

                    var template = templates.FirstOrDefault(t => t.Name == templateName);
                    if (template == null)
                    {
                        Colorful.Console.WriteLine("Specified template not found");
                        return -1;
                    }
                    else
                    {
                        using (var client = new HttpClient())
                        {
                            try
                            {
                                var projectFileContent = await client.GetStringAsync(template.URL);
                                System.IO.File.WriteAllText(file, projectFileContent);
                                System.IO.File.WriteAllText(varfile, Shared.Helper.Json.Serialize<Dictionary<string, object>>(new Dictionary<string, object>()));
                                Colorful.Console.WriteLine("Project created in current directory.");
                                return 0;
                            }
                            catch (Exception ex)
                            {
                                Colorful.Console.WriteLine("Couldn't load template");
                                return -1;
                            }
                        }
                    }
                });
            });
        }
    }
}
