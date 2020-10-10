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
using LibFloaderClient.Implementations.Mappers.Versioned;
using LibFloaderClient.Interfaces.DAO;
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Models.Device;
using System;

namespace LibFloaderClient.Implementations.Device
{
    public class DeviceDataGetter : IDeviceDataGetter
    {
        private readonly ILogger _logger;
        private readonly IDao _dao;

        public DeviceDataGetter(ILogger logger,
            IDao dao)
        {
            _logger = logger;
            _dao = dao;
        }

        public object GetDeviceData(DeviceIdentifierData ident)
        {
            if (ident == null)
            {
                throw new ArgumentNullException(nameof(ident));
            }

            _logger.LogInfo("Getting version-specific data...");

            if (ident.Version == (int)ProtocolVersion.First)
            {
                // Version 1 specific code
                _logger.LogInfo("Version 1 detected.");

                var dbo = _dao.GetDeviceDataV1(ident.VendorId, ident.ModelId);
                if (dbo == null)
                {
                    var msg = $"Unable to get device data for VendorId={ ident.VendorId }, ModelId={ ident.ModelId }";
                    _logger.LogError(msg);
                    throw new InvalidOperationException(msg);
                }

                var result = DeviceDataV1Mapper.MapFromDBO(dbo);

                _logger.LogInfo($"Total FLASH pages: { result.FlashPagesAll }");
                _logger.LogInfo($"Writeable FLASH pages: { result.FlashPagesWriteable }");
                _logger.LogInfo($"FLASH page size: { result.FlashPageSize } bytes");
                _logger.LogInfo($"EEPROM size: { result.EepromSize } bytes");

                return result;
            }
            else
            {
                var msg = $"Unsupported version: {ident.Version}";
                _logger.LogError(msg);
                throw new ArgumentException(msg, nameof(ident));
            }
        }
    }
}
