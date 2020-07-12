using ReactiveUI;
using System;
using FloaderClientGUI;
using Microsoft.Extensions.DependencyInjection;
using LibFloaderClient.Interfaces.SerialPortsLister;
using System.Collections.Generic;

namespace FloaderClientGUI.ViewModels
{
    public class PortSelectionWindowViewModel : ViewModelBase
    {
#region Bound properties
        private List<string> _ports;

        /// <summary>
        /// Ports list (for listbox)
        /// </summary>
        public List<string> Ports
        {
            get => _ports;
            set => this.RaiseAndSetIfChanged(ref _ports, value);
        }
#endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PortSelectionWindowViewModel() : base()
        {
            Ports = GetPortsList();
        }

#region Commands

        /// <summary>
        /// Command to refresh ports
        /// </summary>
        public void RefreshPortsList()
        {
            Ports = GetPortsList();
        }
#endregion

        /// <summary>
        /// Getting current list of ports
        /// </summary>
        private List<string> GetPortsList()
        {
            return new List<string>() { "Port C", "Port D", "Port E" };
        }

    }
}