using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace TimeTracker.Services
{
    public class ActivityMonitor
    {
        private readonly ILogger<ActivityMonitor> _logger;
        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        private static readonly bool IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public ActivityMonitor(ILogger<ActivityMonitor> logger)
        {
            _logger = logger;
        }

        public (string applicationName, string filePath) GetCurrentActivity()
        {
            try
            {
                if (IsWindows)
                {
                    return GetWindowsActiveWindow();
                }
                else if (IsLinux)
                {
                    return GetLinuxActiveWindow();
                }
                else if (IsMacOS)
                {
                    return GetMacOSActiveWindow();
                }

                return ("Unknown", "Unknown");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get current activity");
                return ("Unknown", "Unknown");
            }
        }

        private (string applicationName, string filePath) GetWindowsActiveWindow()
        {
            var activeWindowHandle = GetForegroundWindow();
            if (activeWindowHandle == IntPtr.Zero)
            {
                return ("Unknown", "Unknown");
            }

            GetWindowThreadProcessId(activeWindowHandle, out uint processId);
            var process = Process.GetProcessById((int)processId);

            var title = GetWindowTitle(activeWindowHandle);
            return ParseWindowInfo(process.ProcessName, title);
        }

        private (string applicationName, string filePath) GetLinuxActiveWindow()
        {
            try
            {
                var windowInfo = ExecuteCommand("xdotool", "getactivewindow getwindowname");
                var processInfo = ExecuteCommand("xdotool", "getactivewindow getwindowpid");

                if (string.IsNullOrEmpty(processInfo))
                {
                    return ("Unknown", "Unknown");
                }

                if (int.TryParse(processInfo, out int pid))
                {
                    var process = Process.GetProcessById(pid);
                    return ParseWindowInfo(process.ProcessName, windowInfo);
                }

                return ("Unknown", "Unknown");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Linux active window");
                return ("Unknown", "Unknown");
            }
        }

        private (string applicationName, string filePath) GetMacOSActiveWindow()
        {
            try
            {
                var script = @"
                    tell application ""System Events""
                        set frontApp to first application process whose frontmost is true
                        set appName to name of frontApp
                        set windowTitle to ""Unknown""
                        try
                            set windowTitle to name of first window of frontApp
                        end try
                        return {appName, windowTitle}
                    end tell";

                var result = ExecuteCommand("osascript", $"-e '{script}'");
                var parts = result.Split(',');

                string appName = parts.Length > 0 ? parts[0].Trim() : "Unknown";
                string windowTitle = parts.Length > 1 ? parts[1].Trim() : "Unknown";

                return ParseWindowInfo(appName, windowTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get macOS active window");
                return ("Unknown", "Unknown");
            }
        }

        private (string applicationName, string filePath) ParseWindowInfo(string processName, string windowTitle)
        {
            // Normalize process name
            processName = processName.Replace(".exe", "").Replace("-", " ").ToTitleCase();

            // Try to extract file path from window title based on common patterns
            string filePath = "Unknown";

            // Common patterns: "filename - application" or "application - filename"
            var parts = windowTitle.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                // Try to identify which part is the file path
                foreach (var part in parts)
                {
                    if (part.Contains("/") || part.Contains("\\") || part.Contains("."))
                    {
                        filePath = part.Trim();
                        break;
                    }
                }
            }

            return (processName, filePath);
        }

        private string ExecuteCommand(string command, string arguments)
        {
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = command,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output.Trim();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to execute command: {command} {arguments}");
                return string.Empty;
            }
        }

        #region Windows API Imports
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetWindowTitle(IntPtr hWnd)
        {
            var builder = new StringBuilder(256);
            GetWindowText(hWnd, builder, 256);
            return builder.ToString();
        }
        #endregion
    }

    public static class StringExtensions
    {
        public static string ToTitleCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }
            return string.Join(" ", words);
        }
    }
}
