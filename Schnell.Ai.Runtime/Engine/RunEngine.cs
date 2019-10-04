using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Schnell.Ai.Sdk;
using Schnell.Ai.Sdk.Logging;

namespace Schnell.Ai.Runtime.Engine
{
    /// <summary>
    /// Runner for a pipeline-context
    /// </summary>
    public class RunEngine : Sdk.IRunContext
    {
        public Logger Log { get; set; }

        /// <summary>
        /// Start a pipeline
        /// </summary>
        /// <param name="context">Pipeline-context</param>
        /// <returns>Task</returns>
        public async Task Run(PipelineContext context)
        {
            foreach (var stage in context.Definition.Stages)
            {
                if (context.AiContext.Status == AiContextState.ErrorState)
                {
                    this.Log.Write(LogEntry.LogType.Info, $"Context is in error state - execution will be aborted.");
                    return;
                }

                try
                {
                    if (!String.IsNullOrEmpty(stage.RunAction))
                    {
                        this.Log.Write(LogEntry.LogType.Info, $"Run Action '{stage.RunAction}'");
                        await RunAction(context, stage.RunAction);
                    }
                    await CommitDataSets(context);
                }
                catch (Exception ex)
                {
                    this.Log.Write(LogEntry.LogType.Fatal, ex.Message);
                }
            }

            this.Log.Write(LogEntry.LogType.Info, $"Pipeline has been executed");
        }

        private async Task RunAction(PipelineContext context, string name)
        {
            await context.Actions.ProcessAction(name);
        }

        private async Task CommitDataSets(PipelineContext context)
        {
            foreach (var ds in context.Artifacts.DataSets)
            {
                if (ds.Uncommitted)
                {
                    this.Log.Write(LogEntry.LogType.Info, $"Commit changes to dataset: '{ds.Name}'");
                    await ds.Commit();
                }
            }
        }
    }
}
