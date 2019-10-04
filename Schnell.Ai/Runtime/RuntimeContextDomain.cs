using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schnell.Ai.Runtime
{
    internal static class RuntimeContextDomain
    {
        private static List<RuntimeContext> _contexts = new List<RuntimeContext>();

        internal static void RegisterContext(RuntimeContext context)
        {
            _contexts.Add(context);
        }

        internal static RuntimeContext GetRuntimeContext(string file = null)
        {
            return _contexts.FirstOrDefault();
        }
    }
}
