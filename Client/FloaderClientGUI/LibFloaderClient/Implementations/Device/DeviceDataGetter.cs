using LibFloaderClient.Implementations.Mappers.Versioned;
using LibFloaderClient.Interfaces.DAO;
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Models.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibFloaderClient.Implementations.Device
{
    public class DeviceDataGetter : IDeviceDataGetter
    {
        /// <summary>
        /// First version
        /// </summary>
        private const int Version1 = 1;

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

            if (ident.Version == Version1)
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
