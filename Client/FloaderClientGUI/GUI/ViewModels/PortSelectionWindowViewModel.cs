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
        }

#region Commands

        /// <summary>
        /// Command to refresh ports
        /// </summary>
        public void RefreshPortsList()
        {
            Ports = new List<string>() { "Port A", "Port B", "Port C" };
        }
#endregion

    }
}