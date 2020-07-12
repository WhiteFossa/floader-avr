using ReactiveUI;
using System;
using FloaderClientGUI;
using Microsoft.Extensions.DependencyInjection;
using LibFloaderClient.Interfaces.SerialPortsLister;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Reactive.Linq;

namespace FloaderClientGUI.ViewModels
{
    public class PortSelectionWindowViewModel : ViewModelBase
    {
#region Bound properties
        private List<string> _ports;
        private string _selectedPort;

        /// <summary>
        /// Ports list (for listbox)
        /// </summary>
        public List<string> Ports
        {
            get => _ports;
            set => this.RaiseAndSetIfChanged(ref _ports, value);
        }

        /// <summary>
        /// Port, selected in listbox. May be null if none selected
        /// </summary>
        public string SelectedPort
        {
            get => _selectedPort;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPort, value);
                ProcessPortSelectionChange(value);
            }
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

        /// <summary>
        /// Called by listbox when selection changed
        /// </summary>
        public void ProcessPortSelectionChange(string newPort)
        {
            if (string.IsNullOrEmpty(newPort))
            {
                // Disabling OK button
            }
            else
            {
                // Enabling OK button
            }
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