﻿using LibFloaderClient.Implementations.Exceptions;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Interfaces.SerialPortDriver;
using LibFloaderClient.Interfaces.Versioned.Driver;
using LibFloaderClient.Models.Device.Versioned;
using LibFloaderClient.Models.Port;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// Port settings
        /// </summary>
        private PortSettings _portSettings;

        /// <summary>
        /// Device data
        /// </summary>
        private DeviceDataV1 _deviceData;

        /// <summary>
        /// True if driver is set up
        /// </summary>
        private bool _isSetUp;

        /// <summary>
        /// Serial port driver
        /// </summary>
        private ISerialPortDriver _portDriver;

        public DeviceDriverV1(ILogger logger)
        {
            _logger = logger;

            _isSetUp = false;
        }

        public bool Reboot()
        {
            IsSetUp();
            IsPortOccupied();

            _logger.LogInfo("Requesting device reboot...");
            _portDriver.Write(RebootRequest);

            try
            {
                var response = _portDriver.Read(RebootResponse.Count);

                if (response.SequenceEqual(RebootResponse))
                {
                    // Rebooted
                    _logger.LogInfo("Device reported reboot.");
                    return true;
                }

                _logger.LogError($"Device returned unknown response: { response }.");

                return false;
            }
            catch(SerialPortTimeoutException)
            {
                _logger.LogError("Device didn't respond in time, check did it reboot manually.");
                return false;
            }
        }

        public List<byte> ReadEEPROM()
        {
            IsSetUp();
            IsPortOccupied();

            _logger.LogInfo($"Trying to read { _deviceData.EepromSize } EEPROM bytes...");
            _portDriver.Write(ReadEEPROMRequest);

            try
            {
                var response = _portDriver.Read(_deviceData.EepromSize);

                _logger.LogInfo("Done");

                return response;
            }
            catch (SerialPortTimeoutException)
            {
                _logger.LogError("Timeout during EEPROM read.");
                throw;
            }
        }

        public void WriteEEPROM(List<byte> toWrite)
        {
            IsSetUp();
            IsPortOccupied();

            if (toWrite == null)
            {
                throw new ArgumentNullException(nameof(toWrite));
            }

            if (toWrite.Count() != _deviceData.EepromSize)
            {
                throw new ArgumentException($"Data to write size must equal EEPROM size: { _deviceData.EepromSize } bytes", nameof(toWrite));
            }

            _logger.LogInfo($"Trying to write { _deviceData.EepromSize } EEPROM bytes...");

            var lastByteAddress = _deviceData.EepromSize - 1;

            for (var byteAddress = 0; byteAddress < _deviceData.EepromSize; byteAddress ++)
            {
                _logger.LogInfo($"Writing { byteAddress + 1 } / { _deviceData.EepromSize } byte");

                var wantsMore = WriteEEPROMByte(byteAddress, toWrite[byteAddress]);

                if (!wantsMore && byteAddress != lastByteAddress)
                {
                    // Premature write termination
                    var message = $"Device don't accept new data after writing { byteAddress + 1 } / { _deviceData.EepromSize } bytes.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                if (wantsMore && byteAddress == lastByteAddress)
                {
                    // Greedy device, wants more after completion
                    var message = $"Device wants more data after writing the last byte of EEPROM.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }
            }

            _logger.LogInfo("Done");
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
                throw new InvalidOperationException($"Unexpected write EEPROM byte response size: { answer.Count() } bytes.");
            }

            if (answer.SequenceEqual(EEPROMWriteResponseNext))
            {
                return true;
            }
            else if (answer.SequenceEqual(EEPROMWriteResponseStop))
            {
                return false;
            }

            throw new InvalidOperationException($"Unexpected write EEPROM byte response.");
        }

        public List<byte> ReadFLASHPage(int pageAddress)
        {
            IsSetUp();
            IsPortOccupied();

            if (pageAddress < 0 || pageAddress >= _deviceData.FlashPagesAll)
            {
                throw new ArgumentOutOfRangeException(nameof(pageAddress), pageAddress, $"Allowed page addresses: [0 - { _deviceData.FlashPagesAll - 1 }]");
            }

            _logger.LogInfo($"Trying to read FLASH page with address { pageAddress }...");

            try
            {
                // Initiating
                var toWrite = new List<byte>(ReadFLASHRequest);
                toWrite.Add((byte)pageAddress); // Page address can't be bigger than 255
                _portDriver.Write(toWrite);

                var response = _portDriver.Read(ReadFLASHPageResponseSize);

                if (response.Count() != ReadFLASHPageResponseSize)
                {
                    throw new InvalidOperationException($"Unexpected response size to FLASH page read request: { response.Count() } bytes.");
                }

                if (response.SequenceEqual(ReadFLASHResponseFAIL))
                {
                    var message = $"Device reports failure during FLASH page { pageAddress } read.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }
                else if (!response.SequenceEqual(ReadFLASHResponseOK))
                {
                    var message = $"Unexpected response to FLASH page { pageAddress } read initiation.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                // Reading
                var data = _portDriver.Read(_deviceData.FlashPageSize);

                _logger.LogInfo("Done");

                return data;
            }
            catch (SerialPortTimeoutException)
            {
                _logger.LogError("Timeout during FLASH read.");
                throw;
            }
        }

        public void WriteFLASHPage(int pageAddress, List<byte> toWrite)
        {
            IsSetUp();
            IsPortOccupied();

            if (toWrite.Count() != _deviceData.FlashPageSize)
            {
                throw new ArgumentException($"Page size data is { _deviceData.FlashPageSize }", nameof(toWrite));
            }

            if (pageAddress < 0 || pageAddress >= _deviceData.FlashPagesWriteable)
            {
                throw new ArgumentOutOfRangeException(nameof(pageAddress), pageAddress, $"Writeable page addresses: [0 - { _deviceData.FlashPagesWriteable - 1 }]");
            }

            _logger.LogInfo($"Trying to write FLASH page with address { pageAddress }...");

            try
            {
                // Initiating
                _logger.LogInfo("Checking address...");

                var data = new List<byte>(WriteFLASHRequest);
                data.Add((byte)pageAddress); // Page address can't be bigger than 255
                _portDriver.Write(data);

                var response = _portDriver.Read(WriteFLASHPageCheckAddressResponseSize);

                if (response.Count() != WriteFLASHPageCheckAddressResponseSize)
                {
                    throw new InvalidOperationException($"Unexpected response size to FLASH page write address check request: { response.Count() } bytes.");
                }

                if (response.SequenceEqual(WriteFLASHPageCheckAddressResponseFAIL))
                {
                    var message = $"Device reports incorrect FLASH page address { pageAddress } while attempt to write page.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }
                else if (!response.SequenceEqual(WriteFLASHPageCheckAddressResponseOK))
                {
                    var message = $"Unexpected response to FLASH page { pageAddress } write address check.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                _logger.LogInfo("Done, uploading data...");

                _portDriver.Write(toWrite);

                response = _portDriver.Read(WriteFLASHPageResponseSize);

                if (response.Count() != WriteFLASHPageResponseSize)
                {
                    throw new InvalidOperationException($"Unexpected response size to FLASH page write request: { response.Count() } bytes.");
                }

                if (!response.SequenceEqual(WriteFLASHPageResponseOK))
                {
                    var message = $"Device reports failure during FLASH page write.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                _logger.LogInfo("Done");
            }
            catch(SerialPortTimeoutException)
            {
                _logger.LogError("Timeout during FLASH read.");
                throw;
            }
        }

        public void Setup(PortSettings port, DeviceDataV1 deviceData)
        {
            _isSetUp = false;
            _portSettings = port;
            _deviceData = deviceData;
            _isSetUp = true;
        }

        /// <summary>
        /// Throws an exception if driver isn't set up
        /// </summary>
        private void IsSetUp()
        {
            if (!_isSetUp)
            {
                var msg = "Attempt to use not set up V1 driver.";
                _logger.LogError(msg);
                throw new InvalidOperationException(msg);
            }
        }

        private void IsPortOccupied()
        {
            if (_portDriver == null)
            {
                var msg = "Attempt to use V1 driver when port is not occupied.";
                _logger.LogError(msg);
                throw new InvalidOperationException(msg);
            }
        }

        public void OccupyPort()
        {
            IsSetUp();

            if (_portDriver != null)
            {
                throw new InvalidOperationException("V1 driver - port already occupied.");
            }

            _portDriver = new SerialPortDriver.SerialPortDriver(_portSettings);
        }

        public void ReleasePort()
        {
            IsSetUp();
            IsPortOccupied();

            _portDriver.Dispose();
            _portDriver = null;
        }
    }
}
