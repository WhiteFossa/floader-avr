using System.Xml.Linq;
using System.Threading;
using System.Linq;
using System;
using System.Collections.Generic;
using LibFloaderClient.Interfaces.SerialPortDriver;
using LibFloaderClient.Models.Port;
using System.IO.Ports;
using LibFloaderClient.Implementations.Exceptions;
using System.Timers;

namespace LibFloaderClient.Implementations.SerialPortDriver
{
    public class SerialPortDriver : ISerialPortDriver
    {
        /// <summary>
        /// Default timeout, 60 seconds.
        /// </summary>
        private const int DefaultTimeout = 60000;

        /// <summary>
        /// How long (in milliseconds) to wait before next check if data arrived when Read() called
        /// </summary>
        private const int ReadSleepTime = 10;

        /// <summary>
        /// Attemtp to read data from port using this size blocks
        /// </summary>
        private const int ReadBlockSize = 16;

        private bool _isDisposed = false;

        /// <summary>
        /// We use this serial port
        /// </summary>
        private SerialPort _port;

        /// <summary>
        /// Happened IO error, null if everything is OK
        /// </summary>
        private SerialError? _IOError;

        public SerialPortDriver(PortSettings settings)
        {
            _port = new SerialPort(portName: settings.Name, baudRate: settings.Baudrate, parity: settings.Parity,
                dataBits: settings.DataBits, stopBits: settings.StopBits);

            // Attaching events
            _port.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceivedHandler);

            _port.Open();
        }

        public void Write(List<byte> contentToWrite)
        {
            CheckIfDisposed();
            CheckForIOError();

            if (contentToWrite == null)
            {
                throw new ArgumentNullException(nameof(contentToWrite));
            }

            if (!_port.IsOpen)
            {
                throw new InvalidOperationException("Port isn't open.");
            }

            var buffer = contentToWrite.ToArray();

            _port.Write(buffer, 0, buffer.Length);

            CheckForIOError();
        }

        public List<byte> Read(int requiredSize)
        {
            CheckIfDisposed();
            CheckForIOError();

            var result = new List<byte>();
            var alreadyRead = 0;
            var buffer = new byte[ReadBlockSize];

            while(true)
            {
                var actuallyRead = _port.Read(buffer, 0, ReadBlockSize);
                alreadyRead += actuallyRead;
                result.AddRange(buffer.ToList().GetRange(0, actuallyRead));

                if (alreadyRead == requiredSize)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(ReadSleepTime);
                }
            }

            CheckForIOError();

            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                // Free managed resources here
                _port.Close();
                _port.Dispose();
            }

            // Free unmanaged resources here

            _isDisposed = true;
        }

        private void CheckIfDisposed()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("Already disposed.");
            }
        }

        /// <summary>
        /// Checks for IO error in main thread
        /// </summary>
        private void CheckForIOError()
        {
            if (_IOError != null)
            {
                throw new SerialPortIOErrorException($"During IO { _IOError } error happened.");
            }
        }

        /// <summary>
        /// IO error handler
        /// </summary>
        private void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            _IOError = e.EventType;
        }
    }
}