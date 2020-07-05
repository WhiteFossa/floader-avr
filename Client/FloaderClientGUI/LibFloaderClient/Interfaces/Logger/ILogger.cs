namespace LibFloaderClient.Interfaces.Logger
{
    public interface ILogger
    {
        /// <summary>
        /// Log info-level event
        /// </summary>
        void LogInfo(string message);

        /// <summary>
        /// Log warning-level event
        /// </summary>
        void LogWarning(string message);

        /// <summary>
        /// Log error-level event
        /// </summary>
        void LogError(string message);
    }
}