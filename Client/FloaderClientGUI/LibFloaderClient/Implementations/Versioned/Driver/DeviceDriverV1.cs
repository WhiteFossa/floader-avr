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
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Interfaces.SerialPortDriver;
using LibFloaderClient.Interfaces.Versioned.Driver;
using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Device.Versioned;
using LibFloaderClient.Models.Port;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LibFloaderClient.Implementations.Versioned.Driver
{
    public class DeviceDriverV1 : IDeviceDriverV1
    {
        private ILogger _logger;

        /// <summary>
        /// Send this to device to reboot it into the main firmware
        /// </summary>
        private readonly List<byte> RebootRequest = new List<byte>() { 0x51 };

        /// <summary>
        /// Device must response by this message if it accepted reboot command
        /// </summary>
        private readonly List<byte> RebootResponse = new List<byte>() { 0x42 };

        /// <summary>
        /// Send this to device to initiate EEPROM read
        /// </summary>
        private readonly List<byte> ReadEEPROMRequest = new List<byte>() { 0x72 };

        /// <summary>
        /// Send this to device to initiate EEPROM write
        /// </summary>
        private readonly List<byte> WriteEEPROMRequest = new List<byte>() { 0x77 };

        /// <summary>
        /// Response to EEPROM write have this size
        /// </summary>
        private readonly int EEPROMWriteResponseSize = 1;

        /// <summary>
        /// Device want next byte after EEPROM byte write
        /// </summary>
        private readonly List<byte> EEPROMWriteResponseNext = new List<byte>() { 0x6e };

        /// <summary>
        /// Device DO NOT want next byte after EEPROM byte write
        /// </summary>
        private readonly List<byte> EEPROMWriteResponseStop = new List<byte>() { 0x66 };

        /// <summary>
        /// FLASH page read request
        /// </summary>
        private readonly List<byte> ReadFLASHRequest = new List<byte>() { 0x52 };

        /// <summary>
        /// Response to read FLASH page have this size
        /// </summary>
        private readonly int ReadFLASHPageResponseSize = 1;

        /// <summary>
        /// FLASH page read, go to read data response
        /// </summary>
        private readonly List<byte> ReadFLASHResponseOK = new List<byte>() { 0x00 };

        /// <summary>
        /// FLASH page read failed response
        /// </summary>
        private readonly List<byte> ReadFLASHResponseFAIL = new List<byte>() { 0x01 };

        /// <summary>
        /// FLASH page write request
        /// </summary>
        private readonly List<byte> WriteFLASHRequest = new List<byte>() { 0x57 };

        /// <summary>
        /// Response to write FLASH page address check have this size
        /// </summary>
        private readonly int WriteFLASHPageCheckAddressResponseSize = 1;

        /// <summary>
        /// FLASH page write, go to upload data
        /// </summary>
        private readonly List<byte> WriteFLASHPageCheckAddressResponseOK = new List<byte>() { 0x00 };

        /// <summary>
        /// FLASH page write, address not allowed
        /// </summary>
        private readonly List<byte> WriteFLASHPageCheckAddressResponseFAIL = new List<byte>() { 0x01 };

        /// <summary>
        /// Response to write FLASH page have this size
        /// </summary>
        private readonly int WriteFLASHPageResponseSize = 1;

        /// <summary>
        /// Response to successfull FLASH page write
        /// </summary>
        private readonly List<byte> WriteFLASHPageResponseOK = new List<byte>() { 0x00 };

        /// <summary>
        /// Is already disposed?
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// Port settings
        /// </summary>
        private PortSettings _portSettings;

        /// <summary>
        /// Device data
        /// </summary>
        private DeviceDataV1 _deviceData;

        /// <summary>
        /// Serial port driver
        /// </summary>
        private ISerialPortDriver _portDriver;

        public DeviceDriverV1(PortSettings port, DeviceDataV1 deviceData, ILogger logger)
        {
            _logger = logger;
            _portSettings = port;
            _deviceData = deviceData;
            _portDriver = new SerialPortDriver.SerialPortDriver(_portSettings);
        }

        public bool Reboot()
        {
            CheckIfDisposed();

            _portDriver.Write(RebootRequest);

            try
            {
                var response = _portDriver.Read(RebootResponse.Count);

                if (response.SequenceEqual(RebootResponse))
                {
                    // Rebooted
                    return true;
                }

                _logger.LogError(string.Format(Language.UnknownDeviceResponceOnReboot, response));

                return false;
            }
            catch(SerialPortTimeoutException)
            {
                _logger.LogError(Language.TimeoutOnReboot);
                return false;
            }
        }

        public List<byte> ReadEEPROM()
        {
            CheckIfDisposed();

            _portDriver.Write(ReadEEPROMRequest);

            try
            {
                return _portDriver.Read(_deviceData.EepromSize);
            }
            catch (SerialPortTimeoutException)
            {
                _logger.LogError(Language.EepromReadTimeout);
                throw;
            }
        }

        public void WriteEEPROM(List<byte> toWrite, ProgressDelegate progressDelegate = null)
        {
            CheckIfDisposed();

            if (toWrite == null)
            {
                throw new ArgumentNullException(nameof(toWrite));
            }

            if (toWrite.Count() != _deviceData.EepromSize)
            {
                throw new ArgumentException(string.Format(Language.WrongEepromDataSize, _deviceData.EepromSize), nameof(toWrite));
            }

            progressDelegate?.Invoke(new ProgressData(0, _deviceData.EepromSize, Language.ProgressOperationWritingEeprom));

            var lastByteAddress = _deviceData.EepromSize - 1;

            for (var byteAddress = 0; byteAddress < _deviceData.EepromSize; byteAddress ++)
            {
                var wantsMore = WriteEEPROMByte(byteAddress, toWrite[byteAddress]);

                if (!wantsMore && byteAddress != lastByteAddress)
                {
                    // Premature write termination
                    var message = string.Format(Language.EepromWriteDontAcceptData, byteAddress + 1, _deviceData.EepromSize);
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                if (wantsMore && byteAddress == lastByteAddress)
                {
                    // Greedy device, wants more after completion
                    var message = Language.EepromWriteGreedyDevice;
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                progressDelegate?.Invoke(new ProgressData(byteAddress + 1, _deviceData.EepromSize, Language.ProgressOperationWritingEeprom));
            }
        }

        /// <summary>
        /// Attempts to write next EEPROM byte, returns true if device want more
        /// </summary>
        private bool WriteEEPROMByte(int byteAddress, byte toWrite)
        {
            List<byte> data = new List<byte>();

            if (byteAddress == 0)
            {
                // We need to initiate process
                data.AddRange(WriteEEPROMRequest);
            }

            data.Add(toWrite);
            _portDriver.Write(data);

            var answer = _portDriver.Read(EEPROMWriteResponseSize);

            if (answer.Count() != EEPROMWriteResponseSize)
            {
                throw new InvalidOperationException(string.Format(Language.EepromWriteByteUnexpectedResponseSize, answer.Count()));
            }

            if (answer.SequenceEqual(EEPROMWriteResponseNext))
            {
                return true;
            }
            else if (answer.SequenceEqual(EEPROMWriteResponseStop))
            {
                return false;
            }

            throw new InvalidOperationException(Language.EepromWriteByteUnexpectedResponse);
        }

        public List<byte> ReadFLASHPage(int pageAddress)
        {
            CheckIfDisposed();

            if (pageAddress < 0 || pageAddress >= _deviceData.FlashPagesAll)
            {
                throw new ArgumentOutOfRangeException(nameof(pageAddress), pageAddress,
                    string.Format(Language.AllowedFlashPageAddresses, _deviceData.FlashPagesAll - 1));
            }

            try
            {
                // Initiating
                var toWrite = new List<byte>(ReadFLASHRequest);
                toWrite.Add((byte)pageAddress); // Page address can't be bigger than 255
                _portDriver.Write(toWrite);

                var response = _portDriver.Read(ReadFLASHPageResponseSize);

                if (response.Count() != ReadFLASHPageResponseSize)
                {
                    throw new InvalidOperationException(string.Format(Language.FlashReadUnexpectedResponseSize, response.Count()));
                }

                if (response.SequenceEqual(ReadFLASHResponseFAIL))
                {
                    var message = string.Format(Language.FlashReadFailure, pageAddress);
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }
                else if (!response.SequenceEqual(ReadFLASHResponseOK))
                {
                    var message = string.Format(Language.FlashReadUnexpectedResponse, pageAddress);
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                // Reading
                return _portDriver.Read(_deviceData.FlashPageSize);
            }
            catch (SerialPortTimeoutException)
            {
                _logger.LogError(Language.FlashReadTimeout);
                throw;
            }
        }

        public void WriteFLASHPage(int pageAddress, List<byte> toWrite)
        {
            CheckIfDisposed();

            if (toWrite.Count() != _deviceData.FlashPageSize)
            {
                throw new ArgumentException(string.Format(Language.FlashWriteWrongPageSize, _deviceData.FlashPageSize), nameof(toWrite));
            }

            if (pageAddress < 0 || pageAddress >= _deviceData.FlashPagesWriteable)
            {
                throw new ArgumentOutOfRangeException(nameof(pageAddress), pageAddress,
                    string.Format(Language.FlashWriteWriteableAddressess, _deviceData.FlashPagesWriteable - 1));
            }

            try
            {
                // Initiating
                var data = new List<byte>(WriteFLASHRequest);
                data.Add((byte)pageAddress); // Page address can't be bigger than 255
                _portDriver.Write(data);

                var response = _portDriver.Read(WriteFLASHPageCheckAddressResponseSize);

                if (response.Count() != WriteFLASHPageCheckAddressResponseSize)
                {
                    throw new InvalidOperationException(string.Format(Language.FlashWriteUnexpectedAddressCheckResponseSize, response.Count()));
                }

                if (response.SequenceEqual(WriteFLASHPageCheckAddressResponseFAIL))
                {
                    var message = string.Format(Language.FlashWriteIncorrectPageAddress, pageAddress);
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }
                else if (!response.SequenceEqual(WriteFLASHPageCheckAddressResponseOK))
                {
                    var message = string.Format(Language.FlashWriteAddressCheckUnexpectedResponse, pageAddress);
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                _portDriver.Write(toWrite);

                response = _portDriver.Read(WriteFLASHPageResponseSize);

                if (response.Count() != WriteFLASHPageResponseSize)
                {
                    throw new InvalidOperationException(String.Format(Language.FlashWriteUnexpectedResponseSize, response.Count()));
                }

                if (!response.SequenceEqual(WriteFLASHPageResponseOK))
                {
                    var message = Language.FlashWriteFailure;
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }
            }
            catch(SerialPortTimeoutException)
            {
                _logger.LogError(Language.FlashWriteTimeout);
                throw;
            }
        }

        private void CheckIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(DeviceDriverV1));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Dispose managed here
                }

                // Dispose unmanaged here
                _portDriver.Dispose();
                _portDriver = null;

                _isDisposed = true;
            }
        }
    }
}
