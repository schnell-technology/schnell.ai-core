using Schnell.Ai.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Schnell.Ai.Sdk.Definitions
{
    /// <summary>
    /// Defines the actions and steps of a pipeline
    /// </summary>
    [DataContract]
    public class ActionDefinition
    {
        /// <summary>
        /// Defines the processors, which handles data
        /// </summary>
        [DataMember(Name = "Processors")]
        public IEnumerable<ProcessorDefinition> Processors { get; set; } = new List<ProcessorDefinition>();
    }

    /// <summary>
    /// Processes data with specified parameters
    /// </summary>
    [DataContract]
    public class ProcessorDefinition : IAction
    {
        /// <summary>
        /// Name of the processor
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Type of the processor in a module
        /// </summary>
        [DataMember(Name = "Type")]
        public string Type { get; set; }
        
        /// <summary>
        /// Configuration for the process-step
        /// </summary>
        [DataMember(Name = "Configuration")]
        public IDictionary<string, object> Configuration { get; set; }
    }
}
