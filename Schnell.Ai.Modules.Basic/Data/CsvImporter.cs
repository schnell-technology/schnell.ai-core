using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Schnell.Ai.Sdk.Configuration;
using Schnell.Ai.Sdk.DataSets;

namespace Schnell.Ai.Modules.Basic.Data
{
    [DataContract]
    public class CsvImporterConfiguration
    {
        [DataMember]
        [JsonProperty(Required = Required.Always)]
        public string Path { get; set; }

        [DataMember]
        public string Delimiter { get; set; } = ";";

        [DataMember]
        public bool HasHeader { get; set; } = true;
    }

    public class CsvImporter : Schnell.Ai.Sdk.DataSets.DataSetImporter
    {
        private ConfigurationHandler<CsvImporterConfiguration> _configHandler;
        public override IConfigurationHandler ConfigurationHandler => _configHandler;

        public CsvImporter()
        {

        }

        protected override void OnBuilt()
        {
            base.OnBuilt();
            _configHandler = new ConfigurationHandler<CsvImporterConfiguration>(this.Definition);
        }

        public override async Task<IEnumerable<IDictionary<string, object>>> Import(DataSet ds)
        {
            return ImportCsv(ds);
        }

        private IEnumerable<IDictionary<string, object>> ImportCsv(DataSet ds)
        {
            CsvHelper.Configuration.Configuration csvConf = new CsvHelper.Configuration.Configuration(System.Globalization.CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = _configHandler.Configuration.HasHeader,
                Delimiter = _configHandler.Configuration.Delimiter
            };


            if(String.IsNullOrEmpty(_configHandler.Configuration?.Path) || !System.IO.File.Exists(_configHandler.Configuration?.Path))
            {
                this.Log.Write(Sdk.Logging.LogEntry.LogType.Error, $"File could not be found '{_configHandler.Configuration?.Path}'");
                yield break;
            }

            var recordsRead = 0;

            using (var str = System.IO.File.OpenRead(_configHandler.Configuration.Path))
            using (var rd = new System.IO.StreamReader(str))
            using (var reader = new CsvHelper.CsvReader(rd, csvConf))
            {                
                if (_configHandler.Configuration.HasHeader && reader.Read())
                {
                    reader.ReadHeader();
                }

                while (reader.Read())
                {
                    

                    IDictionary<string, object> record = new Dictionary<string, object>();
                    ds.FieldDefinitions.ToList().ForEach(f =>
                    {
                        object val;

                        if (f.ValueType == Sdk.Definitions.FieldDefinition.ValueTypeEnum.String)
                        {
                            val = reader.GetField(typeof(string), f.Name);
                        }
                        else if (f.ValueType == Sdk.Definitions.FieldDefinition.ValueTypeEnum.Integer)
                        {
                            val = reader.GetField(typeof(int), f.Name);
                        }
                        else if (f.ValueType == Sdk.Definitions.FieldDefinition.ValueTypeEnum.Float)
                        {
                            val = reader.GetField(typeof(float), f.Name);
                        }
                        else if (f.ValueType == Sdk.Definitions.FieldDefinition.ValueTypeEnum.Boolean)
                        {
                            val = reader.GetField(typeof(bool), f.Name);
                        }
                        else
                        {
                            val = reader.GetField(f.Name);
                        }

                        record[f.Name] = val;
                    });

                    recordsRead++;
                    this.Log.Progress(currentValue: recordsRead);
                    yield return record;
                }
            }

        }
    }
}
