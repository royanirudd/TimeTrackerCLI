using Microsoft.Extensions.Logging;
using Moq;
using TimeTracker.Services;
using TimeTracker.Models;
using Xunit;

namespace TimeTracker.Tests
{
    public class SessionManagerTests
    {
        private readonly SessionManager _manager;
        private readonly Mock<ILogger<SessionManager>> _loggerMock;

        public SessionManagerTests()
        {
            _loggerMock = new Mock<ILogger<SessionManager>>();
            _manager = new SessionManager(_loggerMock.Object);
        }

        [Fact]
        public void TestStartSession()
        {
            var session = _manager.StartSession("Test Session");
            Assert.NotNull(session);
            Assert.True(session.IsActive);
        }

        [Fact]
        public void TestStopSession()
        {
            var session = _manager.StartSession("Test Session");
            var result = _manager.StopSession(session.Id);
            Assert.True(result);

            var stoppedSession = _manager.GetSession(session.Id);
            Assert.NotNull(stoppedSession);
            Assert.False(stoppedSession.IsActive);
        }

        [Fact]
        public void TestListActiveSessions()
        {
            var logger = new Mock<ILogger<SessionManager>>().Object;
            var manager = new SessionManager(logger, "test_sessions.json");

            // Start sessions
            var session1 = manager.StartSession("Test1");
            var session2 = manager.StartSession("Test2");
            var session3 = manager.StartSession("Test3");

            // Stop session3
            manager.StopSession(session3.Id);

            var activeSessions = manager.ListActiveSessions();
            Assert.Equal(2, activeSessions.Count());

            // Cleanup
            if (File.Exists("test_sessions.json"))
            {
                File.Delete("test_sessions.json");
            }
        }
    }
}
