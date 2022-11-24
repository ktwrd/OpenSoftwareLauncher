using System.Runtime.CompilerServices;
using System;

namespace OpenSoftwareLauncher.Server
{
    public static class CPrint
    {
        public struct LogColor
        {
            public LogColor()
            {
                Foreground = ConsoleColor.White;
                Background = ConsoleColor.Black;
            }
            public ConsoleColor Foreground;
            public ConsoleColor Background;
        }

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
        public static bool AddMethodName = false;
        public static void Warn(string content, [CallerMemberName] string methodname = null)
        {
            if (methodname != null && AddMethodName)
                content = $"{FormatMethodName(methodname)}{content}";
            WriteLine(content, WarnColor, WarnPrefix, false);
        }
        public static void Error(string content, [CallerMemberName] string methodname = null)
        {
            if (methodname != null && AddMethodName)
                content = $"{FormatMethodName(methodname)}{content}";
            WriteLine(content, ErrorColor, ErrorPrefix, false);
        }
        public static void Debug(string content, [CallerMemberName] string methodname = null)
        {
            if (methodname != null && AddMethodName)
                content = $"{FormatMethodName(methodname)}{content}";
            WriteLine(content, DebugColor, DebugPrefix, false);
        }
        public static void WriteLine(string content, LogColor? color = null, string? prefix = null, bool fetchMethodName = true, [CallerMemberName] string methodname = null)
        {
            SetColor(color ?? DefaultColor);
            if (methodname != null && fetchMethodName && AddMethodName)
                content = $"{FormatMethodName(methodname)}{content}";
            string pfx = (prefix ?? LogPrefix) + " ";
            Console.WriteLine(pfx + content);
            SetColor();
        }
        private static string FormatMethodName(string? methodName)
        {
            string result = "";
            if (methodName != null && AddMethodName)
            {
                result = $"[{methodName}] ";
            }
            if (result.Length > 0 && false)
                result = result.PadRight(40);
            return result;
        }
    }
}
