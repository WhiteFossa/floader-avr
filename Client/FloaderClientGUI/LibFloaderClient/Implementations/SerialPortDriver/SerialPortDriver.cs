using System.Xml.Linq;
using System.Threading;
using System.Linq;
using System;
using System.Collections.Generic;
using LibFloaderClient.Interfaces.SerialPortDriver;
using LibFloaderClient.Models.Port;
using System.IO.Ports;

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

        private bool _isDisposed = false;

        /// <summary>
        /// We use this serial port
        /// </summary>
        private SerialPort _port;

        /// <summary>
        /// Timeout in milliseconds.
        /// </summary>
        private int _timeout = DefaultTimeout;

        /// <summary>
        /// Data, came from port, but not taken by Read() call
        /// </summary>
        private List<byte> _unreadData = new List<byte>();


        public SerialPortDriver(PortSettings settings)
        {
            _unreadData.Clear();

            _port = new SerialPort(portName: settings.Name, baudRate: settings.Baudrate, parity: settings.Parity,
                dataBits: settings.DataBits, stopBits: settings.StopBits);

            // Attaching port listener
            _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            _port.Open();
        }

        public void SetTimeout(int timeout)
        {
            CheckIfDisposed();

            _timeout = timeout;
        }

        public void Write(List<byte> contentToWrite)
        {
            CheckIfDisposed();

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
        }

        public List<byte> Read(int requiredSize)
        {
            CheckIfDisposed();

            int availableSize = 0;
            while(true)
            {
                availableSize = _unreadData.Count();

                if (availableSize >= requiredSize)
                {
                    break;
                }

                // Waiting for data
                Thread.Sleep(ReadSleepTime);
            }

            // Cut first requiredSize bytes and return it
            var result = _unreadData.GetRange(0, requiredSize);
            _unreadData = _unreadData.GetRange(requiredSize, availableSize - requiredSize);
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
        /// Called when new data is received
        /// </summary>
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var port = (SerialPort)sender;

            while(true)
            {
                var b = port.ReadByte();

                if (b == -1)
                {
                    return;
                }

                _unreadData.Add((byte)b);
            }
        }
    }
}