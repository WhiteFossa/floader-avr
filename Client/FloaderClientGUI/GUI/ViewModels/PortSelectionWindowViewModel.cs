using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace FloaderClientGUI.ViewModels
{
    public class PortSelectionWindowViewModel : ViewModelBase
    {
#region Constants

        /// <summary>
        /// Possible baudrates
        /// </summary>
        private readonly List<int> StandardBaudrates = new List<int>()
            {110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };

        /// <summary>
        /// Default baudrate
        /// </summary>
        private const int DefaultBaudrate = 57600;
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
#endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PortSelectionWindowViewModel() : base()
        {
            Ports = GetPortsList();

            // Populating lists
            Baudrates = StandardBaudrates
                .Select(b => MapBaudrateToString(b))
                .ToList();

            SelectedBaudrate = MapBaudrateToString(DefaultBaudrate);
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

#endregion

    }
}