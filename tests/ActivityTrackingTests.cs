using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TimeTracker.Services;
using TimeTracker.Models;
using System;
using System.Linq;
using System.IO;

namespace TimeTracker.Tests
{
    public class ActivityTrackingTests
    {
        private readonly string TestFilePath = "test_sessions.json";

        [Fact]
        public void TestActivityTracking()
        {
            var logger = new Mock<ILogger<SessionManager>>().Object;
            var manager = new SessionManager(logger, TestFilePath);

            // Start a session
            var session = manager.StartSession("Test Session");

            // Log some activities
            var activity1 = new ActivityLog
            {
                Id = Guid.NewGuid(),
                ApplicationName = "Neovim",
                FilePath = "/home/user/test.txt",
                StartTime = DateTime.Now
            };

            session.Activities.Add(activity1);

            // Stop the activity after 1 second
            Thread.Sleep(1000);
            activity1.EndTime = DateTime.Now;

            Assert.True(activity1.Duration.TotalSeconds >= 1);
            Assert.Equal("Neovim", activity1.ApplicationName);

            // Cleanup
            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath);
            }
        }
    }
}
