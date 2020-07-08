using System.Collections.Generic;

namespace LibFloaderClient.Interfaces.SerialPortsLister
{
    /// <summary>
    /// Interface to get serial ports list
    /// </summary>
    public interface ISerialPortsLister
    {
        /// <summary>
        /// Returns list of serial ports names, ordered alphabetically
        /// </summary>
        /// <returns></returns>
        List<string> ListOrdered();
    }
}