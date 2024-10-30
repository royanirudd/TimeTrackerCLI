using System;
using System.Collections.Generic;
using TimeTracker.Models;

namespace TimeTracker.Services
{
    public interface ISessionManager
    {
        Guid StartSession(string name);
        void StopSession(Guid sessionId);
        Session GetSession(Guid sessionId);
        IEnumerable<Session> ListActiveSessions();
        IEnumerable<Session> ListAllSessions();
        void RestartSession(Guid sessionId);
        void RemoveSession(Guid sessionId);
        void StopActivity(Guid sessionId, Guid activityId);
        Guid StartActivity(Guid sessionId);
    }
}
