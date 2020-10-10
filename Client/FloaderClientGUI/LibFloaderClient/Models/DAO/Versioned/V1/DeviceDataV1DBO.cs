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

namespace LibFloaderClient.Models.DAO.Versioned.V1
{
    /// <summary>
    /// Information about device inner constitution for V1 protocol
    /// </summary>
    public class DeviceDataV1DBO
    {
        /// <summary>
        /// Vendor ID
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Model ID
        /// </summary>
        public int ModelId { get; set; }

        /// <summary>
        /// Total amount of device FLASH pages
        /// </summary>
        public int FlashPagesAll { get; set; }

        /// <summary>
        /// Amount of writeable (lower) device FLASH pages
        /// </summary>
        public int FlashPagesWriteable { get; set; }

        /// <summary>
        /// FLASH page size in bytes
        /// </summary>
        public int FlashPageSize { get; set; }

        /// <summary>
        /// EEPROM size in bytes
        /// </summary>
        public int EepromSize { get; set; }
    }
}
