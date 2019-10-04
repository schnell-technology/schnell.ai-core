using Newtonsoft.Json;
using Schnell.Ai.Sdk.Actions;
using Schnell.Ai.Sdk.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Sdk.Components
{
    /// <summary>
    /// Configuration for a generic model-tester
    /// </summary>
    [DataContract]
    public class TesterConfiguration
    {
        /// <summary>
        /// Name of model
        /// </summary>
        [DataMember(Name = "Model")]
        [JsonProperty(Required = Required.Always)]
        public string Model { get; set; }

        /// <summary>
        /// DataSet-Name of data containing test-entries
        /// </summary>
        [DataMember(Name = "TestData")]
        [JsonProperty(Required = Required.Always)]
        public string TestData { get; set; }

        /// <summary>
        /// DataSet-Name for exporting test-result data
        /// </summary>
        [DataMember(Name = "TestResultData")]
        public string TestResultData { get; set; }
    }


    /// <summary>
    /// Base class for tester
    /// </summary>
    public abstract class TesterBase : Sdk.Actions.ActionBase
    {
        protected ConfigurationHandler<TesterConfiguration> ConfigurationHandler { get; set; }
        public override IConfigurationHandler Configuration => ConfigurationHandler;

        public TestResult CurrentTestResult { get; private set; }

        protected TesterBase()
        {
                                
        }

        protected override void OnBuilt()
        {
            base.OnBuilt();
            ConfigurationHandler = new ConfigurationHandler<TesterConfiguration>(this.Definition);
        }

        public override async Task Process(PipelineContext context)
        {
            var result = await ProcessTest(context);
            CurrentTestResult = result;
            if (!String.IsNullOrEmpty(ConfigurationHandler.Configuration.TestResultData))
            {
                var testResultData = context.Artifacts.GetDataSet(ConfigurationHandler.Configuration.TestResultData);
                var results = new List<IDictionary<string, object>>();
                results.Add(Schnell.Ai.Shared.Helper.ObjectDictionaryMapper.GetDictionary<TestResult>(CurrentTestResult));
                await testResultData.SetContent(results);
            }
        }

        /// <summary>
        /// Task to process the test
        /// </summary>
        /// <param name="context">Pipeline-context</param>
        /// <returns>Task of TestResult</returns>
        protected abstract Task<TestResult> ProcessTest(PipelineContext context);
        
    }

    /// <summary>
    /// Test-Result of a test-run
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Timestamp of result
        /// </summary>
        public DateTime Timestamp { get; private set; } = DateTime.Now;

        /// <summary>
        /// Score of test (1 is best, 0 is lowest score)
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// Amount of tested data
        /// </summary>
        public float TestDataLength { get; set; }

        /// <summary>
        /// Additional results and metrics
        /// </summary>
        public Dictionary<string, object> DetailResults { get; private set; } = new Dictionary<string, object>();
    }
}
