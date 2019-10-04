using Newtonsoft.Json.Schema;
using Schnell.Ai.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schnell.Ai.Sdk.Configuration
{
    /// <summary>
    /// Configuration-handler for return typed configuration
    /// </summary>
    public interface IConfigurationHandler
    {
        /// <summary>
        /// Type of configuration object
        /// </summary>
        Type ConfigurationType { get; }

        /// <summary>
        /// Configuration instance
        /// </summary>
        object Configuration { get; }

        /// <summary>
        /// Get JsonSchema of the configuration instance
        /// </summary>
        /// <returns></returns>
        NJsonSchema.JsonSchema GetConfigurationSchema();

        /// <summary>
        /// Validate configuration against schema
        /// </summary>
        /// <returns></returns>
        ICollection<string> ValidateConfiguration();
    }

    public class ConfigurationHandler<TConf> : IConfigurationHandler
    {
        private TConf _cachedConfiguration = default(TConf);
        private IConfigurable configurable;
        public Type ConfigurationType { get { return typeof(TConf); } }

        public TConf Configuration { get
            {
                if(_cachedConfiguration == null)
                {
                    ReloadConfiguration();
                }
                return _cachedConfiguration;
            } }

        object IConfigurationHandler.Configuration => Configuration;

        public void ReloadConfiguration()
        {
            if (this.configurable != null)
            {
                var conf = Shared.Helper.ObjectDictionaryMapper.GetObject<TConf>(this.configurable.Configuration);
                this._cachedConfiguration = conf;
            }
        }

        

        public ConfigurationHandler(IConfigurable configurable)
        {
            this.configurable = configurable;
            this.ReloadConfiguration();
        }


        public NJsonSchema.JsonSchema GetConfigurationSchema()
        {
            return Shared.Helper.Json.GetJsonSchema(ConfigurationType);
        }

        public ICollection<string> ValidateConfiguration()
        {
            if (this.configurable != null)
            {
                //variante 1 - schema-validation failed with IDictionary

                //var json = Shared.Helper.Json.Serialize(conf);
                //return Shared.Helper.Json.ValidateJsonWithSchema(json, GetConfigurationSchema());

                //variante 2 - schema-validation can be evaluated with IDictionary
                if (configurable.Configuration != null)
                    return Shared.Helper.Json.ValidateObjectWithSchema(configurable.Configuration, GetConfigurationSchema()).Select(e => e.ToString()).ToList(); ;

                return new List<string>();
            }
            else
                return new List<string>();
        }
    }
}
