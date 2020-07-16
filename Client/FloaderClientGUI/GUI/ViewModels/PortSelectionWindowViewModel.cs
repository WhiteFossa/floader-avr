using System;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using LibFloaderClient.Implementations.Port;
using System.IO.Ports;
using LibFloaderClient.Interfaces.SerialPortsLister;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Controls;
using LibFloaderClient.Models.Port;

namespace FloaderClientGUI.ViewModels
{
    public class PortSelectionWindowViewModel : ViewModelBase
    {
#region Constants
        /// <summary>
        /// Text representations of various parities
        /// </summary>
        private const string ParityNone = "No parity control";
        private const string ParityOdd = "Odd";
        private const string ParityEven = "Even";
        private const string ParityMark = "Mark";
        private const string ParitySpace = "Space";

        /// <summary>
        /// Dictionary for parity to name and name to parity mapping
        /// </summary>
        private readonly Dictionary<Parity, string> ParityNames = new Dictionary<Parity, string>()
        {
            { Parity.None, ParityNone },
            { Parity.Odd, ParityOdd },
            { Parity.Even, ParityEven },
            { Parity.Mark, ParityMark },
            { Parity.Space, ParitySpace }
        };

        /// <summary>
        /// Text representation of various stop bits
        /// </summary>
        private const string StopBitsOne = "One";
        private const string StopBitsOnePointFive = "One and half";
        private const string StopBitsTwo = "Two";

        /// <summary>
        /// Dictionary for stop bits to name and name to stop bits mapping
        /// </summary>
        private readonly Dictionary<StopBits, string> StopBitsNames = new Dictionary<StopBits, string>()
        {
            { StopBits.One, StopBitsOne },
            { StopBits.OnePointFive, StopBitsOnePointFive },
            { StopBits.Two, StopBitsTwo }
        };

#endregion

        /// <summary>
        /// Port settings or null, if port not specified yet
        /// </summary>
        /// <value></value>
        public PortSettings PortSettings { get; }

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
        public PortSelectionWindowViewModel(PortSettings portSettings) : base()
        {
            PortSettings = portSettings;

            // DI
            _serialPortsLister = Program.Di.GetService<ISerialPortsLister>();

            Ports = GetPortsList();

            // Populating lists
            Baudrates = PossiblePortSettings.StandardBaudrates;

            Parities = PossiblePortSettings.PossibleParities
                .Select(p => MapParityToString(p))
                .ToList();

            PortDataBits = PossiblePortSettings.PossibleDataBits;

            PortStopBits = PossiblePortSettings.PossbileStopBits
                .Select(sb => MapStopBitsToString(sb))
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

#region Mappers

        /// <summary>
        /// Parity to combobox value
        /// </summary>
        private string MapParityToString(Parity parity)
        {
            var result = ParityNames
                .Where(pn => pn.Key == parity);

            if (!result.Any())
            {
                throw new ArgumentException(nameof(parity));
            }

            return result.FirstOrDefault().Value;
        }

        /// <summary>
        /// Combobox value to parity
        /// </summary>
        private Parity MapStringToParity(string parityStr)
        {
            var result = ParityNames
                .Where(pn => string.Equals(pn.Value, parityStr));

            if (!result.Any())
            {
                throw new ArgumentException(nameof(parityStr));
            }

            return result.FirstOrDefault().Key;
        }

        /// <summary>
        /// Stop bits count to combobox value
        /// </summary>
        private string MapStopBitsToString(StopBits stopBits)
        {
            var result = StopBitsNames
                .Where(sb => sb.Key == stopBits);

            if (!result.Any())
            {
                throw new ArgumentException(nameof(stopBits));
            }

            return result.FirstOrDefault().Value;
        }

        /// <summary>
        /// Combobox value to stop bits count
        /// </summary>
        private StopBits MapStringToStopBits(string str)
        {
            var result = StopBitsNames
                .Where(sb => string.Equals(sb.Value, str));

            if (!result.Any())
            {
                throw new ArgumentException(nameof(str));
            }

            return result.FirstOrDefault().Key;
        }

#endregion

        /// <summary>
        /// Resets advanced settings to defaults and disables it
        /// </summary>
        private void ResetToDefaults()
        {
            SelectedBaudrate = PossiblePortSettings.DefaultBaudrate;
            SelectedParity = MapParityToString(PossiblePortSettings.DefaultParity);
            SelectedPortDataBits = PossiblePortSettings.DefaultDataBits;
            SelectedPortStopBits = MapStopBitsToString(PossiblePortSettings.DefaultStopBits);

            IsOverrideDefaults = false;
        }

    }
}