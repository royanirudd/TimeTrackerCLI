using System;
using System.Collections.Generic;
using TimeTracker.Models;

namespace TimeTracker.Interfaces
{
    public interface ISessionManager
    {
        Session StartSession(string name);
        bool StopSession(Guid id);
        Session? GetSession(Guid id);
        IEnumerable<Session> ListActiveSessions();
        IEnumerable<Session> ListAllSessions();
        bool RestartSession(Guid id);
        bool RemoveSession(Guid id);
    }
}
