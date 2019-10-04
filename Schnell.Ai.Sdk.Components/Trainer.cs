using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Schnell.Ai.Sdk.Actions;
using Schnell.Ai.Sdk.Configuration;

namespace Schnell.Ai.Sdk.Components
{

    /// <summary>
    /// Configuration for a generic data-trainer
    /// </summary>
    [DataContract]
    public class TrainerConfiguration
    {
        /// <summary>
        /// Name of model to be trained
        /// </summary>
        [DataMember(Name = "Model")]
        [JsonProperty(Required = Required.Always)]
        public string Model { get; set; }

        /// <summary>
        /// DataSet-name of data to be used as training-data
        /// </summary>
        [DataMember(Name = "TrainingData")]
        [JsonProperty(Required = Required.Always)]
        public string TrainingData { get; set; }
    }

    /// <summary>
    /// Base-class for a trainer
    /// </summary>
    public abstract class TrainerBase : Sdk.Actions.ActionBase
    {
        
        protected ConfigurationHandler<TrainerConfiguration> ConfigurationHandler { get; set; }
        public override IConfigurationHandler Configuration => ConfigurationHandler;

        protected TrainerBase()
        {
           
        }

        protected override void OnBuilt()
        {
            base.OnBuilt();
            ConfigurationHandler = new ConfigurationHandler<TrainerConfiguration>(this.Definition);
        }
    }
}
