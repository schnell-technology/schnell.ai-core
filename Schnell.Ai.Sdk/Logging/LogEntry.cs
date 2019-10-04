using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Schnell.Ai.Sdk.Logging
{
    /// <summary>
    /// Log entry
    /// </summary>
    [DataContract]
    public class LogEntry
    {
        /// <summary>
        /// Type of log entry
        /// </summary>
        [DataContract]
        public enum LogType
        {
            /// <summary>
            /// Debug message
            /// </summary>
            Debug = 0,

            /// <summary>
            /// Information
            /// </summary>
            Info = 10,

            /// <summary>
            /// Warning
            /// </summary>
            Warning = 50,

            /// <summary>
            /// Error, but no cancellation of the pipeline
            /// </summary>
            Error = 90,

            /// <summary>
            /// Fatal error occured - pipeline will be terminated
            /// </summary>
            Fatal = 99
        }

        /// <summary>
        /// Type of log entry
        /// </summary>
        [DataMember(Name ="Type")]
        public LogType Type { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        [DataMember(Name = "Timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Log message
        /// </summary>
        [DataMember(Name = "Message")]
        public string Message { get; set; }

        /// <summary>
        /// Name of the logger (log-context)
        /// </summary>
        [DataMember(Name = "LoggerName")]
        public string LoggerName { get; set; }

        /// <summary>
        /// Level of logger
        /// </summary>
        [DataMember(Name = "Level")]
        public int Level { get; set; }
    }
}
