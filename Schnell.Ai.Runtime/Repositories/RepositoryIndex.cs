using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Schnell.Ai.Runtime.Repositories
{
    [DataContract]
    public class RepositoryIndex
    {
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public string Copyright { get; set; }
        [DataMember]
        public string License { get; set; }
        [DataMember]
        public IEnumerable<RepositoryVersionEntry> Versions { get; set; }

        public RepositoryVersionEntry GetVersionOrDefault(string version)
        {
            if (version == null)
            {
                version = this.Versions.Select(v => Semver.SemVersion.Parse(v.Version)).Where(v => String.IsNullOrEmpty(v.Prerelease)).Max()?.ToString();
            }

            var ver = this.Versions.FirstOrDefault(v => String.Equals(version, v.Version, StringComparison.InvariantCultureIgnoreCase));
            return ver;
        }
    }

    [DataContract]
    public class RepositoryVersionEntry
    {
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string PackageUrl { get; set; }
    }
}
