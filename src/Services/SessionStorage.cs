using System.Text.Json;
using TimeTracker.Models;

namespace TimeTracker.Services
{
    public class SessionStorage
    {
        private readonly string _storageFile;

        public SessionStorage(string storageFile = "sessions.json")
        {
            _storageFile = storageFile;
            if (!File.Exists(_storageFile))
            {
                File.WriteAllText(_storageFile, "[]");
            }
        }

        public List<Session> LoadSessions()
        {
            var json = File.ReadAllText(_storageFile);
            return JsonSerializer.Deserialize<List<Session>>(json) ?? new List<Session>();
        }

        public void SaveSessions(List<Session> sessions)
        {
            var json = JsonSerializer.Serialize(sessions);
            File.WriteAllText(_storageFile, json);
        }
    }
}
