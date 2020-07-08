using System;
using LibFloaderClient.Interfaces.Logger;

namespace FloaderClientGUI.GUISpecific.Logger
{
    /// <summary>
    /// Logger, able to log to GUI console
    /// </summary>
    public class Logger : ILogger
    {
        /// <summary>
        /// Templates for various levels.static {0} is time, {1} is message
        /// </summary>
        private const string InfoTemplate = "[{0}] Info: {1}";
        private const string WarningTemplate = "[{0}] Warning: {1}";
        private const string ErrorTemplate = "[{0}] Error: {1}";

        private Action<string> _logFunc = str => Console.WriteLine(str);

        public void LogInfo(string message)
        {
            WriteMessage(message, InfoTemplate);
        }

        public void LogWarning(string message)
        {
            WriteMessage(message, WarningTemplate);
        }

        public void LogError(string message)
        {
            WriteMessage(message, ErrorTemplate);
        }

        public void SetLoggingFunction(Action<string> logFunc)
        {
            _logFunc = logFunc;
        }

        /// <summary>
        /// Writes message to log, using given template
        /// </summary>
        private void WriteMessage(string message, string template)
        {
            _logFunc(string.Format(template, DateTime.Now.ToLongTimeString(), message));
        }
    }
}