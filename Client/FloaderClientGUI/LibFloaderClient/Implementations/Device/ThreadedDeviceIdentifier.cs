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

using LibFloaderClient.Implementations.Enums.Device;
using LibFloaderClient.Implementations.Exceptions;
using LibFloaderClient.Implementations.Helpers;
using LibFloaderClient.Implementations.Resources;
using LibFloaderClient.Interfaces.SerialPortDriver;
using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Port;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibFloaderClient.Implementations.Device
{
    /// <summary>
    /// Called by ThreadedDeviceIdentifier when device identified.
    /// </summary>
    public delegate void DeviceIdentifiedCallbackDelegate(DeviceIdentifierData data);

    /// <summary>
    /// Called by ThreadedDeviceIdentifier if unhandled exception occured
    /// </summary>
    public delegate void DeviceIdentificationExceptionCallbackDelegate(Exception exception);

    public class ThreadedDeviceIdentifier
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
        /// Try to identify device on this port
        /// </summary>
        private PortSettings _portSettings;

        /// <summary>
        /// Called when device identified
        /// </summary>
        private DeviceIdentifiedCallbackDelegate _deviceIdentifiedCallbackDelegate;

        /// <summary>
        /// Called if exception happened
        /// </summary>
        private DeviceIdentificationExceptionCallbackDelegate _deviceIdentificationExceptionCallbackDelegate;
        
        public ThreadedDeviceIdentifier(PortSettings portSettings,
            DeviceIdentifiedCallbackDelegate deviceIdentifiedCallbackDelegate,
            DeviceIdentificationExceptionCallbackDelegate deviceIdentificationExceptionCallbackDelegate)
        {
            _portSettings = portSettings ?? throw new ArgumentNullException(nameof(portSettings));
            _deviceIdentifiedCallbackDelegate = deviceIdentifiedCallbackDelegate ?? throw new ArgumentNullException(nameof(deviceIdentifiedCallbackDelegate));
            _deviceIdentificationExceptionCallbackDelegate =
                deviceIdentificationExceptionCallbackDelegate ?? throw new ArgumentNullException(nameof(deviceIdentificationExceptionCallbackDelegate));
        }

        /// <summary>
        /// Attempt to identify device
        /// </summary>
        public void Identify()
        {
            try
            {
                DeviceIdentifierData result = null;

                using (ISerialPortDriver port = new SerialPortDriver.SerialPortDriver(_portSettings))
                {
                    port.Write(IdentRequest);

                    try
                    {
                        var response = port.Read(ExpectedIdentResponseLenght);

                        if (!CheckSignature(response))
                        {
                            result = new DeviceIdentifierData(status: DeviceIdentificationStatus.WrongSignature,
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

                        result = new DeviceIdentifierData(status: DeviceIdentificationStatus.Identified,
                            version: version,
                            vendorId: vendorId,
                            modelId: modelId,
                            serial: serial);
                    }
                    catch (SerialPortTimeoutException)
                    {
                        // Timeout, usually it means that this is not our device
                        result = new DeviceIdentifierData(status: DeviceIdentificationStatus.Timeout,
                            version: -1,
                            vendorId: -1,
                            modelId: -1,
                            serial: -1);
                    }
                }

                _deviceIdentifiedCallbackDelegate(result);
            }
            catch (Exception ex)
            {
                _deviceIdentificationExceptionCallbackDelegate(ex);
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
                throw new InvalidOperationException(string.Format(Language.WrongIdentificationResponseLength, ExpectedIdentResponseLenght));
            }

            var signatureInResponse = response.GetRange(SignatureOffset, Signature.Count);

            return signatureInResponse.SequenceEqual(Signature);
        }
    }
}
