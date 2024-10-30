using Microsoft.Extensions.Logging;
using TimeTracker.Models;
using TimeTracker.Interfaces;
using Newtonsoft.Json;

namespace TimeTracker.Services
{
    public class SessionManager : ISessionManager
    {
        private Dictionary<Guid, Session> _sessions = new();
        private readonly ILogger<SessionManager> _logger;
        private readonly string _storageFile;

        public SessionManager(ILogger<SessionManager> logger, string storageFile = "sessions.json")
        {
            _logger = logger;
            _storageFile = storageFile;
            LoadSessions();
        }

        private void LoadSessions()
        {
            try
            {
                if (File.Exists(_storageFile))
                {
                    var json = File.ReadAllText(_storageFile);
                    try
                    {
                        _sessions = JsonConvert.DeserializeObject<Dictionary<Guid, Session>>(json) ?? new Dictionary<Guid, Session>();
                    }
                    catch (JsonSerializationException)
                    {
                        // If dictionary deserialization fails, try deserializing as list
                        var sessionsList = JsonConvert.DeserializeObject<List<Session>>(json);
                        if (sessionsList != null)
                        {
                            _sessions = sessionsList.ToDictionary(s => s.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sessions. Starting with empty session list.");
                _sessions = new Dictionary<Guid, Session>();
                // Delete corrupted file
                if (File.Exists(_storageFile))
                {
                    try
                    {
                        File.Delete(_storageFile);
                    }
                    catch (Exception deleteEx)
                    {
                        _logger.LogError(deleteEx, "Failed to delete corrupted sessions file");
                    }
                }
            }
        }

        private void SaveSessions()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_sessions, Formatting.Indented);
                File.WriteAllText(_storageFile, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving sessions");
            }
        }

        // Rest of the methods remain the same
        public Session StartSession(string name)
        {
            var session = new Session
            {
                Name = name,
                StartTime = DateTime.Now,
                IsActive = true
            };

            _sessions[session.Id] = session;
            _logger.LogInformation("Session started: {Id} - {Name}", session.Id, session.Name);
            SaveSessions();
            return session;
        }

        public bool StopSession(Guid id)
        {
            if (!_sessions.TryGetValue(id, out var session) || !session.IsActive)
            {
                return false;
            }

            session.IsActive = false;
            session.EndTime = DateTime.Now;
            _logger.LogInformation("Session stopped: {Id}", id);
            SaveSessions();
            return true;
        }

        public Session? GetSession(Guid id)
        {
            return _sessions.TryGetValue(id, out var session) ? session : null;
        }

        public IEnumerable<Session> ListActiveSessions()
        {
            return _sessions.Values.Where(s => s.IsActive);
        }

        public IEnumerable<Session> ListAllSessions()
        {
            return _sessions.Values;
        }

        public bool RestartSession(Guid id)
        {
            if (!_sessions.TryGetValue(id, out var session) || session.IsActive)
            {
                return false;
            }

            session.IsActive = true;
            session.StartTime = DateTime.Now;
            session.EndTime = null;
            _logger.LogInformation("Session restarted: {Id}", id);
            SaveSessions();
            return true;
        }

        public bool RemoveSession(Guid id)
        {
            if (!_sessions.ContainsKey(id))
            {
                return false;
            }

            _sessions.Remove(id);
            _logger.LogInformation("Session removed: {Id}", id);
            SaveSessions();
            return true;
        }
    }
}
