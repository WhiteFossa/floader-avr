using System;
using System.IO.Ports;

namespace LibFloaderClient.Implementations.Exceptions
{
    /// <summary>
    /// Exception, indicating serial port IO error
    /// </summary>
    public class SerialPortIOErrorException : Exception
    {
        /// <summary>
        /// Error type
        /// </summary>
        public SerialError? Type { get; private set; }

        public SerialPortIOErrorException() : base()
        {
            Type = null;
        }

        public SerialPortIOErrorException(string message) : base(message)
        {
            Type = null;
        }

        public SerialPortIOErrorException(string message, Exception inner) : base(message, inner)
        {
            Type = null;
        }

        public SerialPortIOErrorException(string message, SerialError type) : base(message)
        {
            Type = type;
        }

        public SerialPortIOErrorException(string message, Exception inner, SerialError type) : base(message, inner)
        {
            Type = type;
        }
    }
}