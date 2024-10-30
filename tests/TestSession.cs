using System;
using TimeTracker.Models;
using Xunit;

namespace TimeTracker.Tests
{
    public class SessionTests
    {
        [Fact]
        public void Session_Constructor_InitializesCorrectly()
        {
            // Arrange & Act
            string sessionName = "TestSession";
            var session = new Session(sessionName);

            // Assert
            Assert.Equal(sessionName, session.Name);
            Assert.True(session.IsActive);
            Assert.NotEqual(Guid.Empty, session.Id);
            Assert.NotNull(session.ApplicationTimes);
            Assert.NotNull(session.FileTimes);
        }
    }
}
