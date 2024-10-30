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
        public Dictionary<string, TimeSpan> ApplicationTimes { get; set; }
        public Dictionary<string, TimeSpan> FileTimes { get; set; }

        public Session(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            StartTime = DateTime.Now;
            IsActive = true;
            ApplicationTimes = new Dictionary<string, TimeSpan>();
            FileTimes = new Dictionary<string, TimeSpan>();
        }
    }
}
