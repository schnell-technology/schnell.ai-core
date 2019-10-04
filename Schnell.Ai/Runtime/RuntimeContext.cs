using Schnell.Ai.Sdk;
using Schnell.Ai.Sdk.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schnell.Ai.Runtime
{
    internal class RuntimeContext : Sdk.AiContext
    {
        internal IEnumerable<LogEntry> LogEntries { get
            {
                return this.Log.Entries;
            }
        }
        
        internal string PipelineName { get
            {
                return this.Pipeline.Name;
            } }

        

        public static RuntimeContext Create(Sdk.Definitions.AiProjectDefinition definition, Logger log)
        {
            return new RuntimeContext(definition, log);
        }

        internal RuntimeContext(Sdk.Definitions.AiProjectDefinition definition, Logger log) : base(definition, log)
        {
            RuntimeContextDomain.RegisterContext(this);
        }

        protected override async Task InstallReferences()
        {
            await Runtime.ReferenceLoader.LoadReferences(Log.CreateChild("reference-loader"), this);
        }

        protected override T OnCreateInstance<T>(string type)
        {            
            return Runtime.ReferenceLoader.CreateInstance<T>(type);
        }

        protected override IRunContext CreateRunContext()
        {
            return new Runtime.Engine.RunEngine() { Log = this.Log.CreateChild("runner") };
        }
    }
}
