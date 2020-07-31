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
