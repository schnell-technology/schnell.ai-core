using Schnell.Ai.Sdk.DataSets;
using Schnell.Ai.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Sdk
{
    /// <summary>
    /// Context
    /// </summary>
    public abstract class AiContext
    {
        /// <summary>
        /// Project-definition
        /// </summary>
        public Definitions.AiProjectDefinition Definition { get; private set; }

        /// <summary>
        /// Pipeline-context
        /// </summary>
        protected PipelineContext Pipeline { get; private set; }

        private IRunContext _runContext;
        private Logging.Logger _log;

        private AiContextState _status;

        /// <summary>
        /// Status of the current context
        /// </summary>
        public AiContextState Status
        {
            get
            {
                if (Log?.Entries.Any(l => l.Type == Logging.LogEntry.LogType.Fatal) ?? false)
                    return AiContextState.ErrorState;
                else
                    return _status;
            }
        }

        /// <summary>
        /// Has errors while execution
        /// </summary>
        public bool HasErrors { get { return Log?.Entries.Any(l => l.Type == Logging.LogEntry.LogType.Error) ?? false; } }

        /// <summary>
        /// Has warnings while execution
        /// </summary>
        public bool HasWarnings { get { return Log?.Entries.Any(l => l.Type == Logging.LogEntry.LogType.Warning) ?? false; } }

        internal protected IRunContext RunContext { get { return _runContext; } }
        internal protected Logging.Logger Log { get { return _log; } }

        protected AiContext(Definitions.AiProjectDefinition definition, Logging.Logger log)
        {
            this._status = AiContextState.Idle;
            this.Definition = definition;
            this._log = log;
            this._runContext = CreateRunContext();
        }

        internal T CreateInstance<T>(string type, string name = null)
        {
            var instance = this.OnCreateInstance<T>(type);
            if (instance is INamed)
            {
                (instance as INamed).Name = name;
            }
            return instance;
        }

        protected abstract T OnCreateInstance<T>(string type);
        protected abstract Task InstallReferences();
        protected abstract IRunContext CreateRunContext();

        /// <summary>
        /// Build a new AiContext for specified pipeline
        /// </summary>
        /// <param name="pipeline">Pipeline-name</param>
        /// <returns>Context</returns>
        public AiContext Build(string pipeline)
        {
            return Build(this.Definition?.Pipelines?.FirstOrDefault(p => p.Name == pipeline));
        }

        /// <summary>
        /// Build a new AiContext for specified pipeline
        /// </summary>
        /// <param name="definition">Pipeline-definition</param>
        /// <returns>Context</returns>
        public AiContext Build(Definitions.PipelineDefinition definition)
        {
            if (this.Pipeline != null)
                throw new ArgumentException("Pipeline already set");

            if (definition == null)
                throw new ArgumentNullException("definition");

            this._status = AiContextState.PipelineBuild;
            this.Pipeline = new PipelineContext(this, definition);
            this._status = AiContextState.PipelineReady;

            return this;
        }

        protected virtual async Task Bootstrap()
        {
            this._status = AiContextState.Bootstrapping;
            await this.InstallReferences();
            await this.Pipeline.Bootstrap();
            this._status = AiContextState.Bootstrapped;
        }

        /// <summary>
        /// Run the context
        /// </summary>
        /// <returns></returns>
        public virtual async Task Run()
        {
            if (this.Pipeline == null)
                throw new Exception("Context isn't build right now");

            Log.Write(Logging.LogEntry.LogType.Info, "Bootstrapping artifacts and actions...");
            await this.Bootstrap();
            Log.Write(Logging.LogEntry.LogType.Info, "Bootstrapping completed.");
            Log.Write(Logging.LogEntry.LogType.Info, "Build and start pipeline...");
            this._status = AiContextState.Running;
            await this.RunContext.Run(Pipeline);
            Log.Write(Logging.LogEntry.LogType.Info, "Pipeline-Execution completed");
            if (this.HasErrors)
                this._status = AiContextState.Completed_WithErrors;
            else
                this._status = AiContextState.Completed_Success;

        }
    }

    /// <summary>
    /// Context, handling the artifacts for a pipeline
    /// </summary>
    public class ArtifactsContext
    {
        private PipelineContext _ctx;

        /// <summary>
        /// Enumerable of available models
        /// </summary>
        public IEnumerable<Definitions.ModelDefinition> Models { get; private set; }
        /// <summary>
        /// Enumerable of available data-sets
        /// </summary>
        public IEnumerable<DataSets.DataSet> DataSets { get; private set; }

        internal ArtifactsContext(PipelineContext context)
        {
            this._ctx = context;
            this.Models = context.AiContext.Definition.Artifacts?.Models;
        }

        internal async Task BuildDataSets()
        {
            var dsl = new List<DataSets.DataSet>();
            foreach (var def in _ctx.AiContext.Definition.Artifacts?.DataSets)
            {
                var ds = new DataSets.DataSet();
                ds.Name = def.Name;
                ds.FieldDefinitions = def.FieldDefinition;

                if (def.Exporter != null)
                {
                    ds.Exporter = _ctx.AiContext.CreateInstance<Sdk.DataSets.IDataSetExporter>(def.Exporter.Type);
                    if (ds.Exporter is DataSetExporter dsi)
                        await dsi.Build(def.Exporter, _ctx.AiContext.Log.CreateChild("exporter: " + ds.Name));
                }

                if (def.Importer != null)
                {
                    var importer = new List<IDataSetImporter>();

                    foreach (var i in def.Importer)
                    {
                        var iInstance = _ctx.AiContext.CreateInstance<Sdk.DataSets.IDataSetImporter>(i.Type);
                        importer.Add(iInstance);
                        if (iInstance is DataSetImporter dsi)
                            await dsi.Build(i, _ctx.AiContext.Log.CreateChild("importer: " + ds.Name));
                    }

                    ds.Importer = importer.ToArray();
                }

                dsl.Add(ds);
            }
            this.DataSets = dsl;
        }

        /// <summary>
        /// Get data-set
        /// </summary>
        /// <param name="name">Name of the data-set</param>
        /// <returns>Instance of data-set</returns>
        public DataSets.DataSet GetDataSet(string name)
        {
            return DataSets?.FirstOrDefault(d => d.Name == name);
        }

        /// <summary>
        /// Get model
        /// </summary>
        /// <param name="name">Name of the model</param>
        /// <returns>Instance of model</returns>
        public Definitions.ModelDefinition GetModel(string name)
        {
            return Models?.FirstOrDefault(d => d.Name == name);
        }
    }

    /// <summary>
    /// Context, handling the actions for a pipeline
    /// </summary>
    public class ActionsContext
    {
        private PipelineContext _ctx;

        private List<Actions.IActionBase> _processorActions = new List<Actions.IActionBase>();

        private IEnumerable<Actions.IActionBase> AllActions()
        {
            return _processorActions;
        }

        /// <summary>
        /// Create a new context, based on the pipeline-context
        /// </summary>
        /// <param name="context">Pipeline-context</param>
        public ActionsContext(PipelineContext context)
        {
            this._ctx = context;
        }

        internal async Task BuildActions()
        {
            var pipelineDefinition = this._ctx.Definition;
            var actionsDefinition = this._ctx.AiContext.Definition?.Actions;

            foreach (var stage in pipelineDefinition.Stages?.Where(s => s.RunAction != null).Select(s => s.RunAction).Distinct())
            {
                var processor = actionsDefinition.Processors.FirstOrDefault(t => t.Name == stage);
                if (processor != null)
                {
                    var instance = this._ctx.AiContext.CreateInstance<Actions.IActionBase>(processor.Type, processor.Name);
                    if (instance != null)
                    {
                        if (instance is Actions.ActionBase iab)
                            iab.Build(processor, _ctx.AiContext.Log.CreateChild("action: " + processor.Name));
                        _processorActions.Add(instance);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Run action
        /// </summary>
        /// <param name="name">Name of the action</param>
        /// <returns>Task</returns>
        public async Task ProcessAction(string name)
        {
            var action = AllActions().FirstOrDefault(a => a.Name == name);
            if (action != null)
            {
                (action as Actions.ActionBase)?.Log?.Progress();
                await action.Process(_ctx);
                (action as Actions.ActionBase)?.Log?.ProgressCompleted();
                return;
            }
            else
                throw new Exception($"A action with the name '{name}' could not be found.");

        }
    }

    /// <summary>
    /// Context of the pipeline
    /// </summary>
    public class PipelineContext
    {
        /// <summary>
        /// Name of the pipeline
        /// </summary>
        public string Name { get { return Definition?.Name; } }

        /// <summary>
        /// Ai-context
        /// </summary>
        public AiContext AiContext { get; private set; }

        /// <summary>
        /// Context, handling the artifacts
        /// </summary>
        public ArtifactsContext Artifacts { get; private set; }

        /// <summary>
        /// Context, handling the actions
        /// </summary>
        public ActionsContext Actions { get; private set; }

        /// <summary>
        /// Definition of the pipeline
        /// </summary>
        public Definitions.PipelineDefinition Definition { get; private set; }

        /// <summary>
        /// Instantiate a new context for a pipeline
        /// </summary>
        /// <param name="context">Ai-Context</param>
        /// <param name="pipe">Definition of the pipeline</param>
        public PipelineContext(AiContext context, Definitions.PipelineDefinition pipe)
        {
            this.AiContext = context;
            this.Definition = pipe;
            this.Artifacts = new ArtifactsContext(this);
            this.Actions = new ActionsContext(this);
        }

        internal async Task Bootstrap()
        {
            await this.Artifacts.BuildDataSets();
            await this.Actions.BuildActions();
        }
    }

    /// <summary>
    /// State of Ai-Context
    /// </summary>
    public enum AiContextState
    {
        Idle = 0,
        Bootstrapping = 10,
        Bootstrapped = 11,
        PipelineBuild = 20,
        PipelineReady = 21,
        Running = 30,
        Completed_Success = 31,
        Completed_WithErrors = 32,
        ErrorState = 99
    }
}
