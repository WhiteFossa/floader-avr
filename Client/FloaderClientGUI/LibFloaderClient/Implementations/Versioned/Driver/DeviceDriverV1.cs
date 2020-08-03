using LibFloaderClient.Implementations.Exceptions;
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

        public DeviceDriverV1(ILogger logger)
        {
            _logger = logger;

            _isSetUp = false;
        }

        public bool Reboot()
        {
            IsSetUp();

            using (ISerialPortDriver port = new SerialPortDriver.SerialPortDriver(_portSettings))
            {
                _logger.LogInfo("Requesting device reboot...");
                port.Write(RebootRequest);

                try
                {
                    var response = port.Read(RebootResponse.Count);

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
        }

        public List<byte> ReadEEPROM()
        {
            IsSetUp();

            using (ISerialPortDriver port = new SerialPortDriver.SerialPortDriver(_portSettings))
            {
                _logger.LogInfo($"Trying to read { _deviceData.EepromSize } EEPROM bytes...");
                port.Write(ReadEEPROMRequest);

                try
                {
                    var response = port.Read(_deviceData.EepromSize);

                    _logger.LogInfo("Done");

                    return response;
                }
                catch (SerialPortTimeoutException)
                {
                    _logger.LogError("Timeout during EEPROM read.");
                    return null;
                }
            }
        }

        public void WriteEEPROM(List<byte> toWrite)
        {
            IsSetUp();

            if (toWrite == null)
            {
                throw new ArgumentNullException(nameof(toWrite));
            }

            if (toWrite.Count() != _deviceData.EepromSize)
            {
                throw new ArgumentException($"Data to write size must equal EEPROM size: { _deviceData.EepromSize } bytes", nameof(toWrite));
            }

            using (ISerialPortDriver port = new SerialPortDriver.SerialPortDriver(_portSettings))
            {
                _logger.LogInfo($"Trying to write { _deviceData.EepromSize } EEPROM bytes...");

                var lastByteAddress = _deviceData.EepromSize - 1;

                for (var byteAddress = 0; byteAddress < _deviceData.EepromSize; byteAddress ++)
                {
                    _logger.LogInfo($"Writing { byteAddress + 1 } / { _deviceData.EepromSize } byte");

                    var wantsMore = WriteEEPROMByte(port, byteAddress, toWrite[byteAddress]);

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
        }

        /// <summary>
        /// Attempts to write next EEPROM byte, returns true if device want more
        /// </summary>
        private bool WriteEEPROMByte(ISerialPortDriver port, int byteAddress, byte toWrite)
        {
            List<byte> data = new List<byte>();

            if (byteAddress == 0)
            {
                // We need to initiate process
                data.AddRange(WriteEEPROMRequest);
            }

            data.Add(toWrite);
            port.Write(data);

            var answer = port.Read(EEPROMWriteResponseSize);

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
    }
}
