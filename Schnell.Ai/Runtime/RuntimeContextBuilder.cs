using Microsoft.Extensions.Logging;
using Schnell.Ai.Sdk.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schnell.Ai.Runtime
{
    internal class RuntimeContextBuilder
    {
        internal IDictionary<string, object> Variables { get; private set; } = new Dictionary<string, object>();

        internal void SetVariables(IDictionary<string, object> dict)
        {
            if (dict != null)
            {
                dict.ToList().ForEach(kvp =>
                {
                    if (this.Variables.ContainsKey(kvp.Key))
                        this.Variables[kvp.Key] = kvp.Value;
                    else
                        this.Variables.Add(kvp.Key, kvp.Value);
                });
            }
        }

        internal RuntimeContext Create(string file, Logger log)
        {
            var projectFile = ReplaceVariables(System.IO.File.ReadAllText(file));
            
            var definition = Shared.Helper.Json.Deserialize<Sdk.Definitions.AiProjectDefinition>(projectFile);

            if (definition == null)
                throw new ArgumentException("File is not a valid project-definition");

            return new RuntimeContext(definition, log);
        }

        private string ReplaceVariables(string json)
        {
            const string head = "{";
            const string tail = "}";

            var fc = json;

            this.Variables.ToList().ForEach(v => {
                fc = fc.Replace($"{head}{v.Key}{tail}", v.Value.ToString());
            });

            return fc;
        }
    }
}
