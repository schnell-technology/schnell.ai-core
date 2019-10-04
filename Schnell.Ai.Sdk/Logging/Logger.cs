using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schnell.Ai.Sdk.Logging
{
    /// <summary>
    /// Logging-mechanism
    /// </summary>
    public class Logger
    {
        private List<Logger> ChildLogger { get; set; } = new List<Logger>();
        private List<LogEntry> _entries { get; set; } = new List<LogEntry>();
        private List<Progress> _progresses { get; set; } = new List<Progress>();
        private int Level { get; set; }

        internal string Name { get; private set; }
        internal Logger ParentLogger { get; private set; }
        
        internal List<Action<LogEntry>> HistoryReaderInterceptor { get; private set; } = new List<Action<LogEntry>>();

        internal void RegisterChild(Logger log)
        {
            this.ChildLogger.Add(log);
        }

        /// <summary>
        /// Enumerable of log-entries
        /// </summary>
        public IEnumerable<LogEntry> Entries { get {
                var entries = _entries.AsEnumerable();

                ChildLogger.ForEach(l => entries = entries.Concat(l.Entries));

                return entries;
                }
        }

        public Logger(string name, Logger parent = null)
        {
            this.Name = name;
            this.ParentLogger = parent;
            this.ParentLogger?.RegisterChild(this);

            Level = GetLevel();
        }

        private int GetLevel()
        {
            int lvl = 0;
            var current = ParentLogger;
            while(current != null)
            {
                current = current.ParentLogger;
                lvl = lvl + 1;
            }
            return lvl;
        }

        private void WriteEntry(LogEntry entry)
        {
            _entries.Add(entry);
            OnEntryWritten(entry, this);            
        }

        /// <summary>
        /// Write log entry
        /// </summary>
        /// <param name="type">Type of log-entry</param>
        /// <param name="message">Log message</param>
        public void Write(LogEntry.LogType type, string message)
        {
            WriteEntry(new LogEntry() { Type = type, Timestamp = DateTime.UtcNow, Message = message, LoggerName = this.Name, Level = this.Level});
        }

        /// <summary>
        /// Write process
        /// </summary>
        /// <param name="action">Action, which is processing</param>
        /// <param name="currentValue">Current value of process</param>
        /// <param name="maximumValue">Maximum value of process</param>
        public void Progress(string action = null, int? currentValue = null, int? maximumValue = null)
        {
            var prog = _progresses.FirstOrDefault(p => p.LoggerName == this.Name && p.Action == action);

            if (prog == null)
            {
                prog = new Progress() { LoggerName = this.Name, Action = action };
                this._progresses.Add(prog);
            }
            prog.Level = this.Level;

            if(currentValue != null)
            prog.CurrentValue = currentValue;

            if(maximumValue != null)
            prog.MaximumValue = maximumValue;

            OnProgressChanged(prog, this);
            
        }

        /// <summary>
        /// Report a completed process
        /// </summary>
        /// <param name="action">Action, which is now completed</param>
        public void ProgressCompleted(string action = null)
        {
            var prog = _progresses.FirstOrDefault(p => p.LoggerName == this.Name && p.Action == action);

            if (prog == null)
            {
                prog = new Progress() { LoggerName = this.Name, Action = action };
                this._progresses.Add(prog);
            }

            prog.Completed = DateTime.UtcNow;
            OnProgressChanged(prog, this);
        }

        protected virtual void OnEntryWritten(LogEntry entry, Logger log)
        {
            ParentLogger?.OnEntryWritten(entry, this);
        }

        protected virtual void OnProgressChanged(Progress progress, Logger log)
        {
            ParentLogger?.OnProgressChanged(progress, this);
        }

        /// <summary>
        /// Create child-logger
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Logger CreateChild(string name)
        {
            return new Logger(name, this);            
        }

        
    }
}
