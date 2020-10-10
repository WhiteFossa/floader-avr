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

namespace LibFloaderClient.Models.Device
{
    /// <summary>
    /// Data, identifying connected device
    /// </summary>
    public class DeviceIdentifierData
    {
        /// <summary>
        /// Identification result
        /// </summary>
        public DeviceIdentificationStatus Status { get; private set; }

        /// <summary>
        /// Bootloader protocol version
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Vendor ID
        /// </summary>
        public int VendorId { get; private set; }

        /// <summary>
        /// Device model ID
        /// </summary>
        public int ModelId { get; private set; }

        /// <summary>
        /// Device serial number
        /// </summary>
        public long Serial { get; private set; }

        public DeviceIdentifierData(DeviceIdentificationStatus status, int version, int vendorId, int modelId, long serial)
        {
            Status = status;
            Version = version;
            VendorId = vendorId;
            ModelId = modelId;
            Serial = serial;
        }
    }
}