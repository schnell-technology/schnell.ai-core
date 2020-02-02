using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schnell.Ai.CliCommands
{
    internal class CliCommandRegister
    {
        /// <summary>
        /// Registers available Cli-Commands
        /// </summary>
        /// <param name="app"></param>
        internal virtual void Register(CommandLineApplication app)
        {
            Project.Register(app);
            Repository.Register(app);
            Runner.Register(app);
            Module.Register(app);         
            Development.Register(app);
            Common.Register(app);   
        }
    }
}
