using Schnell.Ai.Sdk.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Schnell.Ai.Sdk.Utilities
{
    /// <summary>
    /// Creates a new typed dictionary based on the field-definitions
    /// </summary>
    public static class FieldDefinitionToTypeCompiler
    {
        /// <summary>
        /// Create a new type
        /// </summary>
        /// <param name="fieldDefinitions">Field-Definitions</param>
        /// <param name="typeName">Name of the newly created type</param>
        /// <returns></returns>
        public static TypeInfo CreateTypeFromFielDefinitions(IEnumerable<FieldDefinition> fieldDefinitions, string typeName)
        {
            var dict = BuildPrototype(fieldDefinitions);
            return Shared.Helper.TypeExtensions.CreateNewType(dict, typeName);
        }

        private static Dictionary<string, object> BuildPrototype(IEnumerable<FieldDefinition> fieldDefinitions)
        {
            var dict = new Dictionary<string, object>();
            fieldDefinitions.ToList().ForEach(f =>
            {
                object obj = null;
                switch(f.ValueType)
                {
                    case FieldDefinition.ValueTypeEnum.String: obj = String.Empty; break;
                    case FieldDefinition.ValueTypeEnum.Boolean: obj = false; break;
                    case FieldDefinition.ValueTypeEnum.Float: obj = (float)1.0; break;
                    case FieldDefinition.ValueTypeEnum.Integer: obj = (int)1; break;
                    case FieldDefinition.ValueTypeEnum.Unspecified:
                    default: obj = null; break;                        
                }
                dict.Add(f.Name, obj);
            }            
            );
            return dict;
        }
    }
}
