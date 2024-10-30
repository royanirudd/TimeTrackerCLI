using System;
using System.Collections.Generic;
using TimeTracker.Models;

namespace TimeTracker.Interfaces
{
    public interface ISessionManager
    {
        Session StartSession(string name);
        bool StopSession(Guid sessionId);
        Session GetSession(Guid sessionId);
        IEnumerable<Session> GetActiveSessions();
        void UpdateSessionStats(Guid sessionId, string application, string file);
    }
}
