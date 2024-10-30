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
        private readonly string TestFilePath = "test_sessions.json";

        public SessionManagerTests()
        {
            _loggerMock = new Mock<ILogger<SessionManager>>();
            _manager = new SessionManager(_loggerMock.Object, TestFilePath);
        }

        // Keep all existing tests...

        [Fact]
        public void TestActivityTracking()
        {
            // Start a session
            var session = _manager.StartSession("Test Session");

            // Start an activity
            var activity = _manager.StartActivity(session.Id, "Neovim", "/test/file.txt");
            Assert.NotNull(activity);
            
            // Wait briefly to ensure measurable duration
            Thread.Sleep(1000);
            
            // Stop the activity
            var result = _manager.StopActivity(session.Id, activity.Id);
            Assert.True(result);

            // Verify statistics
            var activities = _manager.GetSessionActivities(session.Id);
            Assert.Single(activities);

            var appStats = _manager.GetApplicationStatistics(session.Id);
            Assert.True(appStats.ContainsKey("Neovim"));
            Assert.True(appStats["Neovim"].TotalMilliseconds >= 1000);

            var fileStats = _manager.GetFileStatistics(session.Id);
            Assert.True(fileStats.ContainsKey("/test/file.txt"));

            var totalTime = _manager.GetTotalActiveTime(session.Id);
            Assert.True(totalTime.TotalMilliseconds >= 1000);
        }

        [Fact]
        public void TestInvalidActivityOperations()
        {
            var session = _manager.StartSession("Test Session");
            var invalidSessionId = Guid.NewGuid();
            var invalidActivityId = Guid.NewGuid();

            // Test invalid session ID
            Assert.Throws<KeyNotFoundException>(() => 
                _manager.StartActivity(invalidSessionId, "Test", "test.txt"));

            // Test stopping non-existent activity
            Assert.False(_manager.StopActivity(session.Id, invalidActivityId));

            // Test getting activities for invalid session
            var activities = _manager.GetSessionActivities(invalidSessionId);
            Assert.Empty(activities);
        }

        public void Dispose()
        {
            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath);
            }
        }
    }
}
