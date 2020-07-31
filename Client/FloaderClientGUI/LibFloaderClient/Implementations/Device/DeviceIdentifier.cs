using System.Linq;
using System;
using System.Collections.Generic;
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Port;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Implementations.Enums.Device;
using LibFloaderClient.Interfaces.SerialPortDriver;
using LibFloaderClient.Implementations.Exceptions;
using LibFloaderClient.Implementations.Helpers;

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
        /// Version data starts here
        /// </summary>
        private const int VersionOffset = 3;

        /// <summary>
        /// Vendor ID starts here
        /// </summary>
        private const int VendorIdOffset = 4;

        /// <summary>
        /// Device model ID starts here
        /// </summary>
        private const int ModelIdOffset = 7;

        /// <summary>
        /// Device serial number starts here
        /// </summary>
        private const int SerialOffset = 10;

        /// <summary>
        /// Logger
        /// </summary>
        private ILogger _logger;

        public DeviceIdentifier(ILogger logger)
        {
            _logger = logger;
        }

        public DeviceIdentifierData Identify(PortSettings portSettings)
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
                        return new DeviceIdentifierData(status: DeviceIdentificationStatus.WrongSignature,
                            version: -1,
                            vendorId: -1,
                            modelId: -1,
                            serial: -1);
                    }

                    // Our device, collecting data
                    var version = PortDataHelper.ExtractUnsignedByteAsInt(response, VersionOffset);
                    var vendorId = PortDataHelper.ExtractThreeBytesAsInt(response, VendorIdOffset);
                    var modelId = PortDataHelper.ExtractThreeBytesAsInt(response, ModelIdOffset);
                    var serial = PortDataHelper.ExtractFourBytesAsLong(response, SerialOffset);

                    var result = new DeviceIdentifierData(status: DeviceIdentificationStatus.Identified,
                        version: version,
                        vendorId: vendorId,
                        modelId: modelId,
                        serial: serial);

                        _logger.LogInfo($@"Valid identification record received:
Version: { result.Version },
Vendor ID: { result.VendorId },
Model ID: { result.ModelId },
Serial: { result.Serial }.");

                    return result;
                }
                catch(SerialPortTimeoutException)
                {
                    // Timeout, usually it means that this is not our device
                    _logger.LogError("Device didn't respond in time, usually it mean that this is not Fossa's bootloader device.");
                    return new DeviceIdentifierData(status: DeviceIdentificationStatus.Timeout,
                        version: -1,
                        vendorId: -1,
                        modelId: -1,
                        serial: -1);
                }
            }
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
    }
}