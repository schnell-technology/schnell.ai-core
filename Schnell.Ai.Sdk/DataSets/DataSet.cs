using Schnell.Ai.Sdk.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Sdk.DataSets
{
    /// <summary>
    /// DataSet for handling (reading and writing) data
    /// </summary>
    public class DataSet
    {
        /// <summary>
        /// True, if there are unwritten changes
        /// </summary>
        public bool Uncommitted { get; private set; }

        private IEnumerable<IDictionary<string,object>> _content { get; set; }

        /// <summary>
        /// Name of DataSet
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Array of importers
        /// </summary>
        internal IDataSetImporter[] Importer { get; set; }

        /// <summary>
        /// Exporter for the data
        /// </summary>
        internal IDataSetExporter Exporter { get; set; }

        /// <summary>
        /// Definition of data-fields
        /// </summary>
        public IEnumerable<Definitions.FieldDefinition> FieldDefinitions { get; internal set; }


        /// <summary>
        /// Read content for dataset
        /// </summary>
        /// <returns>Enumerable of Data</returns>
        public async Task<IEnumerable<IDictionary<string, object>>> GetContent()
        {
            if (_content == null)
            {
                if (Importer != null)
                {
                    IEnumerable<IDictionary<string, object>> data = null;
                    Importer.ToList().ForEach(async i =>
                    {
                        (Exporter as DataSetImporter)?.Log?.Progress();
                        var idata = await i.Import(this);
                        if (data == null)
                            data = idata;
                        else
                            data = data.Concat(idata);
                        (Exporter as DataSetImporter)?.Log?.Progress();                        
                    });

                    _content = data;
                }
            }

            return _content;
        }

        /// <summary>
        /// Set data content for DataSet
        /// </summary>
        /// <param name="data">Enumerable of Data as Dictionary</param>
        /// <returns>Task</returns>
        public async Task SetContent(IEnumerable<IDictionary<string, object>> data)
        {
            Uncommitted = true;
            _content = data;            
        }

        /// <summary>
        /// Write data to exporter
        /// </summary>
        /// <returns></returns>
        public async Task Commit()
        {
            if(Uncommitted && Exporter != null)
            {
                (Exporter as DataSetExporter)?.Log?.Progress();
                await Exporter.Export(this, _content);
                (Exporter as DataSetExporter)?.Log?.ProgressCompleted();
                Uncommitted = false;
            }
        }
    }

    /// <summary>
    /// Importer/Reader for a DataSet
    /// </summary>
    public abstract class DataSetImporter : IDataSetImporter
    {
        internal protected DataSetHandlerDefinition Definition { get; internal set; }
        internal protected Logging.Logger Log { get; private set; }

        /// <summary>
        /// Import Data for DataSet
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public abstract Task<IEnumerable<IDictionary<string, object>>> Import(DataSet ds);

        /// <summary>
        /// Configuration-Handler for DataSet-Importer
        /// </summary>
        public abstract Configuration.IConfigurationHandler ConfigurationHandler { get; }

        protected virtual void OnBuilt()
        {

        }

        internal async Task Build(DataSetHandlerDefinition definition, Logging.Logger logger)
        {
            this.Log = logger;
            this.Definition = definition;
            this.OnBuilt();
        }
    }

    /// <summary>
    /// Exporter for DataSet
    /// </summary>
    public abstract class DataSetExporter : IDataSetExporter
    {
        internal protected DataSetHandlerDefinition Definition { get; internal set; }
        internal protected Logging.Logger Log { get; private set; }

        /// <summary>
        /// Configuration-Dictionery for DataSet-Exporter
        /// </summary>
        protected IDictionary<string, object> Configuration { get; set; }

        /// <summary>
        /// Export data for DataSet
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract Task Export(DataSet ds, IEnumerable<IDictionary<string, object>> data);

        /// <summary>
        /// Configuration-Handler for DataSet-Exporter
        /// </summary>
        public abstract Configuration.IConfigurationHandler ConfigurationHandler { get; }

        /// <summary>
        /// Will be called after instantiate the Exporter
        /// </summary>
        protected virtual void OnBuilt()
        {

        }

        internal async Task Build(DataSetHandlerDefinition definition, Logging.Logger logger)
        {
            this.Log = logger;
            this.Definition = definition;
            this.OnBuilt();
        }
    }
}
