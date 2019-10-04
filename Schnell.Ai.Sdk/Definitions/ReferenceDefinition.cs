using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Schnell.Ai.Sdk.Definitions
{
    /// <summary>
    /// Defines a reference to a module
    /// </summary>
    [DataContract]
    public class ReferenceDefinition
    {
        /// <summary>
        /// Name of the module/reference
        /// </summary>
        [DataMember(Name = "Reference")]
        public string Reference { get; set; }

        /// <summary>
        /// Version of the module
        /// </summary>
        [DataMember(Name = "Version")]
        public string Version { get; set; }
    }
}
