using System;

namespace TimeTracker.Models
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public string ApplicationName { get; set; }
        public string FilePath { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration => EndTime.HasValue 
            ? EndTime.Value - StartTime 
            : DateTime.Now - StartTime;
    }
}
