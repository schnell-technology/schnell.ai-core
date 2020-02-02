using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Schnell.Ai.Runtime.Environment
{
    /// <summary>
    /// Configuration-manifest for local environment
    /// </summary>
    [DataContract]
    public class SchnellAiConfigurationManifest
    {
        /// <summary>
        /// Repository-registry
        /// </summary>
        [DataMember]
        public List<string> Repositories { get; set; } = new List<string>();

        /// <summary>
        /// Licenses for application and modules
        /// </summary>
        [DataMember]
        public List<string> Licenses { get; set; } = new List<string>();

        /// <summary>
        /// Default manifest
        /// </summary>
        public static SchnellAiConfigurationManifest Default
        {
            get
            {
                var def = new SchnellAiConfigurationManifest();
                //def.Repositories.Add("https://repo.schnell.ai/");
                def.Repositories.Add("https://raw.githubusercontent.com/schnell-technology/schnell.ai-modules/master/");
                

                return def;
            }
        }
    }
}
