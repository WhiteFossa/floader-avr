/*
                    Fossa's AVR bootloader client
Copyright (C) 2020 White Fossa aka Artyom Vetrov <whitefossa@protonmail.com>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using LibFloaderClient.Implementations.Exceptions;
using LibFloaderClient.Implementations.Resources;
using LibFloaderClient.Interfaces.SerialPortDriver;
using LibFloaderClient.Models.Port;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Timers;

namespace LibFloaderClient.Implementations.SerialPortDriver
{
    public class SerialPortDriver : ISerialPortDriver
    {
        /// <summary>
        /// If no one byte sent/received during this time, then throws timeout exception
        /// </summary>
        private const int SingleOperationTimeout = 10000;

        /// <summary>
        /// If required data wasn't sent/received during this time, then throws timeout exception
        /// </summary>
        private const int MultipleOperationsTimeout = 60000;

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

        /// <summary>
        /// Read stash. If during read operation more data, that required, is read, then remainder is stashed here and added to the next read operation data.
        /// </summary>
        private List<byte> _readStash = new List<byte>();

        /// <summary>
        /// Timer to count read timeout
        /// </summary>
        private System.Timers.Timer _readTimeoutTimer;

        /// <summary>
        /// True if read timeout happened
        /// </summary>
        private bool _isReadTimeoutHappened;

        public SerialPortDriver(PortSettings settings)
        {
            _port = new SerialPort(portName: settings.Name, baudRate: settings.Baudrate, parity: settings.Parity,
                dataBits: settings.DataBits, stopBits: settings.StopBits);
            _port.ReadTimeout = SingleOperationTimeout;
            _port.WriteTimeout = SingleOperationTimeout;

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
                throw new InvalidOperationException(Language.PortIsntOpen);
            }

            var buffer = contentToWrite.ToArray();

            try
            {
                _port.Write(buffer, 0, buffer.Length);
            }
            catch(TimeoutException ex)
            {
                throw new SerialPortTimeoutException(Language.WriteTimeout, ex);
            }


            CheckForIOError();
        }

        public List<byte> Read(int requiredSize)
        {
            CheckIfDisposed();
            CheckForIOError();

            _isReadTimeoutHappened = false;

            _readTimeoutTimer = new System.Timers.Timer(MultipleOperationsTimeout);
            _readTimeoutTimer.AutoReset = false;
            _readTimeoutTimer.Elapsed += OnReadTimeoutEvent;
            _readTimeoutTimer.Enabled = true;

            var result = new List<byte>();

            try
            {
                result.AddRange(_readStash);
                var alreadyRead = _readStash.Count();
                _readStash.Clear();

                var buffer = new byte[ReadBlockSize];
                var isFirstRead = true;

                while (true)
                {
                    if (_isReadTimeoutHappened)
                    {
                        throw new SerialPortTimeoutException(Language.MultipleReadTimeout);
                    }

                    if (alreadyRead >= requiredSize)
                    {
                        if (alreadyRead > requiredSize)
                        {
                            // Stashing remainder
                            _readStash = result.GetRange(requiredSize, alreadyRead - requiredSize);
                            result = result.GetRange(0, requiredSize);
                        }

                        break;
                    }
                    else
                    {
                        if (!isFirstRead)
                        {
                            Thread.Sleep(ReadSleepTime);
                        }
                        else
                        {
                            isFirstRead = false;
                        }
                    }

                    var actuallyRead = _port.Read(buffer, 0, ReadBlockSize);
                    alreadyRead += actuallyRead;
                    result.AddRange(buffer.ToList().GetRange(0, actuallyRead));
                }
            }
            catch (TimeoutException ex)
            {
                // Signle operation timeout
                throw new SerialPortTimeoutException(Language.SingleReadTimeout, ex);
            }

            _readTimeoutTimer.Enabled = false;

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
                throw new InvalidOperationException(Language.AlreadyDisposed);
            }
        }

        /// <summary>
        /// Checks for IO error in main thread
        /// </summary>
        private void CheckForIOError()
        {
            if (_IOError != null)
            {
                throw new SerialPortIOErrorException(string.Format(Language.IOError, _IOError));
            }
        }

        /// <summary>
        /// IO error handler
        /// </summary>
        private void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            _IOError = e.EventType;
        }


        private void OnReadTimeoutEvent(Object source, ElapsedEventArgs e)
        {
            _isReadTimeoutHappened = true;
        }
    }
}