using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Runtime.Environment
{
    /// <summary>
    /// Local schnell.ai-environment
    /// </summary>
    internal static class SchnellAiEnvironment
    {
        private static string _tempDir = null;

        /// <summary>
        /// Environment directory
        /// </summary>
        internal static string EnvironmentDir
        {
            get
            {
                return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "schnell.ai");
            }
        }

        /// <summary>
        /// Temporary directory
        /// </summary>
        internal static string TempDir
        {
            get
            {
               if(_tempDir == null)
                {
                    var p = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "schnell.ai");
                    System.IO.Directory.CreateDirectory(p);
                    _tempDir = p;
                }
                return _tempDir;
            }
        }

        /// <summary>
        /// Get current configuration
        /// </summary>
        /// <returns></returns>
        internal static async Task<SchnellAiConfigurationManifest> GetConfiguration()
        {
            var filePath = System.IO.Path.Combine(EnvironmentDir, "manifest.json");
            if (System.IO.File.Exists(filePath))
                return Shared.Helper.Json.Deserialize<SchnellAiConfigurationManifest>(System.IO.File.ReadAllText(filePath));
            else
                return SchnellAiConfigurationManifest.Default;
        }

        /// <summary>
        /// Set configuration
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        internal static async Task SetConfiguration(SchnellAiConfigurationManifest manifest)
        {
            var filePath = System.IO.Path.Combine(EnvironmentDir, "manifest.json");
            System.IO.File.WriteAllText(filePath, Shared.Helper.Json.Serialize<SchnellAiConfigurationManifest>(manifest));
        }
    }
}
