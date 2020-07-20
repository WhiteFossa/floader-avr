using System;
using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Port;

namespace LibFloaderClient.Interfaces.Device
{
    public interface IDeviceIdentifier
    {
        /// <summary>
        /// Attempt to identify device
        /// </summary>
        DeviceIdentificator Identify(PortSettings portSettings);
    }
}