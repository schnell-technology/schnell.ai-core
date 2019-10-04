using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schnell.Ai.Sdk;

namespace Schnell.Ai.Modules.Basic.Evaluators
{
    public class DummyEvaluator : Sdk.Components.EvaluatorBase
    {        
        public override async Task Process(PipelineContext context)
        {
            var model = context.Artifacts.GetModel(this.ConfigurationHandler.Configuration.Model);
            var inputData = await(context.Artifacts.GetDataSet(this.ConfigurationHandler.Configuration.InputData).GetContent());
            var testResultData = context.Artifacts.GetDataSet(this.ConfigurationHandler.Configuration.ResultData);


            this.Log.Write(Sdk.Logging.LogEntry.LogType.Info, inputData.ToList().Count + " Entries read");

            await testResultData?.SetContent(this.GetResults(context, inputData));
        }

        private IEnumerable<IDictionary<string,object>> GetResults(PipelineContext context, IEnumerable<IDictionary<string,object>> input)
        {            
            foreach(var record in input)
            {
                var result = new Dictionary<string, object>();
                foreach(var f in context.Artifacts.GetDataSet(this.ConfigurationHandler.Configuration.ResultData).FieldDefinitions)
                {
                    if (f.FieldType == Sdk.Definitions.FieldDefinition.FieldTypeEnum.Score)
                    {
                        result[f.Name] = DateTime.Now.Ticks;
                    }
                    else
                    {
                        result[f.Name] = record[f.Name];
                    }
                }
                yield return result;
            }
        }
    }
}
