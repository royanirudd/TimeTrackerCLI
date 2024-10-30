using System;
using System.Collections.Generic;

namespace TimeTracker.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
        public List<ActivityLog> Activities { get; set; } = new List<ActivityLog>();
        public bool IsAutomated { get; set; }
        public DateTime? ScheduledStartTime { get; set; }

        public TimeSpan Duration => EndTime.HasValue
            ? EndTime.Value - StartTime
            : DateTime.Now - StartTime;
    }
}
