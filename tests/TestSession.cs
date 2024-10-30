using TimeTracker.Models;
using Xunit;

namespace TimeTracker.Tests
{
    public class TestSession
    {
        [Fact]
        public void TestSessionCreation()
        {
            var session = new Session
            {
                Name = "Test Session",
                StartTime = DateTime.Now,
                IsActive = true
            };

            Assert.Empty(session.ApplicationTimes);
            Assert.Empty(session.FileTimes);
            Assert.True(session.IsActive);
            Assert.NotEqual(Guid.Empty, session.Id);
        }
    }
}
