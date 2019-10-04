using System;
using System.Collections.Generic;
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

        internal WebRepository(Uri uri) : base(uri)
        {
            SetFilePath(Environment.SchnellAiEnvironment.TempDir);
        }

        public override async Task<bool> Exists(string module, string version)
        {
            var result = await base.Exists(module, version);
            if (result == false)
            {
                var uri = new Uri(Path + $"/{module}_{version}.zip");
                WebRequest request = WebRequest.Create(uri);
                request.Method = "HEAD";
                HttpWebResponse response = null;

                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    result = true;
                }
                catch (WebException ex)
                {
                    result = false;                    
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }
                }
            }

            return result;
        }

        public override async Task FindAndExtractOrigin(string module, string version)
        {
            if (await base.Exists(module, version) == false)
            {
                var uri = new Uri(Path + $"/{module}_{version}.zip");
                using (var wc = new WebClient())
                {
                    var temp = Environment.SchnellAiEnvironment.TempDir;
                    var local = System.IO.Path.Combine(temp, $"{module}_{version}.zip");
                    await wc.DownloadFileTaskAsync(uri.ToString(), local);
                }
            }
            await base.FindAndExtractOrigin(module, version);
        }
    }
}
