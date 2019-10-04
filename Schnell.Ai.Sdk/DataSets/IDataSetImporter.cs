using System.Collections.Generic;
using System.Threading.Tasks;
using Schnell.Ai.Sdk.Configuration;

namespace Schnell.Ai.Sdk.DataSets
{
    /// <summary>
    /// Interface for DataSet-Importer
    /// </summary>
    public interface IDataSetImporter
    {
        /// <summary>
        /// Configuration-handler for importer
        /// </summary>
        IConfigurationHandler ConfigurationHandler { get; }

        /// <summary>
        /// Import data for dataset
        /// </summary>
        /// <param name="ds">Dataset</param>
        /// <returns>Enumerable of Dictionaries</returns>
        Task<IEnumerable<IDictionary<string, object>>> Import(DataSet ds);
    }    
}