using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TimeTracker.Services;
using TimeTracker.Interfaces;

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());
services.AddSingleton<ISessionManager, SessionManager>();

var serviceProvider = services.BuildServiceProvider();
var sessionManager = serviceProvider.GetRequiredService<ISessionManager>();

if (args.Length == 0)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  start <name>     - Start a new session");
    Console.WriteLine("  stop <id>        - Stop a session");
    Console.WriteLine("  list             - List active sessions");
    Console.WriteLine("  list --all       - List all sessions");
    Console.WriteLine("  restart <id>     - Restart a stopped session");
    Console.WriteLine("  remove <id>      - Remove a session");
    return;
}

string command = args[0].ToLower();

switch (command)
{
    case "start" when args.Length > 1:
        var session = sessionManager.StartSession(args[1]);
        Console.WriteLine($"Session started: {session.Id}");
        break;

    case "stop" when args.Length > 1:
        if (Guid.TryParse(args[1], out Guid stopId))
        {
            bool stopped = sessionManager.StopSession(stopId);
            Console.WriteLine(stopped ? $"Session stopped: {stopId}" : "Session not found or already stopped");
        }
        break;

    case "list":
        bool showAll = args.Length > 1 && (args[1] == "--all" || args[1] == "-a");
        var sessions = showAll ? sessionManager.ListAllSessions() : sessionManager.ListActiveSessions();
        if (!sessions.Any())
        {
            Console.WriteLine(showAll ? "No sessions found" : "No active sessions");
            break;
        }
        if (!showAll) Console.WriteLine("Active sessions:");
        foreach (var s in sessions)
        {
            Console.WriteLine($"ID: {s.Id}");
            Console.WriteLine($"Name: {s.Name}");
            var duration = s.IsActive ?
                DateTime.Now - s.StartTime :
                s.EndTime!.Value - s.StartTime;
            Console.WriteLine($"Duration: {duration}");
            Console.WriteLine($"Status: {(s.IsActive ? "Active" : "Completed")}");
            Console.WriteLine();
        }
        break;

    case "restart" when args.Length > 1:
        if (Guid.TryParse(args[1], out Guid restartId))
        {
            bool restarted = sessionManager.RestartSession(restartId);
            Console.WriteLine(restarted ? $"Session restarted: {restartId}" : "Session not found or already active");
        }
        break;

    case "remove" when args.Length > 1:
        if (Guid.TryParse(args[1], out Guid removeId))
        {
            bool removed = sessionManager.RemoveSession(removeId);
            Console.WriteLine(removed ? $"Session removed: {removeId}" : "Session not found");
        }
        break;

    default:
        Console.WriteLine("Invalid command or missing arguments");
        break;
}
