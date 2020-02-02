using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Runtime.Repositories
{
    /// <summary>
    /// Interface for a module-repository
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Path to repository
        /// </summary>
        Uri Path { get; }

        /// <summary>
        /// Check, if a module with a specific version exists
        /// </summary>
        /// <param name="module">module-name</param>
        /// <param name="version">version-id</param>
        /// <returns></returns>
        Task<RepositoryResult> Exists(string module, string version = null);

        /// <summary>
        /// Find module and extract to module-directory
        /// </summary>
        /// <param name="module">module-name</param>
        /// <param name="version">version-id</param>
        /// <returns></returns>
        Task FindAndExtractOrigin(string module, string version = null);        
    }
}
