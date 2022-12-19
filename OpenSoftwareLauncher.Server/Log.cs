using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System;

namespace OpenSoftwareLauncher.Server
{
    public static class Log
    {
        public struct LogColor
        {
            public ConsoleColor Foreground;
            public ConsoleColor Background;
        }
        #region Init Colors
        public static LogColor WarnColor = new LogColor
        {
            Background = ConsoleColor.Yellow,
            Foreground = ConsoleColor.Black
        };
        public static LogColor ErrorColor = new LogColor
        {
            Background = ConsoleColor.Red,
            Foreground = ConsoleColor.Black
        };
        public static LogColor DebugColor = new LogColor
        {
            Background = ConsoleColor.Blue,
            Foreground = ConsoleColor.White
        };
        public static LogColor DefaultColor = new LogColor
        {
            Background = ConsoleColor.Black,
            Foreground = ConsoleColor.White
        };
        #endregion

        private static List<string> linequeue = new List<string>();
        private static System.Timers.Timer? _timer = null;
        public static string LogOutput => Path.Combine(Directory.GetCurrentDirectory(), "Logs", $"log_{MainClass.StartupTimestamp}.txt");
        public static bool EnableLogFileWrite = false;
        private static void CreateTimer()
        {
            if (!EnableLogFileWrite) return;
            if (_timer != null) return;
            string logDirectory = Path.GetDirectoryName(LogOutput) ?? Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);
            _timer = new System.Timers.Timer();
            _timer.Interval = 5000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;
            _timer.Start();
        }

        private static void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _timer?.Stop();
            string[] lines = linequeue.ToArray();
            linequeue.Clear();
            File.AppendAllLines(LogOutput, lines);
            _timer?.Start();
        }

        public static void SetColor(LogColor? color = null)
        {
            LogColor targetColor = color ?? DefaultColor;
            Console.BackgroundColor = targetColor.Background;
            Console.ForegroundColor = targetColor.Foreground;
        }
        public static string WarnPrefix = "[WARN]";
        public static string ErrorPrefix = "[ERR] ";
        public static string LogPrefix = "[LOG] ";
        public static string DebugPrefix = "[DEBG]";
        public static bool ShowMethodName = true;
        public static bool ShowTimestamp = false;
        public static void Warn(string content, [CallerMemberName] string methodname = null, [CallerFilePath] string methodfile = null)
        {
            WriteLine(content, WarnColor, WarnPrefix, ShowMethodName, methodname, methodfile);
        }
        public static void Error(string content, [CallerMemberName] string methodname = null, [CallerFilePath] string methodfile = null)
        {
            WriteLine(content, ErrorColor, ErrorPrefix, ShowMethodName, methodname, methodfile);
        }
        public static void Debug(string content, [CallerMemberName] string methodname = null, [CallerFilePath] string methodfile = null)
        {
            WriteLine(content, DebugColor, DebugPrefix, ShowMethodName, methodname, methodfile);
        }
        public static void WriteLine(string content, LogColor? color = null, string prefix = null, bool fetchMethodName = true, [CallerMemberName] string methodname = null, [CallerFilePath] string methodfile = null)
        {
            string pfx = (prefix ?? LogPrefix) + " ";
            // Create log file timer if it's not made yet
            CreateTimer();
            SetColor(color);
            // If apliciable, prepend the formatted method/class name to the content
            if (methodname != null && fetchMethodName && methodfile != null)
                content = $"{FormatMethodName(methodname, methodfile)}{content}";


            // Write content with custom color, reset color, then add new line.
            Console.Write(pfx + content);
            if (EnableLogFileWrite)
                linequeue.Add(pfx + content);
            SetColor();
            Console.Write("\n");
        }
        private static string FormatMethodName(string methodName, string methodFilePath)
        {
            var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (methodName != null)
            {
                if (methodFilePath != null)
                    if (ShowTimestamp)
                        return $"[{Path.GetFileNameWithoutExtension(methodFilePath)}->{methodName}:{ts}] ";
                    else
                        return $"[{Path.GetFileNameWithoutExtension(methodFilePath)}->{methodName}] ";
                if (ShowTimestamp)
                    return $"[unknown->{methodName}:{ts}] ";
                return $"[unknown->{methodName}] ";
            }
            else if (methodFilePath != null)
                if (ShowTimestamp)
                    return $"[{Path.GetFileNameWithoutExtension(methodFilePath)}:{ts}] ";
                else
                    return $"[{Path.GetFileNameWithoutExtension(methodFilePath)}] ";
            if (ShowTimestamp)
                return $"[{ts}] ";
            return "";
        }
    }
}
