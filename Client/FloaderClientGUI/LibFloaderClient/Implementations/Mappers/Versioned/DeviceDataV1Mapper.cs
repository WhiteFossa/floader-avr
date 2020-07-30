using LibFloaderClient.Models.DAO.Versioned.V1;
using LibFloaderClient.Models.Device.Versioned;

namespace LibFloaderClient.Implementations.Mappers.Versioned
{
    /// <summary>
    /// Mapper for Device Data for version 1 protocol
    /// </summary>
    public static class DeviceDataV1Mapper
    {
        /// <summary>
        /// Maps from DBO
        /// </summary>
        public static DeviceDataV1 MapFromDBO(DeviceDataV1DBO dbo)
        {
            return new DeviceDataV1(vendorId: dbo.VendorId,
                modelId: dbo.ModelId,
                flashPagesAll: dbo.FlashPagesAll,
                flashPagesWriteable: dbo.FlashPagesWriteable,
                flashPageSize: dbo.FlashPageSize,
                eepromSize: dbo.EepromSize);
        }
    }
}
