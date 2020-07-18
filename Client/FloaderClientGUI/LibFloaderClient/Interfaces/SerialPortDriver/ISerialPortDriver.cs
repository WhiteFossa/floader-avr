using System;
using System.Collections.Generic;

namespace LibFloaderClient.Interfaces.SerialPortDriver
{
    /// <summary>
    /// Pseudo-synchronous serial port IO driver
    /// </summary>
    public interface ISerialPortDriver : IDisposable
    {
        /// <summary>
        /// Sets port operations timeout.
        /// </summary>
        void SetTimeout(int timeout);

        /// <summary>
        /// Blocks IO and trying to write given bytes into port. If there is no succes during the timeout,
        /// then throws SerialPortTimeoutException.
        /// </summary>
        void Write(List<byte> contentToWrite);

        /// <summary>
        /// Blocks IO and trying to read requiredSize of bytes from it. If there is no succes during the timeout,
        /// then throws SerialPortTimeoutException.
        /// </summary>
        List<byte> Read(int requiredSize);
    }
}