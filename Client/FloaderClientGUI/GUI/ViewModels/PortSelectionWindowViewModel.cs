/*
                    Fossa's AVR bootloader client
Copyright (C) 2020 White Fossa aka Artyom Vetrov <whitefossa@protonmail.com>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Avalonia.Controls;
using LibFloaderClient.Implementations.Port;
using LibFloaderClient.Interfaces.SerialPortsLister;
using LibFloaderClient.Models.Port;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace FloaderClientGUI.ViewModels
{
    public class PortSelectionWindowViewModel : ViewModelBase
    {


        /// <summary>
        /// Port settings or null, if port not specified yet
        /// </summary>
        /// <value></value>
        public PortSettings PortSettings { get; private set; }

        /// <summary>
        /// Ports lister
        /// </summary>
        private ISerialPortsLister _serialPortsLister;


#region Bound properties
        private List<string> _ports;
        private string _selectedPort;
        private bool _isOkBtnEnabled;
        private bool _isOverrideDefaults;
        private bool _isBaudrateEnabled;
        private bool _isParityEnabled;
        private bool _isDataBitsEnabled;
        private bool _isStopBitsEnabled;
        private List<int> _baudrates;
        private int _selectedBaudrate;
        private List<string> _parities;
        private string _selectedParity;
        private List<int> _portDataBits;
        private int _selectedPortDataBits;
        private List<string> _portStopBits;
        private string _selectedPortStopBits;
        private bool _isResetToDefaultsEnabled;

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

        /// <summary>
        /// List of possible baudrates
        /// </summary>
        public List<int> Baudrates
        {
            get => _baudrates;
            set => this.RaiseAndSetIfChanged(ref _baudrates, value);
        }

        /// <summary>
        /// Selected baudrate
        /// </summary>
        public int SelectedBaudrate
        {
            get => _selectedBaudrate;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedBaudrate, value);
            }
        }

        /// <summary>
        /// List of possible parities
        /// </summary>
        public List<string> Parities
        {
            get => _parities;
            set => this.RaiseAndSetIfChanged(ref _parities, value);
        }

        /// <summary>
        /// Selected parity
        /// </summary>
        public string SelectedParity
        {
            get => _selectedParity;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedParity, value);
            }
        }

        /// <summary>
        /// List of possible port data bits
        /// </summary>
        public List<int> PortDataBits
        {
            get => _portDataBits;
            set => this.RaiseAndSetIfChanged(ref _portDataBits, value);
        }

        /// <summary>
        /// Selected data bits
        /// </summary>
        public int SelectedPortDataBits
        {
            get => _selectedPortDataBits;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPortDataBits, value);
            }
        }

        /// <summary>
        /// List of possible stop bits
        /// </summary>
        public List<string> PortStopBits
        {
            get => _portStopBits;
            set => this.RaiseAndSetIfChanged(ref _portStopBits, value);
        }

        /// <summary>
        /// Selected stop bits
        /// </summary>
        public string SelectedPortStopBits
        {
            get => _selectedPortStopBits;
            set => this.RaiseAndSetIfChanged(ref _selectedPortStopBits, value);
        }

        /// <summary>
        /// State of Reset to Defaults button
        /// </summary>
        public bool IsResetToDefaultsEnabled
        {
            get => _isResetToDefaultsEnabled;
            set => this.RaiseAndSetIfChanged(ref _isResetToDefaultsEnabled, value);
        }

#endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PortSelectionWindowViewModel() : base()
        {
            // DI
            _serialPortsLister = Program.Di.GetService<ISerialPortsLister>();

            Ports = GetPortsList();

            // Populating lists
            Baudrates = PossiblePortSettings.StandardBaudrates;

            Parities = PossiblePortSettings.PossibleParities
                .Select(p => PortSelectionHelper.MapParityToString(p))
                .ToList();

            PortDataBits = PossiblePortSettings.PossibleDataBits;

            PortStopBits = PossiblePortSettings.PossbileStopBits
                .Select(sb => PortSelectionHelper.MapStopBitsToString(sb))
                .ToList();

            // Resetting advanced settings
            ResetToDefaults();
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
        /// Reset advanced settings
        /// </summary>
        public void ResetAdvancedSettings()
        {
            ResetToDefaults();
        }

        /// <summary>
        /// Close window without applying settings
        /// </summary>
        public void Cancel(Window window)
        {
            window.Close();
        }

        /// <summary>
        /// Close window and apply settings
        /// </summary>
        public void OK(Window window)
        {
            // Port is always selected now, otherwise OK button will be disabled
            PortSettings = new PortSettings(name: SelectedPort, baudrate: SelectedBaudrate, parity: PortSelectionHelper.MapStringToParity(SelectedParity),
                dataBits: SelectedPortDataBits, stopBits: PortSelectionHelper.MapStringToStopBits(SelectedPortStopBits));

            window.Close();
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
            IsResetToDefaultsEnabled = newValue;
        }

        /// <summary>
        /// Getting current list of ports
        /// </summary>
        private List<string> GetPortsList()
        {
            return _serialPortsLister.ListOrdered();
        }

        /// <summary>
        /// Resets advanced settings to defaults and disables it
        /// </summary>
        private void ResetToDefaults()
        {
            SelectedBaudrate = PossiblePortSettings.DefaultBaudrate;
            SelectedParity = PortSelectionHelper.MapParityToString(PossiblePortSettings.DefaultParity);
            SelectedPortDataBits = PossiblePortSettings.DefaultDataBits;
            SelectedPortStopBits = PortSelectionHelper.MapStopBitsToString(PossiblePortSettings.DefaultStopBits);

            IsOverrideDefaults = false;
        }

    }
}