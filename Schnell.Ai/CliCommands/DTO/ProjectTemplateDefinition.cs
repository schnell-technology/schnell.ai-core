using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Schnell.Ai.CliCommands.DTO
{
    [DataContract]
    public class ProjectTemplateDefinition
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string URL { get; set; }
    }
}
