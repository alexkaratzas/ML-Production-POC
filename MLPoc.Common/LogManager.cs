using System;

namespace MLPoc.Common
{
    public static class LogManager
    {
        public static ILogger Instance { get; private set; } = new NullLogger();

        public static void SetLogger(ILogger logger)
        {
            Instance = logger;
        }
    }

    public interface ILogger
    {
        void Info(string message);
        void Error(string message, Exception ex = null);
    }

    public class NullLogger : ILogger
    {
        public void Info(string message)
        {
        }

        public void Error(string message, Exception ex = null)
        {
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Info(string message)
        {
            WriteToConsole(message);
        }

        public void Error(string message, Exception ex = null)
        {
            WriteToConsole(message);
        }

        private static void WriteToConsole(string message)
        {
            Console.WriteLine(message);
        }
    }
}