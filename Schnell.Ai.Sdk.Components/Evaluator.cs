using Newtonsoft.Json;
using Schnell.Ai.Sdk.Actions;
using Schnell.Ai.Sdk.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Sdk.Components
{
    /// <summary>
    /// Configuration for a basic evaluator
    /// </summary>
    [DataContract]
    public class EvaluatorConfiguration
    {
        /// <summary>
        /// Name of model
        /// </summary>
        [DataMember(Name = "Model")]
        [JsonProperty(Required = Required.Always)]
        public string Model { get; set; }

        /// <summary>
        /// DataSet-Name for input-data
        /// </summary>
        [DataMember(Name = "InputData")]
        [JsonProperty(Required = Required.Always)]
        public string InputData { get; set; }

        /// <summary>
        /// DataSet-Name for result-data
        /// </summary>
        [DataMember(Name = "ResultData")]
        [JsonProperty(Required = Required.Always)]
        public string ResultData { get; set; }
    }

    /// <summary>
    /// Base-class for a evaluator
    /// </summary>
    public abstract class EvaluatorBase : Sdk.Actions.ActionBase
    {
        protected virtual ConfigurationHandler<EvaluatorConfiguration> ConfigurationHandler { get; set; }
        public override IConfigurationHandler Configuration => ConfigurationHandler;

        protected EvaluatorBase()
        {
            
        }

        protected override void OnBuilt()
        {
            base.OnBuilt();
            this.ConfigurationHandler = new ConfigurationHandler<EvaluatorConfiguration>(this.Definition);            
        }
    }
}
