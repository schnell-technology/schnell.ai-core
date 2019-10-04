using Schnell.Ai.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Schnell.Ai.Sdk.Definitions
{
    /// <summary>
    /// Artifacts describes models and datasets to be used in a project
    /// </summary>
    [DataContract]
    public class ArtifactsDefinition
    {
        [DataMember(Name = "Models")]
        public IEnumerable<ModelDefinition> Models { get; set; } = new List<ModelDefinition>();

        [DataMember(Name = "DataSets")]
        public IEnumerable<DataSetDefinition> DataSets { get; set; } = new List<DataSetDefinition>();
    }

    /// <summary>
    /// Trained machine-learning model stored as a file
    /// </summary>
    [DataContract]
    public class ModelDefinition : INamed
    {
        /// <summary>
        /// Name of the model
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Path to the trained model
        /// </summary>
        [DataMember(Name = "Filepath")]
        public string Filepath { get; set; }
    }

    /// <summary>
    /// DataSet which can read and write data from or to a specified 
    /// </summary>
    [DataContract]
    public class DataSetDefinition : INamed
    {
        /// <summary>
        /// Name of DataSet
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Importers for the DataSet
        /// </summary>
        [DataMember(Name = "Importer")]
        public DataSetHandlerDefinition[] Importer { get; set; }

        /// <summary>
        /// Exporter for the DataSet
        /// </summary>
        [DataMember(Name = "Exporter")]
        public DataSetHandlerDefinition Exporter { get; set; }

        /// <summary>
        /// Definition of data-fields
        /// </summary>
        [DataMember(Name = "FieldDefinition")]
        public IEnumerable<FieldDefinition> FieldDefinition { get; set; }
    }

    /// <summary>
    /// Configuration of the data-set handler
    /// </summary>
    [DataContract]
    public class DataSetHandlerDefinition : ITyped, IConfigurable
    {
        /// <summary>
        /// Type of the handler
        /// </summary>
        [DataMember(Name = "Type")]
        public string Type { get; set; }

        /// <summary>
        /// Configuration
        /// </summary>
        [DataMember(Name = "Configuration")]
        public IDictionary<string,object> Configuration { get; set; }
    }

    /// <summary>
    /// Data-field definition
    /// </summary>
    [DataContract]
    public class FieldDefinition : INamed
    {
        /// <summary>
        /// Usage-type of field
        /// </summary>
        [DataContract]
        public enum FieldTypeEnum
        {
            /// <summary>
            /// No type set
            /// </summary>
            Unspecified = 0,

            /// <summary>
            /// Field is used as a label/class
            /// </summary>
            Label = 1,

            /// <summary>
            /// Field should be used to train or evaluate with
            /// </summary>
            Feature = 2,

            /// <summary>
            /// Result score of a evaluation or test
            /// </summary>
            Score = 3
        }

        /// <summary>
        /// Type of the values
        /// </summary>
        [DataContract]
        public enum ValueTypeEnum
        {
            /// <summary>
            /// Unspecified type
            /// </summary>
            Unspecified = 0,

            /// <summary>
            /// String/Text-Value
            /// </summary>
            String = 1,

            /// <summary>
            /// Numeric integer value
            /// </summary>
            Integer = 2,

            /// <summary>
            /// Floating point number
            /// </summary>
            Float = 3,

            /// <summary>
            /// Boolean value (true/false)
            /// </summary>
            Boolean = 4
        }

        /// <summary>
        /// Name of field
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Type of value
        /// </summary>
        [DataMember(Name = "ValueType")]
        public ValueTypeEnum ValueType { get; set; }

        /// <summary>
        /// Usage-type of the field
        /// </summary>
        [DataMember(Name = "FieldType")]
        public FieldTypeEnum FieldType { get; set; }
    }
}
