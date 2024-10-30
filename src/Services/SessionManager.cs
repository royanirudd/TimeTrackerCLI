using System;
using System.Collections.Generic;
using System.Linq;
using TimeTracker.Models;
using Microsoft.Extensions.Logging;

namespace TimeTracker.Services
{
    public class SessionManager : ISessionManager
    {
        private readonly ILogger<SessionManager> _logger;
        private readonly Dictionary<Guid, Session> _sessions;
        private readonly IActivityMonitor _activityMonitor;

        public SessionManager(ILogger<SessionManager> logger, IActivityMonitor activityMonitor)
        {
            _logger = logger;
            _activityMonitor = activityMonitor;
            _sessions = new Dictionary<Guid, Session>();
        }

        public Guid StartSession(string name)
        {
            var session = new Session
            {
                Id = Guid.NewGuid(),
                Name = name,
                StartTime = DateTime.UtcNow,
                IsActive = true,
                Activities = new List<Activity>()
            };

            _sessions[session.Id] = session;
            _logger.LogInformation($"Started new session: {name} with ID: {session.Id}");
            return session.Id;
        }

        public void StopSession(Guid sessionId)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                session.IsActive = false;
                session.EndTime = DateTime.UtcNow;
                _logger.LogInformation($"Stopped session: {session.Name} with ID: {sessionId}");
            }
            else
            {
                _logger.LogWarning($"Attempted to stop non-existent session: {sessionId}");
            }
        }

        public Session GetSession(Guid sessionId)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                return session;
            }
            _logger.LogWarning($"Session not found: {sessionId}");
            return null;
        }

        public IEnumerable<Session> ListActiveSessions()
        {
            return _sessions.Values.Where(s => s.IsActive);
        }

        public IEnumerable<Session> ListAllSessions()
        {
            return _sessions.Values;
        }

        public void RestartSession(Guid sessionId)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                session.IsActive = true;
                session.EndTime = null;
                _logger.LogInformation($"Restarted session: {session.Name} with ID: {sessionId}");
            }
            else
            {
                _logger.LogWarning($"Attempted to restart non-existent session: {sessionId}");
            }
        }

        public void RemoveSession(Guid sessionId)
        {
            if (_sessions.Remove(sessionId))
            {
                _logger.LogInformation($"Removed session with ID: {sessionId}");
            }
            else
            {
                _logger.LogWarning($"Attempted to remove non-existent session: {sessionId}");
            }
        }

        public void StopActivity(Guid sessionId, Guid activityId)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                var activity = session.Activities.FirstOrDefault(a => a.Id == activityId);
                if (activity != null)
                {
                    activity.EndTime = DateTime.UtcNow;
                    _logger.LogInformation($"Stopped activity {activityId} in session {sessionId}");
                }
                else
                {
                    _logger.LogWarning($"Activity {activityId} not found in session {sessionId}");
                }
            }
            else
            {
                _logger.LogWarning($"Session not found: {sessionId}");
            }
        }

        public Guid StartActivity(Guid sessionId)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
            {
                _logger.LogWarning($"Session not found: {sessionId}");
                return Guid.Empty;
            }

            if (!session.IsActive)
            {
                _logger.LogWarning($"Cannot start activity in inactive session: {sessionId}");
                return Guid.Empty;
            }

            var currentWindow = _activityMonitor.GetCurrentActivity();
            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                ApplicationName = currentWindow.applicationName,
                FilePath = currentWindow.filePath,
                StartTime = DateTime.UtcNow
            };

            session.Activities.Add(activity);
            _logger.LogInformation($"Started activity {activity.Id} in session {sessionId}");
            return activity.Id;
        }
    }
}
