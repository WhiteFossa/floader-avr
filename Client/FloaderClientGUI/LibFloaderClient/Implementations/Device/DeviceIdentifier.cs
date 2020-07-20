using System.Linq;
using System;
using System.Collections.Generic;
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Port;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Implementations.Enums.Device;
using LibFloaderClient.Interfaces.SerialPortDriver;
using LibFloaderClient.Implementations.SerialPortDriver;
using LibFloaderClient.Implementations.Exceptions;

namespace LibFloaderClient.Implementations.Device
{
    public class DeviceIdentifier : IDeviceIdentifier
    {
        /// <summary>
        /// Send this to device to ask for identification
        /// </summary>
        private readonly List<byte> IdentRequest = new List<byte>() { 0x49 };

        /// <summary>
        /// We must get 14 bytes as ident response
        /// </summary>
        private const int ExpectedIdentResponseLenght = 14;

        /// <summary>
        /// Our bootloader signature
        /// </summary>
        private readonly List<byte> Signature = new List<byte>() { 0x46, 0x42, 0x4C }; // "FBL"

        /// <summary>
        /// First signature byte position
        /// </summary>
        private const int SignatureOffset = 0;

        /// <summary>
        /// Version data start here
        /// </summary>
        private const int VersionOffset = 3;

        /// <summary>
        /// Version data length
        /// </summary>
        private const int VersionLength = 1;

        /// <summary>
        /// Version byte position in version data
        /// </summary>
        private const int VersionByteOffset = 0;

        /// <summary>
        /// Logger
        /// </summary>
        private ILogger _logger;

        public DeviceIdentifier(ILogger logger)
        {
            _logger = logger;
        }

        public DeviceIdentificator Identify(PortSettings portSettings)
        {
            if (portSettings == null)
            {
                throw new ArgumentNullException(nameof(portSettings));
            }

            using(ISerialPortDriver port = new SerialPortDriver.SerialPortDriver(portSettings))
            {
                _logger.LogInfo($"Requesting identification via { portSettings.Name }");
                port.Write(IdentRequest);

                try
                {
                    var response = port.Read(ExpectedIdentResponseLenght);

                    if (!CheckSignature(response))
                    {
                        _logger.LogError("Wrong signature, this is not Fossa's bootloader device.");
                        return new DeviceIdentificator(status: DeviceIdentificationStatus.WrongSignature,
                            version: -1,
                            vendorId: -1,
                            modelId: -1,
                            serial: -1);
                    }

                    // Our device, collecting data
                    var version = ExtractUnsignedByteAsInt(response.GetRange(VersionOffset, VersionLength), VersionByteOffset);

                    int a = 10;
                }
                catch(SerialPortTimeoutException ex)
                {
                    // Timeout, usually it means that this is not our device
                    _logger.LogError("Device didn't respond in time, usually it mean that this is not Fossa's bootloader device.");
                    return new DeviceIdentificator(status: DeviceIdentificationStatus.Timeout,
                        version: -1,
                        vendorId: -1,
                        modelId: -1,
                        serial: -1);
                }
            }

            return new DeviceIdentificator(DeviceIdentificationStatus.WrongSignature, 0, 0, 0, 0);
        }

        /// <summary>
        /// Checking for our bootloader signature in response.
        /// </summary>
        private bool CheckSignature(List<byte> response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (response.Count != ExpectedIdentResponseLenght)
            {
                throw new InvalidOperationException($"Identification response must be { ExpectedIdentResponseLenght } bytes long.");
            }

            var signatureInResponse = response.GetRange(SignatureOffset, Signature.Count);

            return signatureInResponse.SequenceEqual(Signature);
        }

        /// <summary>
        /// Takes byte at given position in response and returns it as int (byte is unsigned).null
        /// I.e. 0x03 -> 3, 0xFF -> 255 etc
        /// </summary>
        private int ExtractUnsignedByteAsInt(List<byte> response, int position)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if ((position < 0) || (position >= response.Count))
            {
                throw new ArgumentException("Position is outside of response.", nameof(position));
            }

            return (int)(response[position] & 0xFF);
        }
    }
}