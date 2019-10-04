using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Runtime.Repositories
{
    /// <summary>
    /// File repository
    /// </summary>
    public class FileRepository : IRepository
    {
        /// <summary>
        /// Path to repository
        /// </summary>
        public virtual Uri Path { get; private set; }
        private string fileBasePath;
        internal FileRepository(Uri uri)
        {   
            Path = uri;
            SetFilePath(Path.LocalPath);
        }

        internal void SetFilePath(string path)
        {
            this.fileBasePath = path;
        }

        /// <summary>
        /// Check if path exists
        /// </summary>
        /// <param name="module">module-name</param>
        /// <param name="version">version of module</param>
        /// <returns></returns>
        public async virtual Task<bool> Exists(string module, string version)
        {
            var filePath = System.IO.Path.Combine(fileBasePath, $"{module}_{version}.zip");
            var dirPath = System.IO.Path.Combine(fileBasePath, module, version);
            return System.IO.File.Exists(filePath) || System.IO.Directory.Exists(dirPath);
        }

        /// <summary>
        /// Extract module-file to module-directory
        /// </summary>
        /// <param name="module">module-name</param>
        /// <param name="version">version of module</param>
        /// <returns></returns>
        public async virtual Task FindAndExtractOrigin(string module, string version)
        {
            var filePath = System.IO.Path.Combine(fileBasePath, $"{module}_{version}.zip");
            var dirPath = System.IO.Path.Combine(fileBasePath, module, version);

            var destPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "ai_modules", module, version);

            if (System.IO.File.Exists(filePath))
                System.IO.Compression.ZipFile.ExtractToDirectory(filePath, destPath);
            else if (System.IO.Directory.Exists(dirPath))
                Shared.Helper.File.DirectoryCopy(dirPath, destPath, true);
            else
                throw new DirectoryNotFoundException($"Module '{module}' (Version: '{version}') not found");
        }


    }
}
