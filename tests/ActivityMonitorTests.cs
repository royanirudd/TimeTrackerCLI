using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TimeTracker.Services;

namespace TimeTracker.Tests
{
    public class ActivityMonitorTests
    {
        private readonly ActivityMonitor _monitor;
        private readonly Mock<ILogger<ActivityMonitor>> _loggerMock;

        public ActivityMonitorTests()
        {
            _loggerMock = new Mock<ILogger<ActivityMonitor>>();
            _monitor = new ActivityMonitor(_loggerMock.Object);
        }

        [Fact]
        public void TestGetCurrentActivity()
        {
            var (appName, filePath) = _monitor.GetCurrentActivity();
            
            Assert.NotNull(appName);
            Assert.NotNull(filePath);
            Assert.NotEqual("", appName);
            Assert.NotEqual("", filePath);
        }

        [Theory]
        [InlineData("notepad.exe", "document.txt - Notepad", "Notepad", "document.txt")]
        [InlineData("code", "project.cs - Visual Studio Code", "Code", "project.cs")]
        [InlineData("unknown-process", "Unknown Window", "Unknown Process", "Unknown")]
        public void TestParseWindowInfo(string processName, string windowTitle, string expectedApp, string expectedFile)
        {
            var type = typeof(ActivityMonitor);
            var method = type.GetMethod("ParseWindowInfo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var result = (ValueTuple<string, string>)method.Invoke(_monitor, new object[] { processName, windowTitle });
            
            Assert.Equal(expectedApp, result.Item1);
            Assert.Equal(expectedFile, result.Item2);
        }
    }
}
