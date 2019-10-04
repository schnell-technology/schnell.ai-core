using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Sdk
{
    public interface IRunContext
    {
        Logging.Logger Log { get; }
        Task Run(PipelineContext context);
    }
}
