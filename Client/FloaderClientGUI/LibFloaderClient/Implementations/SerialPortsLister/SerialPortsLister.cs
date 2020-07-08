using System.Reflection;
using LibFloaderClient.Interfaces.SerialPortsLister;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace LibFloaderClient.Implementations.SerialPortsLister
{
    public class SerialPortsLister : ISerialPortsLister
    {
        public List<string> ListOrdered()
        {
            return SerialPort.GetPortNames()
                .OrderBy(sp => sp)
                .ToList();
        }
    }
}