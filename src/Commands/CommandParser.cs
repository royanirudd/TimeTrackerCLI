using TimeTracker.Interfaces;
using TimeTracker.Models;

namespace TimeTracker.Commands
{
    public class CommandParser
    {
        private readonly ISessionManager _sessionManager;

        public CommandParser(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public void ParseAndExecute(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
                return;
            }

            switch (args[0].ToLower())
            {
                case "start":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Error: Session name required");
                        ShowUsage();
                        return;
                    }
                    StartSession(args[1]);
                    break;

                case "stop":
                    if (args.Length < 2 || !Guid.TryParse(args[1], out Guid stopId))
                    {
                        Console.WriteLine("Error: Valid session ID required");
                        ShowUsage();
                        return;
                    }
                    StopSession(stopId);
                    break;

                case "restart":
                    if (args.Length < 2 || !Guid.TryParse(args[1], out Guid restartId))
                    {
                        Console.WriteLine("Error: Valid session ID required");
                        ShowUsage();
                        return;
                    }
                    RestartSession(restartId);
                    break;

                case "remove":
                    if (args.Length < 2 || !Guid.TryParse(args[1], out Guid removeId))
                    {
                        Console.WriteLine("Error: Valid session ID required");
                        ShowUsage();
                        return;
                    }
                    RemoveSession(removeId);
                    break;

                case "list":
                    bool showAll = args.Length > 1 && args[1] == "-a";
                    ListSessions(showAll);
                    break;

                default:
                    Console.WriteLine($"Unknown command: {args[0]}");
                    ShowUsage();
                    break;
            }
        }

        private void StartSession(string name)
        {
            var session = _sessionManager.StartSession(name);
            Console.WriteLine($"Session started: {session.Id}");
        }

        private void StopSession(Guid sessionId)
        {
            if (_sessionManager.StopSession(sessionId))
            {
                Console.WriteLine($"Session stopped: {sessionId}");
            }
            else
            {
                Console.WriteLine($"Failed to stop session: {sessionId}");
            }
        }

        private void RestartSession(Guid sessionId)
        {
            if (_sessionManager.RestartSession(sessionId))
            {
                Console.WriteLine($"Session restarted: {sessionId}");
            }
            else
            {
                Console.WriteLine($"Failed to restart session: {sessionId}");
            }
        }

        private void RemoveSession(Guid sessionId)
        {
            if (_sessionManager.RemoveSession(sessionId))
            {
                Console.WriteLine($"Session removed: {sessionId}");
            }
            else
            {
                Console.WriteLine($"Failed to remove session: {sessionId}");
            }
        }

        private void ListSessions(bool showAll)
        {
            var sessions = showAll ? _sessionManager.ListAllSessions() : _sessionManager.ListActiveSessions();

            if (!sessions.Any())
            {
                Console.WriteLine(showAll ? "No sessions" : "No active sessions");
                return;
            }

            Console.WriteLine(showAll ? "All sessions:" : "Active sessions:");
            foreach (var session in sessions)
            {
                Console.WriteLine($"ID: {session.Id}");
                Console.WriteLine($"Name: {session.Name}");
                Console.WriteLine($"Duration: {(session.IsActive ? DateTime.Now - session.StartTime : session.EndTime - session.StartTime)}");
                Console.WriteLine($"Status: {(session.IsActive ? "Active" : "Stopped")}");
                Console.WriteLine();
            }
        }

        private void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  start <session-name>  - Start a new session");
            Console.WriteLine("  stop <session-id>     - Stop an active session");
            Console.WriteLine("  restart <session-id>  - Restart a stopped session");
            Console.WriteLine("  remove <session-id>   - Remove a session completely");
            Console.WriteLine("  list                  - List active sessions");
            Console.WriteLine("  list -a               - List all sessions (including stopped)");
        }
    }
}
