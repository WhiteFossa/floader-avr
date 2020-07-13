using System;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using LibFloaderClient.Implementations.Port;
using System.IO.Ports;

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
#endregion

#region Bound properties
        private List<string> _ports;
        private string _selectedPort;
        private bool _isOkBtnEnabled;
        private bool _isOverrideDefaults;
        private bool _isBaudrateEnabled;
        private bool _isParityEnabled;
        private bool _isDataBitsEnabled;
        private bool _isStopBitsEnabled;
        private List<string> _baudrates;
        private string _selectedBaudrate;
        private List<string> _parities;
        private string _selectedParity;

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
        public List<string> Baudrates
        {
            get => _baudrates;
            set => this.RaiseAndSetIfChanged(ref _baudrates, value);
        }

        /// <summary>
        /// Selected baudrate
        /// </summary>
        public string SelectedBaudrate
        {
            get => _selectedBaudrate;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedBaudrate, value);

                // TODO: Write to model here
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

        public string SelectedParity
        {
            get => _selectedParity;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedParity, value);

                // TODO: Write to model here
            }
        }

#endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PortSelectionWindowViewModel() : base()
        {
            Ports = GetPortsList();

            // Populating lists
            Baudrates = PossiblePortSettings.StandardBaudrates
                .Select(b => MapBaudrateToString(b))
                .ToList();

            SelectedBaudrate = MapBaudrateToString(PossiblePortSettings.DefaultBaudrate);

            Parities = PossiblePortSettings.PossibleParities
                .Select(p => MapParityToString(p))
                .ToList();

            SelectedParity = MapParityToString(PossiblePortSettings.DefaultParity);
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

#region Mappers

        /// <summary>
        /// Baudrate to combobox value
        /// </summary>
        private string MapBaudrateToString(int baudrate)
        {
            return baudrate.ToString();
        }

        /// <summary>
        /// Combobox value to baudrate
        /// </summary>
        private int MapStringToBaudrate(string baudrateStr)
        {
            return int.Parse(baudrateStr);
        }

        /// <summary>
        /// Parity to combobox value
        /// </summary>
        private string MapParityToString(Parity parity)
        {
            switch (parity)
            {
                case Parity.None:
                    return ParityNone;

                case Parity.Odd:
                    return ParityOdd;

                case Parity.Even:
                    return ParityEven;

                case Parity.Mark:
                    return ParityMark;
                
                case Parity.Space:
                    return ParitySpace;

                default:
                    throw new ArgumentException(nameof(parity));
            }
        }

        private Parity MapStringToParity(string parityStr)
        {
            switch (parityStr)
            {
                case ParityNone:
                    return Parity.None;

                case ParityOdd:
                    return Parity.Odd;

                case ParityEven:
                    return Parity.Even;

                case ParityMark:
                    return Parity.Mark;

                case ParitySpace:
                    return Parity.Space;

                default:
                    throw new ArgumentException(nameof(parityStr));
            }
        }

#endregion

    }
}