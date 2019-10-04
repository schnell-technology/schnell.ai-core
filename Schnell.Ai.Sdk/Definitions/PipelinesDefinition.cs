using Schnell.Ai.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Schnell.Ai.Sdk.Definitions
{
    /// <summary>
    /// Defines a pipeline
    /// </summary>
    [DataContract]
    public class PipelineDefinition : INamed
    {
        /// <summary>
        /// Name of the pipeline
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Stages, which will be executed successively
        /// </summary>
        [DataMember(Name = "Stages")]
        public IEnumerable<PipeStageDefinition> Stages { get; set; }
    }

    /// <summary>
    /// Definition of a stage
    /// </summary>
    [DataContract]
    public class PipeStageDefinition
    {
        /// <summary>
        /// Action to be executed
        /// </summary>
        [DataMember(Name = "RunAction")]
        public string RunAction { get; set; }
    }
}
