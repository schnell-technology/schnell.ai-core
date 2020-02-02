using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using Schnell.Ai.Runtime.Environment;

namespace Schnell.Ai.CliCommands
{
    /// <summary>
    /// Common actions for schnell.AI
    /// </summary>
    internal class Development
    {
        internal static void Register(CommandLineApplication app)
        {
            app.Command("dev-bld", (command) =>
            {
                command.Description = "Build module";
                var projectArgument = command.Option("--proj|-p <csproject>", "CSProj-File", CommandOptionType.SingleValue);
                var outArgument = command.Option("--out|-o <path>", "Optional output path", CommandOptionType.SingleValue);
                var verArgument = command.Option("--ver|-v <version>", "Optional version parameter", CommandOptionType.SingleValue);
                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {
                    if (!projectArgument.HasValue() && !String.Equals(System.IO.Path.GetExtension(projectArgument.Value()), "csproj", StringComparison.InvariantCultureIgnoreCase))
                        Console.WriteLine("Please provide a valid csproj-Project");

                    var outBase = Path.Combine(Path.GetDirectoryName(projectArgument.Value()), "bin", "output");

                    if (outArgument.HasValue())
                        outBase = outArgument.Value();

                    var outputDir = Path.Combine(outBase, Path.GetFileNameWithoutExtension(projectArgument.Value()));
                    var outputFile = Path.Combine(outBase, Path.GetFileNameWithoutExtension(projectArgument.Value()) + ".zip");
                    var buildParam = $"build {projectArgument.Value()} -c Release -o {outputDir}";

                    if(verArgument.HasValue())
                        buildParam = buildParam + $" -p:Version={verArgument.Value()}";


                    try
                    {
                        using (Process compiler = new Process())
                        {
                            compiler.StartInfo.FileName = "dotnet";
                            compiler.StartInfo.Arguments = buildParam;
                            compiler.StartInfo.UseShellExecute = false;
                            compiler.StartInfo.RedirectStandardOutput = true;
                            compiler.Start();

                            Console.WriteLine(compiler.StandardOutput.ReadToEnd());

                            compiler.WaitForExit();
                            
                            if (compiler.ExitCode != 0)
                            {
                                Console.WriteLine("Build of module failed. Please see the output for more informations.");
                                return -1;
                            }
                            else
                            {
                                var removeFiles = new[] { "Schnell.Ai.Sdk.dll", "Schnell.Ai.Shared.dll" };
                                removeFiles.ToList().ForEach(f =>
                                {
                                    var fp = Path.Combine(outputDir, f);
                                    if (File.Exists(fp))
                                        File.Delete(fp);
                                });
                                File.Delete(outputFile);
                                System.IO.Compression.ZipFile.CreateFromDirectory(outputDir, outputFile);
                                Directory.Delete(outputDir, true);
                                Console.WriteLine("===");
                                Console.WriteLine($"Sucessfully build project. Output-File: {outputFile}");
                                Console.WriteLine("===");
                                return 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Build and Package failed. Please see the output for more informations.");
                        return -1;
                    }
                });
            });
        }
    }
}
