using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Runtime.Repositories
{
    /// <summary>
    /// Remote web-repository
    /// </summary>
    public class WebRepository : FileRepository
    {
        private bool initialized = false;
        private bool isIndexed = false;

        internal WebRepository(Uri uri) : base(uri)
        {
            SetFilePath(Environment.SchnellAiEnvironment.TempDir);
        }

        private async Task Initialize()
        {
            if (!initialized)
            {
                try
                {
                    var uri = new Uri(Path + $"/.index");
                    WebRequest request = WebRequest.Create(uri);
                    request.Method = "HEAD";
                    HttpWebResponse response = null;

                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                        isIndexed = true;
                    }
                    catch (WebException ex)
                    {
                        isIndexed = false;
                    }
                    finally
                    {
                        response?.Close();
                    }
                }
                catch (Exception ex)
                {

                }

                initialized = true;
            }
        }

        private async Task<RepositoryIndex> GetIndex(string module)
        {
            var uri = new Uri(Path + $"/{module}/.index");
            using (var wc = new WebClient())
            {
                var content = await wc.DownloadStringTaskAsync(uri);
                return Shared.Helper.Json.Deserialize<RepositoryIndex>(content, returnDefaultOnError: true);
            }
        }

        public override async Task<RepositoryResult> Exists(string module, string version = null)
        {
            await Initialize();
            if(version != null)
                version = version.ToLowerInvariant();
            var result = await base.Exists(module, version);
            version = result?.Version;
            
            if (result == null)
            {
                if (isIndexed)
                    result = await ExistsIndexed(module, version);
                else
                    result = await ExistsLegacy(module, version);
            }

            return result;
        }

        private async Task<RepositoryResult> ExistsLegacy(string module, string version = null)
        {
            RepositoryResult result;
            var uri = new Uri(Path + $"/{module}_{version}.zip");
            WebRequest request = WebRequest.Create(uri);
            request.Method = "HEAD";
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                result = new RepositoryResult() { ModuleName=module, Version = version };
            }
            catch
            {
                result = null;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;
        }

        private async Task<RepositoryResult> ExistsIndexed(string module, string version)
        {
            RepositoryResult result;
            var index = await GetIndex(module);
            if (index != null)
            {
                var ver = index.GetVersionOrDefault(version);
                if (ver == null)
                {
                    throw new Exception($"Version {version} is not available.");
                }
                else
                {
                    var uri = new Uri(ver.PackageUrl);
                    WebRequest request = WebRequest.Create(uri);
                    request.Method = "HEAD";
                    HttpWebResponse response = null;

                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                        result = new RepositoryResult() { ModuleName = module, Version = ver?.Version, Author = index.Author };                        
                    }
                    catch
                    {
                        result = null;
                    }
                    finally
                    {
                        if (response != null)
                        {
                            response.Close();
                        }
                    }

                    return result;
                }
            }

            return null;
        }



        public override async Task FindAndExtractOrigin(string module, string version = null)
        {
            await Initialize();
            if(version != null)
                version = version.ToLowerInvariant();

            if (await base.Exists(module, version) == null)
            {
                if (isIndexed)
                {
                    await FindAndExtractIndexed(module, version);
                }
                else
                {
                    await FindAndExtractLegacy(module, version);
                }
            }
            await base.FindAndExtractOrigin(module, version);
        }

        private async Task FindAndExtractLegacy(string module, string version)
        {
            var uri = new Uri(Path + $"/{module}_{version}.zip");
            using (var wc = new WebClient())
            {
                var temp = Environment.SchnellAiEnvironment.TempDir;
                var local = System.IO.Path.Combine(temp, $"{module}_{version}.zip");
                await wc.DownloadFileTaskAsync(uri.ToString(), local);
            }
        }

        private async Task FindAndExtractIndexed(string module, string version)
        {
            var index = await GetIndex(module);
            if (index != null)
            {
                var ver = index.GetVersionOrDefault(version);
                var uri = new Uri(ver.PackageUrl);
                using (var wc = new WebClient())
                {
                    var temp = Environment.SchnellAiEnvironment.TempDir;
                    var local = System.IO.Path.Combine(temp, $"{module}_{ver.Version.ToLowerInvariant()}.zip");
                    await wc.DownloadFileTaskAsync(uri.ToString(), local);
                }
            }

        }
    }
}
