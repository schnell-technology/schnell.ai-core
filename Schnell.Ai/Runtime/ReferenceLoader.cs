using McMaster.NETCore.Plugins;
using Schnell.Ai.Runtime.Repositories;
using Schnell.Ai.Sdk.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Schnell.Ai.Runtime
{
    /// <summary>
    /// Loads dynamic references
    /// </summary>
    internal static class ReferenceLoader
    {
        private static List<Sdk.Definitions.ReferenceDefinition> loadedReferences = new List<Sdk.Definitions.ReferenceDefinition>();
        private static List<Assembly> loadedAssmblies = new List<Assembly>();
        internal static async Task<bool> LoadReferences(Logger log, Sdk.AiContext ctx)
        {
            await Runtime.Repositories.RepositoryHandler.InitializeRepositories(log);
            var success = true;
            foreach (var r in ctx.Definition?.References)
            {
                var result = await LoadReference(log, r.Reference, r.Version);
                if (result == false)
                    success = false;
            }
            return success;
        }

        internal static async Task<bool> LoadReference(Logger log, string module, string version)
        {
            //local project-modules
            var localPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "ai_modules", module, version, $"{module}.dll");

            if (!System.IO.File.Exists(localPath))
            {
                foreach (IRepository repo in Runtime.Repositories.RepositoryHandler.Repositories)
                {
                    try
                    {
                        if (await repo.Exists(module, version))
                        {
                            await repo.FindAndExtractOrigin(module, version);
                            if (System.IO.File.Exists(localPath))
                                break;
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Write(LogEntry.LogType.Error, $"Couldn't load '{module}' (Version: {version}) from repository '{repo.Path}'");
                        return false;
                    }
                }
            }
            if (!loadedReferences.Any(lr => lr.Reference == module && lr.Version == version))
            {
                try
                {
                    LoadAssemblyFile(localPath);
                    return true;
                }
                catch (Exception ex)
                {
                    log?.Write(LogEntry.LogType.Fatal, $"Couldn't load '{module}' (Version: {version})");
                    return false;
                }
            }

            return false;
        }

        internal static Assembly LoadAssemblyFile(string moduleName, string version)
        {
            var localPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "ai_modules", moduleName, version, $"{moduleName}.dll");
            return LoadAssemblyFile(localPath);
        }

        internal static Assembly LoadAssemblyFile(string localPath)
        {
            if (System.IO.File.Exists(localPath))
            {
                try
                {
                    //Runtime-specific loading
                    PluginLoader loader = PluginLoader.CreateFromAssemblyFile(localPath,                        
                        sharedTypes: new[] {
                                typeof(Sdk.DataSets.IDataSetImporter),
                                typeof(Sdk.DataSets.IDataSetExporter),
                                typeof(Sdk.Actions.IActionBase),
                                typeof(Sdk.Configuration.IConfigurationHandler),
                                typeof(Sdk.Interfaces.IConfigurable),
                                typeof(Sdk.Interfaces.IModeled),
                                typeof(Sdk.Interfaces.INamed),
                        },
                         config => {
                             config.PreferSharedTypes = true;
                         }
                        );
                    Assembly pluginDll = loader.LoadDefaultAssembly();
                    loadedAssmblies.Add(pluginDll);
                    return pluginDll;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return null;
        }

        internal static T CreateInstance<T>(string type)
        {
            foreach(var a in loadedAssmblies)
            {
                var tp = a.GetTypes().Where(t => t.FullName == type).FirstOrDefault();
                if(tp != null)
                {
                    var instance = Activator.CreateInstance(tp);
                    return (T)instance;
                }
            }

            throw new TypeLoadException($"No type-definition for '{type}' could be found");
        }
    }
}
