using System;

namespace LibFloaderClient.Implementations.Exceptions
{
    /// <summary>
    /// Exception, thrown by SerialPortDriver if operation didn't complete in time
    /// </summary>
    public class SerialPortTimeoutException : Exception
    {
        public SerialPortTimeoutException() : base()
        {

        }

        public SerialPortTimeoutException(string message) : base(message)
        {

        }

        public SerialPortTimeoutException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}