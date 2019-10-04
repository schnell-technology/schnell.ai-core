using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Schnell.Ai.Shared.Helper
{
    public class Json
    {
        private static JsonSerializerSettings _settings;

        private static JsonSerializerSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new JsonSerializerSettings()
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        TypeNameHandling = TypeNameHandling.None,
                        Formatting = Formatting.Indented
                    };
                }
                return _settings;
            }
        }

        /// <summary>
        /// Get a JSON-schema for a type
        /// </summary>
        /// <param name="t">Type to create a JSON-schema for</param>
        /// <returns>Json-schema</returns>
        public static JsonSchema GetJsonSchema(Type t)
        {
            if (t != null)
            {
                var schema = JsonSchema.FromType(t, new NJsonSchema.Generation.JsonSchemaGeneratorSettings() { FlattenInheritanceHierarchy=true });
                var jschema = schema.ToJson(Formatting.Indented);
                return schema;
            }
            return null;
        }

        /// <summary>
        /// Validate a object against a JSON-schema
        /// </summary>
        /// <param name="obj">Object-instance</param>
        /// <param name="schema">Json-schema</param>
        /// <returns>Validation-errors</returns>
        public static ICollection<NJsonSchema.Validation.ValidationError> ValidateObjectWithSchema(object obj, JsonSchema schema)
        {
            if (obj != null)
            {
                if (schema != null)
                {
                    var json = JsonConvert.SerializeObject(obj);
                    return ValidateJsonWithSchema(json, schema);
                }
            }
            return new List<NJsonSchema.Validation.ValidationError>();
        }

        /// <summary>
        /// Validate a JSON-string against a JSON-schema
        /// </summary>
        /// <param name="json">JSON-string</param>
        /// <param name="schema">JSON-schema</param>
        /// <returns>Validation-errors</returns>
        public static ICollection<NJsonSchema.Validation.ValidationError> ValidateJsonWithSchema(string json, JsonSchema schema)
        {
            if (json != null)
            {
                if (schema != null)
                {
                    var errors = schema.Validate(json);
                    return errors;
                }
            }
            return new List<NJsonSchema.Validation.ValidationError>();
        }

        /// <summary>
        /// Deserialize a JSON-string to a typed object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="json">JSON-string</param>
        /// <param name="settings">Settings for serialization</param>
        /// <returns>Object of type T</returns>
        public static T Deserialize<T>(string json, JsonSerializerSettings settings = null)
        {
            if (settings == null)
                settings = Settings;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// Serialize a object-instance to a JSON-string
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="obj">Object-instance</param>
        /// <param name="settings">Settings for serialization</param>
        /// <returns>JSON-string</returns>
        public static string Serialize<T>(T obj, JsonSerializerSettings settings = null)
        {
            if (settings == null)
                settings = Settings;
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, settings);
        }
    }
}
