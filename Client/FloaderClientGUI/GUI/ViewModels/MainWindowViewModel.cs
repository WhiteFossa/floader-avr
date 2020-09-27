using Avalonia.Controls;
using FloaderClientGUI.Models;
using FloaderClientGUI.Views;
using LibFloaderClient.Implementations.Enums.Device;
using LibFloaderClient.Implementations.Port;
using LibFloaderClient.Interfaces.DAO;
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Interfaces.Versioned.Common;
using LibFloaderClient.Models.Device;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Linq;
using System.Text;
using TextCopy;

namespace FloaderClientGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ILogger _logger;
        private IDeviceIdentifier _deviceIdentifier;
        private IVersionValidator _versionValidator;
        private IDao _dao;
        private IDeviceDataGetter _deviceDataGetter;
        private IDeviceIndependentOperationsProvider _deviceIndependentOperationsProvider;

        public PortSelectionWindowViewModel PortSelectionVM { get; }

#region Bound properties
        private string _consoleText;
        private string _portName;
        private string _vendorName;
        private string _modelName;
        private string _serialNumber;
        private bool _isFlashUpload;
        private bool _isEepromUpload;
        private string _flashUploadFile;
        private string _eepromUploadFile;
        private string _uploadBackupsDirectory;
        private string _flashDownloadFile;
        private string _eepromDownloadFile;
        private bool _isPollDeviceEnabled;
        private bool _isUploadEnabled;
        private bool _isDownloadEnabled;
        private bool _isRebootEnabled;
        private int _consoleCaretIndex;
        private bool _isFlashUploadFileEnabled;
        private bool _isEepromUploadFileEnabled;
        private bool _isSelectFlashForUploadButtonEnabled;
        private bool _isSelectEepromForUploadButtonEnabled;

        /// <summary>
        /// Text in console
        /// </summary>
        public string ConsoleText
        {
            get => _consoleText;
            set => this.RaiseAndSetIfChanged(ref _consoleText, value);
        }

        /// <summary>
        /// Port name
        /// </summary>
        public string PortName
        {
            get => _portName;
            set => this.RaiseAndSetIfChanged(ref _portName, value);
        }

        /// <summary>
        /// Device vendor name
        /// </summary>
        public string VendorName
        {
            get => _vendorName;
            set => this.RaiseAndSetIfChanged(ref _vendorName, value);
        }

        /// <summary>
        /// Device model name
        /// </summary>
        public string ModelName
        {
            get => _modelName;
            set => this.RaiseAndSetIfChanged(ref _modelName, value);
        }

        /// <summary>
        /// Device serial number
        /// </summary>
        public string SerialNumber
        {
            get => _serialNumber;
            set => this.RaiseAndSetIfChanged(ref _serialNumber, value);
        }

        /// <summary>
        /// Do we need to upload FLASH?
        /// </summary>
        public bool IsFlashUpload
        {
            get => _isFlashUpload;
            set
            {
                this.RaiseAndSetIfChanged(ref _isFlashUpload, value);
                SetUploadFlashFilePathState();
                SetUploadButtonState();
            }
        }

        /// <summary>
        /// Do we need to upload EEPROM?
        /// </summary>
        public bool IsEepromUpload
        {
            get => _isEepromUpload;
            set
            {
                this.RaiseAndSetIfChanged(ref _isEepromUpload, value);
                SetUploadEepromFilePathState();
                SetUploadButtonState();
            }
        }

        /// <summary>
        /// Get FLASH for upload from this file
        /// </summary>
        public string FlashUploadFile
        {
            get => _flashUploadFile;
            set => this.RaiseAndSetIfChanged(ref _flashUploadFile, value);
        }

        /// <summary>
        /// Get EEPROM for upload from this file
        /// </summary>
        public string EepromUploadFile
        {
            get => _eepromUploadFile;
            set => this.RaiseAndSetIfChanged(ref _eepromUploadFile, value);
        }

        /// <summary>
        /// Place pre-upload backups here
        /// </summary>
        public string UploadBackupsDirectory
        {
            get => _uploadBackupsDirectory;
            set => this.RaiseAndSetIfChanged(ref _uploadBackupsDirectory, value);
        }

        /// <summary>
        /// Put downloaded FLASH here
        /// </summary>
        public string FlashDownloadFile
        {
            get => _flashDownloadFile;
            set
            {
                this.RaiseAndSetIfChanged(ref _flashDownloadFile, value);
                SetDownloadButtonState();
            }
        }

        /// <summary>
        /// Put downloaded EEPROM here
        /// </summary>
        public string EepromDownloadFile
        {
            get => _eepromDownloadFile;
            set
            {
                this.RaiseAndSetIfChanged(ref _eepromDownloadFile, value);
                SetDownloadButtonState();
            }
        }

        /// <summary>
        /// If "Poll device" button enabled
        /// </summary>
        public bool IsPollDeviceEnabled
        {
            get => _isPollDeviceEnabled;
            set => this.RaiseAndSetIfChanged(ref _isPollDeviceEnabled, value);
        }

        /// <summary>
        /// Is "Upload" button enabled
        /// </summary>
        public bool IsUploadEnabled
        {
            get => _isUploadEnabled;
            set => this.RaiseAndSetIfChanged(ref _isUploadEnabled, value);
        }

        /// <summary>
        /// Is "Download" button enabled
        /// </summary>
        public bool IsDownloadEnabled
        {
            get => _isDownloadEnabled;
            set => this.RaiseAndSetIfChanged(ref _isDownloadEnabled, value);
        }

        /// <summary>
        /// Is "Reboot" button enabled
        /// </summary>
        public bool IsRebootEnabled
        {
            get => _isRebootEnabled;
            set => this.RaiseAndSetIfChanged(ref _isRebootEnabled, value);
        }

        /// <summary>
        /// Consone caret index (to scroll programmatically)
        /// </summary>
        public int ConsoleCaretIndex
        {
            get => _consoleCaretIndex;
            set => this.RaiseAndSetIfChanged(ref _consoleCaretIndex, value);
        }

        /// <summary>
        /// Is FLASH upload file text field enabled
        /// </summary>
        public bool IsFlashUploadFileEnabled
        {
            get => _isFlashUploadFileEnabled;
            set => this.RaiseAndSetIfChanged(ref _isFlashUploadFileEnabled, value);
        }

        /// <summary>
        /// Is EEPROM upload file text field enabled
        /// </summary>
        public bool IsEepromUploadFileEnabled
        {
            get => _isEepromUploadFileEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEepromUploadFileEnabled, value);
        }

        /// <summary>
        /// "Select FLASH file for upload" button state
        /// </summary>
        public bool IsSelectFlashForUploadButtonEnabled
        {
            get => _isSelectFlashForUploadButtonEnabled;
            set => this.RaiseAndSetIfChanged(ref _isSelectFlashForUploadButtonEnabled, value);
        }

        /// <summary>
        /// "Select EEPROM file for upload" button state
        /// </summary>
        public bool IsSelectEepromForUploadButtonEnabled
        {
            get => _isSelectEepromForUploadButtonEnabled;
            set => this.RaiseAndSetIfChanged(ref _isSelectEepromForUploadButtonEnabled, value);
        }

        #endregion Bound properties


        /// <summary>
        /// Main application model
        /// </summary>
        private MainModel _mainModel { get; }

        /// <summary>
        /// Are we ready to upload/download?
        /// </summary>
        private bool _isReadyInner;
        private bool _isReady
        {
            get => _isReadyInner;
            set
            {
                _isReadyInner = value;

                SetActionsButtonsState(_isReadyInner);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel(MainModel model) : base()
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            _mainModel = model;

            // DI
            _logger = Program.Di.GetService<ILogger>();
            _deviceIdentifier = Program.Di.GetService<IDeviceIdentifier>();
            _versionValidator = Program.Di.GetService<IVersionValidator>();
            _dao = Program.Di.GetService<IDao>();
            _deviceDataGetter = Program.Di.GetService<IDeviceDataGetter>();
            _deviceIndependentOperationsProvider = Program.Di.GetService<IDeviceIndependentOperationsProvider>();

            // Setting up logger
            _logger.SetLoggingFunction(AddLineToConsole);

            // Setting up port selection VM
            PortSelectionVM = new PortSelectionWindowViewModel();

            SetPollDeviceState();

            _isReady = false;
        }

#region Commands
        /// <summary>
        /// Command to clear console
        /// </summary>
        public void ClearConsole()
        {
            ConsoleText = string.Empty;
        }

        /// <summary>
        /// Copies console content to clipboard
        /// </summary>
        public void CopyConsoleToClipboard()
        {
            ClipboardService.SetText(ConsoleText);
        }

        /// <summary>
        /// Command to select port
        /// </summary>
        public async void SelectPortAsync()
        {
            var portSelectionDialog = new PortSelectionWindow()
            {

                DataContext= PortSelectionVM
            };

            await portSelectionDialog.ShowDialog(Program.GetMainWindow());

            _mainModel.PortSettings = PortSelectionVM.PortSettings != null ? PortSelectionVM.PortSettings : _mainModel.PortSettings;

            PortName = _mainModel.PortSettings?.Name != null ? _mainModel.PortSettings?.Name : "";

            LogPortSettings();

            SetPollDeviceState();

            _isReady = false;

            // No identification data known yet
            _mainModel.DeviceIdentData = null;
            _mainModel.DeviceHumanReadableDescription = null;
            _mainModel.VersionSpecificDeviceData = null;
        }

        /// <summary>
        /// Command to poll device
        /// </summary>
        public void PollDevice()
        {
            if (_mainModel.PortSettings == null)
            {
                throw new InvalidOperationException("Port not specified.");
            }

            try
            {
                _mainModel.DeviceIdentData = _deviceIdentifier.Identify(_mainModel.PortSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                LockProceeding();
                return;
            }

            // Is version acceptable?
            if (!_versionValidator.Validate(_mainModel.DeviceIdentData.Version))
            {
                _logger.LogError($"Bootloader protocol version { _mainModel.DeviceIdentData.Version } is not supported.");
                LockProceeding();
                return;
            }

            // Human-readable port info
            _logger.LogInfo($"Queriying vendor data for Vendor ID={ _mainModel.DeviceIdentData.VendorId }");
            var vendorData = _dao.GetVendorNameData(_mainModel.DeviceIdentData.VendorId);
            if (vendorData == null)
            {
                _logger.LogError($"Vendor with ID={ _mainModel.DeviceIdentData.VendorId } wasn't found in database.");
                LockProceeding();
                return;
            }
            _logger.LogInfo($"Vendor ID={ vendorData.Id }, Vendor name=\"{ vendorData.Name }\"");

            _logger.LogInfo($"Querying device name data for Vendor ID={ _mainModel.DeviceIdentData.VendorId }, Model ID={ _mainModel.DeviceIdentData.ModelId }");
            var nameData = _dao.GetDeviceNameData(_mainModel.DeviceIdentData.VendorId, _mainModel.DeviceIdentData.ModelId);
            if (nameData == null)
            {
                _logger.LogError($"Device model with Vendor ID={ _mainModel.DeviceIdentData.VendorId } and ModelID={ _mainModel.DeviceIdentData.ModelId } wasn't found in database.");
                LockProceeding();
                return;
            }
            _logger.LogInfo($"Vendor ID={ nameData.VendorId }, Model ID={ nameData.ModelId }, Model name=\"{ nameData.Name }\"");

            _mainModel.DeviceHumanReadableDescription = new DeviceHumanReadableDescription(vendorData.Name, nameData.Name, _mainModel.DeviceIdentData.Serial);
            VendorName = _mainModel.DeviceHumanReadableDescription.Vendor;
            ModelName = _mainModel.DeviceHumanReadableDescription.Model;
            SerialNumber = _mainModel.DeviceHumanReadableDescription.Serial;

            // Versioned data
            _mainModel.VersionSpecificDeviceData = _deviceDataGetter.GetDeviceData(_mainModel.DeviceIdentData);

            // Initializing operations provider and we are ready to go
            _deviceIndependentOperationsProvider.Setup(_mainModel.PortSettings, _mainModel.DeviceIdentData, _mainModel.VersionSpecificDeviceData);

            _isReady = true;
        }

        /// <summary>
        /// De set-ups client in a such way, like "Poll" button was never pressed
        /// </summary>
        private void DeSetup()
        {
            // Logic
            _isReady = false;
            _deviceIndependentOperationsProvider.DeSetup();
            _mainModel.VersionSpecificDeviceData = null;
            _mainModel.DeviceIdentData = null;

            // GUI
            VendorName = string.Empty;
            ModelName = string.Empty;
            SerialNumber = string.Empty;
        }

        /// <summary>
        /// Select FLASH file to upload
        /// </summary>
        public async void SelectFlashForUploadAsync()
        {
            var dialog = PrepareOpenHexDialog();
            FlashUploadFile = (await dialog.ShowAsync(Program.GetMainWindow())).FirstOrDefault();

            SetUploadButtonState();
        }

        /// <summary>
        /// Select EEPROM file to upload
        /// </summary>
        public async void SelectEepromForUploadAsync()
        {
            var dialog = PrepareOpenHexDialog();
            EepromUploadFile = (await dialog.ShowAsync(Program.GetMainWindow())).FirstOrDefault();

            SetUploadButtonState();
        }

        /// <summary>
        /// Select directory, where pre-upload backups will be placed
        /// </summary>
        public async void SelectBackupsDirectoryAsync()
        {
            var dialog = new OpenFolderDialog();
            dialog.Title = "Select backups directory";
            UploadBackupsDirectory = await dialog.ShowAsync(Program.GetMainWindow());

            SetUploadButtonState();
        }

        /// <summary>
        /// Upload to MCU
        /// </summary>
        public void Upload()
        {
            CheckReadyness();

            try
            {
                _deviceIndependentOperationsProvider.UploadToDevice(FlashUploadFile, EepromUploadFile, UploadBackupsDirectory);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error: { ex.Message }, Stack trace: { ex.StackTrace }");
            }
        }

        /// <summary>
        /// Select FLASH file for download
        /// </summary>
        public async void SelectFlashForDownloadAsync()
        {
            var dialog = PrepareSaveHexDialog();
            // If not ready, we can't generate default filename, because it contains IDs read from device
            dialog.InitialFileName = _isReady ? _deviceIndependentOperationsProvider.GenerateFlashFileName(isBackup: false) : string.Empty;
            FlashDownloadFile = await dialog.ShowAsync(Program.GetMainWindow());
        }

        /// <summary>
        /// Prepare dialog to save Intel's HEX
        /// </summary>
        private SaveFileDialog PrepareSaveHexDialog()
        {
            var dialog = new SaveFileDialog();
            // TODO: load texts from resources
            dialog.Filters.Add(new FileDialogFilter() { Name = "Intel HEX", Extensions = { "hex", "HEX" } });
            dialog.Filters.Add(new FileDialogFilter() { Name = "All files", Extensions = { "*" } });
            dialog.DefaultExtension = "hex";

            return dialog;
        }

        /// <summary>
        /// Prepare dialog to open Intel's HEX
        /// </summary>
        private OpenFileDialog PrepareOpenHexDialog()
        {
            var dialog = new OpenFileDialog();
            // TODO: load texts from resources
            dialog.Filters.Add(new FileDialogFilter() { Name = "Intel HEX", Extensions = { "hex", "HEX" } });
            dialog.Filters.Add(new FileDialogFilter() { Name = "All files", Extensions = { "*" } });
            dialog.AllowMultiple = false;

            return dialog;
        }

        /// <summary>
        /// Select EEPROM file for download
        /// </summary>
        public async void SelectEepromForDownloadAsync()
        {
            var dialog = PrepareSaveHexDialog();
            dialog.InitialFileName = _isReady ? _deviceIndependentOperationsProvider.GenerateEepromFileName(isBackup: false) : string.Empty;
            EepromDownloadFile = await dialog.ShowAsync(Program.GetMainWindow());
        }

        /// <summary>
        /// Download from MCU
        /// </summary>
        public void Download()
        {
            CheckReadyness();

            // FLASH and EEPROM must differ
            if (FlashDownloadFile.Equals(EepromDownloadFile))
            {
                var message = $"FLASH and EEPROM files to download into must differ.";

                MessageBoxManager.GetMessageBoxStandardWindow(
                    new MessageBoxStandardParams()
                    {
                        ContentTitle = "Files must differ",
                        ContentMessage = message,
                        Icon = Icon.Warning,
                        ButtonDefinitions = ButtonEnum.Ok
                    })
                    .Show();

                _logger.LogWarning(message);

                return;
            }

            try
            {
                _deviceIndependentOperationsProvider.DownloadFromDevice(FlashDownloadFile, EepromDownloadFile);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error: { ex.Message }, Stack trace: { ex.StackTrace }");
            }
        }

        /// <summary>
        /// Reboot MCU into main firmware
        /// </summary>
        public void Reboot()
        {
            CheckReadyness();
            _deviceIndependentOperationsProvider.RebootToFirmware();
            DeSetup();
        }
        #endregion

        /// <summary>
        /// Adds a new text line to console. Feed it to logger
        /// </summary>
        public void AddLineToConsole(string line)
        {
            ConsoleText += $"{ line }{ Environment.NewLine }";

            ConsoleCaretIndex = ConsoleText.Length;
        }

        /// <summary>
        /// Logging current port settings
        /// </summary>
        private void LogPortSettings()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Port settings:");

            if (_mainModel.PortSettings == null)
            {
                sb.AppendLine("Port not selected!");
            }
            else
            {
                sb.AppendLine($"Port: { _mainModel.PortSettings.Name }");
                sb.AppendLine($"Baudrate: { _mainModel.PortSettings.Baudrate }");
                sb.AppendLine($"Parity: { PortSelectionHelper.MapParityToString(_mainModel.PortSettings.Parity) }");
                sb.AppendLine($"Data bits: { _mainModel.PortSettings.DataBits }");
                sb.AppendLine($"Stop bits: { PortSelectionHelper.MapStopBitsToString(_mainModel.PortSettings.StopBits) }");
            }

            _logger.LogInfo(sb.ToString());
        }

        /// <summary>
        /// Setting state of "Poll device" button
        /// </summary>
        private void SetPollDeviceState()
        {
            IsPollDeviceEnabled = (_mainModel.PortSettings != null);
        }

        /// <summary>
        /// Enables / disables "Upload", "Download" and "Reboot" buttons
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetActionsButtonsState(bool isEnable)
        {
            SetUploadButtonState();
            SetRebootButtonState();
            SetDownloadButtonState();
        }

        /// <summary>
        /// Call it to block upload / download buttons and suggest user to select another device
        /// </summary>
        private void LockProceeding()
        {
            SetActionsButtonsState(false);
            _logger.LogError(@"Unable to proceed!
Please, select another device.");
        }

        /// <summary>
        /// Checks if we are ready to upload/download?
        /// </summary>
        private void CheckReadyness()
        {
            if (!_isReady)
            {
                var message = "Not ready to proceed!";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Checks conditions and enables/disables "Download" button
        /// </summary>
        private void SetDownloadButtonState()
        {
            IsDownloadEnabled = _isReady && !string.IsNullOrEmpty(FlashDownloadFile) && !string.IsNullOrEmpty(EepromDownloadFile);
        }

        /// <summary>
        /// As SetDownloadButtonState(), but for reboot button
        /// </summary>
        private void SetRebootButtonState()
        {
            IsRebootEnabled = _isReady;
        }

        /// <summary>
        /// As SetDownloadButtonState(), but for upload button
        /// </summary>
        private void SetUploadButtonState()
        {
            IsUploadEnabled = _isReady
                &&
                !string.IsNullOrEmpty(UploadBackupsDirectory)
                && ((IsFlashUpload && !string.IsNullOrEmpty(FlashUploadFile)) || (IsEepromUpload && !string.IsNullOrEmpty(EepromUploadFile)));
        }

        /// <summary>
        /// Enables or disables Upload FLASH File Path field and corresponding button
        /// </summary>
        private void SetUploadFlashFilePathState()
        {
            IsFlashUploadFileEnabled = IsFlashUpload;
            IsSelectFlashForUploadButtonEnabled = IsFlashUpload;
        }

        /// <summary>
        /// Enables or disables Upload EEPROM File Path field and corresponding button
        /// </summary>
        private void SetUploadEepromFilePathState()
        {
            IsEepromUploadFileEnabled = IsEepromUpload;
            IsSelectEepromForUploadButtonEnabled = IsEepromUpload;
        }
    }
}
