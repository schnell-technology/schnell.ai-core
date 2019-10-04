using Schnell.Ai.Sdk.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Runtime.Repositories
{
    /// <summary>
    /// Handles the registered repositories
    /// </summary>
    public static class RepositoryHandler
    {
        static List<IRepository> repositories = new List<IRepository>();

        /// <summary>
        /// Get registered repositories
        /// </summary>
        public static IEnumerable<IRepository> Repositories { get { return repositories.AsEnumerable();  } }

        /// <summary>
        /// Initialize registered repositories
        /// </summary>
        /// <param name="logger">Logger for log-informations</param>
        /// <returns>Registered repositories</returns>
        public static async Task<List<IRepository>> InitializeRepositories(Logger logger)
        {
            repositories.Clear();            
            var conf = await Environment.SchnellAiEnvironment.GetConfiguration();
            var repos = new List<IRepository>();

            conf.Repositories.ForEach(r =>
            {
                var uri = new Uri(r);
                if (uri.Scheme == "http" || uri.Scheme == "https")
                {
                    repos.Add(new WebRepository(uri));
                }
                else if (uri.Scheme == "file")
                {
                    repos.Add(new FileRepository(uri));
                }
                else
                {
                    logger?.Write(LogEntry.LogType.Warning, $"Repository '{r}' has not a valid scheme (file, http, https) and therefore cannot be used as a source.");
                }
            }
            );
            repositories = repos;
            return repos;
        }

        /// <summary>
        /// Register a new repository
        /// </summary>
        /// <param name="path">Path to repository</param>
        /// <returns>Task</returns>
        public static async Task CreateRepository(Uri path)
        {
            if (Repositories.Any(r => r.Path.ToString() == path.ToString()))
                throw new Exception("Repository already exists");
            else
            {
                var conf = await Environment.SchnellAiEnvironment.GetConfiguration();
                conf.Repositories.Add(path.ToString());
                await Environment.SchnellAiEnvironment.SetConfiguration(conf);                
            }
        }

        /// <summary>
        /// Remove a registered repository
        /// </summary>
        /// <param name="path">Path to repository</param>
        /// <returns>Task</returns>
        public static async Task RemoveRepository(Uri path)
        {            
            if (!Repositories.Any(r => r.Path == path))
                throw new Exception("Repository does not exists");
            else
            {
                var conf = await Environment.SchnellAiEnvironment.GetConfiguration();
                conf.Repositories.Remove(path.ToString());
                await Environment.SchnellAiEnvironment.SetConfiguration(conf);
            }
        }
    }
}
