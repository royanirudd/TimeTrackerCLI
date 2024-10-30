using System;
using System.Collections.Generic;

namespace TimeTracker.Models
{
    public class Session
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<string, TimeSpan> ApplicationTimes { get; set; } = new();
        public Dictionary<string, TimeSpan> FileTimes { get; set; } = new();
    }
}
