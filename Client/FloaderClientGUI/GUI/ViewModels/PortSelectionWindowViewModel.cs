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
        private bool _isOkBtnEnabled;
        private bool _isOverrideDefaults;
        private bool _isBaudrateEnabled;
        private bool _isParityEnabled;
        private bool _isDataBitsEnabled;
        private bool _isStopBitsEnabled;

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

        /// <summary>
        /// Is OK button enabled or not
        /// </summary>
        public bool IsOkBtnEnabled
        {
            get => _isOkBtnEnabled;
            set => this.RaiseAndSetIfChanged(ref _isOkBtnEnabled, value);
        }

        /// <summary>
        /// True if Override Defaults checkbox is checked
        /// </summary>
        public bool IsOverrideDefaults
        {
            get => _isOverrideDefaults;
            set
            {
                this.RaiseAndSetIfChanged(ref _isOverrideDefaults, value);
                ProcessIsOverrideDefaultsChange(value);
            }
        }

        /// <summary>
        /// Is baudrate checkbox enabled
        /// </summary>
        public bool IsBaudrateEnabled
        {
            get => _isBaudrateEnabled;
            set => this.RaiseAndSetIfChanged(ref _isBaudrateEnabled, value);
        }

        /// <summary>
        /// Is parity checkbox enabled
        /// </summary>
        public bool IsParityEnabled
        {
            get => _isParityEnabled;
            set => this.RaiseAndSetIfChanged(ref _isParityEnabled, value);
        }

        /// <summary>
        /// Is data bits checkbox enabled
        /// </summary>
        public bool IsDataBitsEnabled
        {
            get => _isDataBitsEnabled;
            set => this.RaiseAndSetIfChanged(ref _isDataBitsEnabled, value);
        }

        /// <summary>
        /// Is stop bits checkbox enabled
        /// </summary>
        public bool IsStopBitsEnabled
        {
            get => _isStopBitsEnabled;
            set => this.RaiseAndSetIfChanged(ref _isStopBitsEnabled, value);
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
        /// Called by listbox when selection changed
        /// </summary>
        private void ProcessPortSelectionChange(string newPort)
        {
            IsOkBtnEnabled = !string.IsNullOrEmpty(newPort);
        }

        /// <summary>
        /// Called when Override Defaults checkbox changes state
        /// </summary>
        private void ProcessIsOverrideDefaultsChange(bool newValue)
        {
            IsBaudrateEnabled = newValue;
            IsParityEnabled = newValue;
            IsDataBitsEnabled = newValue;
            IsStopBitsEnabled = newValue;
        }

        /// <summary>
        /// Getting current list of ports
        /// </summary>
        private List<string> GetPortsList()
        {
            return new List<string>() { "Port C", "Port D", "Port E" };
        }

    }
}