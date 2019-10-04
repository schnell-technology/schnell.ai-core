using System.Collections.Generic;
using System.Threading.Tasks;
using Schnell.Ai.Sdk.Configuration;

namespace Schnell.Ai.Sdk.DataSets
{
    /// <summary>
    /// Interface for DataSet-Exporter
    /// </summary>
    public interface IDataSetExporter
    {
        /// <summary>
        /// Configuration-Handler for the exporter
        /// </summary>
        IConfigurationHandler ConfigurationHandler { get; }

        /// <summary>
        /// Export data for dataset
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <param name="data">Data as enumerated dictionaries</param>
        /// <returns>Task</returns>
        Task Export(DataSet ds, IEnumerable<IDictionary<string, object>> data);
    }
}