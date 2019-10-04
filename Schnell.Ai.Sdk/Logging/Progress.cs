using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Schnell.Ai.Sdk.Logging
{
    /// <summary>
    /// Progress of a action
    /// </summary>
    [DataContract]
    public class Progress
    {
        /// <summary>
        /// Name of the logger
        /// </summary>
        [DataMember(Name="LoggerName")]
        public string LoggerName { get; set; }

        /// <summary>
        /// Action, which is processing
        /// </summary>
        [DataMember(Name = "Action")]
        public string Action { get; set; }

        /// <summary>
        /// Current value of the process
        /// </summary>
        [DataMember(Name = "CurrentValue")]
        public int? CurrentValue { get; set; }

        /// <summary>
        /// Maximum value of the process
        /// </summary>
        [DataMember(Name = "MaximumValue")]
        public int? MaximumValue { get; set; }

        /// <summary>
        /// Begin-timestamp
        /// </summary>
        [DataMember(Name = "Start")]
        public DateTime Start { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Completed-timestamp
        /// </summary>
        [DataMember(Name = "Completed")]
        public DateTime? Completed { get; set; }

        /// <summary>
        /// Level of logger
        /// </summary>
        [DataMember(Name = "Level")]
        public int Level { get; set; }
    }
}
