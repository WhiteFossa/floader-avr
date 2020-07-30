using LibFloaderClient.Models.Device;
using System;

namespace LibFloaderClient.Interfaces.Device
{
    /// <summary>
    /// Versioned device data getter
    /// </summary>
    public interface IDeviceDataGetter
    {
        /// <summary>
        /// Getting device identifier data and returns device data for corresponding protocol version.
        /// </summary>
        Object GetDeviceData(DeviceIdentifierData ident);
    }
}
