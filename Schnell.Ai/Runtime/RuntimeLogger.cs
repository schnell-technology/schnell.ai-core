using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Schnell.Ai.Sdk.Logging;

namespace Schnell.Ai.Runtime
{
    internal class RuntimeLogger : Sdk.Logging.Logger
    {
        public RuntimeLogger(string name) : base(name, null)
        {

        }

        protected override void OnEntryWritten(LogEntry entry, Logger log)
        {
            base.OnEntryWritten(entry, log);

            var col = System.Drawing.Color.White;
            var statusabbr = "";
            switch(entry.Type)
            {
                case LogEntry.LogType.Debug: col = System.Drawing.Color.Gray; statusabbr = "[DBG]"; break;
                case LogEntry.LogType.Info: col = System.Drawing.Color.White; statusabbr = "[INF]"; break;
                case LogEntry.LogType.Warning: col = System.Drawing.Color.Orange; statusabbr = "[WRN]"; break;
                case LogEntry.LogType.Error: col = System.Drawing.Color.Red; statusabbr = "[ERR]"; break;
                case LogEntry.LogType.Fatal: col = System.Drawing.Color.Magenta; statusabbr = "[!!!]"; break;
                default: col = System.Drawing.Color.White; break;
            }

            var indent = "";
            for(int i = 0;i<entry.Level;i++)
            {
                indent = indent + "\t";
            }

            Colorful.Console.WriteLine($"{indent}{entry.LoggerName} {statusabbr} {entry.Timestamp.ToString("HH:mm:ss")} - {entry.Message}", col);
        }

        protected override void OnProgressChanged(Progress progress, Logger log)
        {
            base.OnProgressChanged(progress, log);
            var indent = "";
            for (int i = 0; i < progress.Level; i++)
            {
                indent = indent + "\t";
            }

            if (progress.CurrentValue == null && progress.Completed==null)
            {
                Colorful.Console.WriteLine($"{indent}{progress.LoggerName} START  {progress.Start.ToString("HH:mm:ss")}", System.Drawing.Color.LightBlue);
            }

            if(progress.CurrentValue != null && progress.Completed==null)
            {
                Colorful.Console.WriteLine($"{indent}{progress.LoggerName} PROC  {progress.CurrentValue}", System.Drawing.Color.LightBlue);
            }

            if (progress.CurrentValue == null && progress.Completed != null)
            {
                Colorful.Console.WriteLine($"{indent}{progress.LoggerName} FINISH {(progress.Completed - progress.Start).Value.TotalMilliseconds.ToString("0.00")} ms", System.Drawing.Color.LightGreen);
            }

        }
    }
}
