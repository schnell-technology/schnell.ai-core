using Newtonsoft.Json.Schema;
using Schnell.Ai.Sdk.Configuration;
using Schnell.Ai.Sdk.Definitions;
using Schnell.Ai.Sdk.Interfaces;
using Schnell.Ai.Sdk.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Sdk.Actions
{
    /// <summary>
    /// Action to process data in a pipeline
    /// </summary>
    public interface IActionBase
    {
        /// <summary>
        /// Name of action
        /// </summary>
        string Name { get; }    

        /// <summary>
        /// Configuration for the action
        /// </summary>
        object Configuration { get; }

        /// <summary>
        /// Process data
        /// </summary>
        /// <param name="context">Pipeline-context</param>
        /// <returns>Task</returns>
        Task Process(PipelineContext context);        

    }

    public abstract class ActionBase : IActionBase, IDisposable
    {
        /// <summary>
        /// Name of action
        /// </summary>
        public string Name { get { return Definition?.Name; } }
        protected internal Logging.Logger Log { get; private set; }
        internal protected Definitions.ProcessorDefinition Definition { get; private set; }
        public abstract IConfigurationHandler Configuration { get; }
        object IActionBase.Configuration => Configuration;

        /// <summary>
        /// Process-action
        /// </summary>
        /// <param name="context">Pipeline-context</param>
        /// <returns>Task</returns>
        public abstract Task Process(PipelineContext context);
        protected virtual void OnBuilt() { }

        public virtual void Dispose()
        {
            this.Definition = null;
        }
        internal void Build(Definitions.ProcessorDefinition definition, Logging.Logger logger)
        {
            this.Definition = definition;
            this.Log = logger;
            
            OnBuilt();

            if (this.Configuration != null)
            {
                var errors = this.Configuration.ValidateConfiguration();
                if (errors.Any())
                {
                    this.Log.Write(LogEntry.LogType.Warning, $"Configuration for action '{this.Name}' is not fully valid.");
                }
            }
        }

    }

   
}
