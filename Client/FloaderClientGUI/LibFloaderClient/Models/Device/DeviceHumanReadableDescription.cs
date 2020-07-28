using System;
using System.Collections.Generic;
using System.Text;

namespace LibFloaderClient.Models.Device
{
    /// <summary>
    /// Human-readable device description
    /// </summary>
    public class DeviceHumanReadableDescription
    {
        /// <summary>
        /// Device vendor name
        /// </summary>
        public string Vendor { get; }

        /// <summary>
        /// Device model name
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// Device serial number
        /// </summary>
        public string Serial { get; }

        public DeviceHumanReadableDescription(string vendor, string model, long serial)
        {
            if (string.IsNullOrEmpty(vendor))
            {
                throw new ArgumentException(nameof(vendor));
            }

            if (string.IsNullOrEmpty(model))
            {
                throw new ArgumentException(nameof(model));
            }

            Vendor = vendor;
            Model = model;
            Serial = serial.ToString();
        }
    }
}
