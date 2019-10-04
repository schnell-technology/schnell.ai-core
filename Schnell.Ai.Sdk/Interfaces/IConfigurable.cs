using System;
using System.Collections.Generic;
using System.Text;

namespace Schnell.Ai.Sdk.Interfaces
{
    public interface IConfigurable
    {
        IDictionary<string, object> Configuration { get; set; }
    }
}
