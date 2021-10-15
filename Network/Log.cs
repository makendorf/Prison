using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
//using System.Windows.Forms;

namespace Network
{
    public enum LogLevel
    {
        None = 0,
        Info,
        Success,
        Warning,
        Error
    }

    public class Log
    {
        public delegate void LogDelegate(LogLevel Level, string Message);
        public static   event LogDelegate OnLog;

        public static Color GetLevelColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.None:
                    return Color.Black;
                case LogLevel.Info:
                    return Color.White;
                case LogLevel.Success:
                    return Color.Lime;
                case LogLevel.Warning:
                    return Color.Yellow;
                case LogLevel.Error:
                    return Color.Red;
            }
            return Color.Black;
        }

        private static string MakeLogMessage(string format, params object[] arg)
        {
            DateTime dt = DateTime.Now;
            string Result;
            string timeformat = "T";
            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            Result = "[" + dt.ToString(timeformat, culture) + "] ";
            Result += String.Format(format, arg);
            return Result;
        }
       
        public static void Info(string format, params object[] arg)
        {
            string Message = MakeLogMessage(format, arg);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Message);
            Console.ForegroundColor = ConsoleColor.White;
            OnLog?.Invoke(LogLevel.Info, Message);
        }

        public static void Warning(string format, params object[] arg)
        {
            string Message = MakeLogMessage(format, arg);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Message);
            Console.ForegroundColor = ConsoleColor.White;
            OnLog?.Invoke(LogLevel.Warning, Message);

        }
        public static void Error(string format, params object[] arg)
        {
            string Message = MakeLogMessage(format, arg);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Message);
            Console.ForegroundColor = ConsoleColor.White;
            OnLog?.Invoke(LogLevel.Error, Message);
        }

        public static void Success(string format, params object[] arg)
        {
            string Message = MakeLogMessage(format, arg);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Message);
            Console.ForegroundColor = ConsoleColor.White;
            OnLog?.Invoke(LogLevel.Success, Message);
        }
    }
}
