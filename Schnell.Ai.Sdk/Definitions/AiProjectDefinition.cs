using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Schnell.Ai.Sdk.Definitions
{
    [DataContract]
    public class AiProjectDefinition
    {
        /// <summary>
        /// Version of the project-definition
        /// </summary>
        [DataMember(Name = "Version")]
        public string Version { get; set; }

        /// <summary>
        /// Required referenced modules
        /// </summary>
        [DataMember(Name = "References")]
        public IEnumerable<ReferenceDefinition> References { get; set; }

        /// <summary>
        /// Artifacts which contains data to be used in this project
        /// </summary>
        [DataMember(Name = "Artifacts")]
        public ArtifactsDefinition Artifacts { get; set; }

        /// <summary>
        /// Actions which can be used in this project
        /// </summary>
        [DataMember(Name = "Actions")]
        public ActionDefinition Actions { get; set; }

        /// <summary>
        /// Pipelines, which defines the order of actions to be executed
        /// </summary>
        [DataMember(Name = "Pipelines")]
        public IEnumerable<PipelineDefinition> Pipelines { get; set; }

        public static AiProjectDefinition Default
        {
            get {
                var projDefinition = new AiProjectDefinition();
                projDefinition.Version = "1.0";

                projDefinition.References = new List<ReferenceDefinition>();
                projDefinition.Pipelines = new[] { new PipelineDefinition() { Name = "default", Stages = new List<PipeStageDefinition>() } };
                projDefinition.Actions = new ActionDefinition();
                projDefinition.Artifacts = new ArtifactsDefinition();

                return projDefinition;
            }
        }
    }
}
