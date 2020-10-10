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

using LibFloaderClient.Models.DAO;
using LibFloaderClient.Models.DAO.Versioned.V1;

namespace LibFloaderClient.Interfaces.DAO
{
    /// <summary>
    /// DAO interface
    /// </summary>
    public interface IDao
    {
        /// <summary>
        /// Returns vendor name data for given ID
        /// </summary>
        VendorDBO GetVendorNameData(int vendorId);

        /// <summary>
        /// Get human-readable device model name
        /// </summary>
        DeviceNameDBO GetDeviceNameData(int vendorId, int modelId);

        /// <summary>
        /// Get device data for V1 protocol
        /// </summary>
        DeviceDataV1DBO GetDeviceDataV1(int vendorId, int modelId);
    }
}