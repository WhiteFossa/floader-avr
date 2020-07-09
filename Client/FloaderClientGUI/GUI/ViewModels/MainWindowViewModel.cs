using System.Net.Mime;
using ReactiveUI;
using TextCopy;
using System;
using LibFloaderClient.Interfaces.Logger;
using FloaderClientGUI;
using Microsoft.Extensions.DependencyInjection;
using LibFloaderClient.Interfaces.SerialPortsLister;
using FloaderClientGUI.Views;

namespace FloaderClientGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ILogger _logger;

        private string _consoleText;
        private string _portName;
        private string _vendorName;
        private string _modelName;
        private string _serialNumber;
        private bool _isFlashUpload;
        private bool _isEepromUpload;
        private bool _isBackupBeforeUpload;
        private string _flashUploadFile;
        private string _eepromUploadFile;
        private string _uploadBackupsDirectory;
        private string _flashDownloadFile;
        private string _eepromDownloadFile;

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
            set => this.RaiseAndSetIfChanged(ref _isFlashUpload, value);
        }

        /// <summary>
        /// Do we need to upload EEPROM?
        /// </summary>
        public bool IsEepromUpload
        {
            get => _isEepromUpload;
            set => this.RaiseAndSetIfChanged(ref _isEepromUpload, value);
        }

        /// <summary>
        /// Do we need to make backups before upload?
        /// </summary>
        /// <value></value>
        public bool IsBackupBeforeUpload
        {
            get => _isBackupBeforeUpload;
            set => this.RaiseAndSetIfChanged(ref _isBackupBeforeUpload, value);
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
            set => this.RaiseAndSetIfChanged(ref _flashDownloadFile, value);
        }

        /// <summary>
        /// Put downloaded EEPROM here
        /// </summary>
        public string EepromDownloadFile
        {
            get => _eepromDownloadFile;
            set => this.RaiseAndSetIfChanged(ref _eepromDownloadFile, value);
        }



        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel() : base()
        {
            // Setting up logger
            _logger = Program.Di.GetService<ILogger>();
            _logger.SetLoggingFunction(AddLineToConsole);

            IsBackupBeforeUpload = true;
        }

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
        public void SelectPort()
        {
            new PortSelectionWindow().ShowDialog(Program.GetMainWindow());

            // // TODO: Remove it from here
            // _logger.LogInfo("Ports:");

            // var lister = Program.Di.GetService<ISerialPortsLister>();
            // var listedPorts = lister.ListOrdered();

            // foreach (var port in listedPorts)
            // {
            //     _logger.LogInfo(port);
            // }

            // PortName = "Megaport";
        }

        /// <summary>
        /// Command to poll device
        /// </summary>
        public void PollDevice()
        {
            VendorName = "TestVendor";
            ModelName = "Megadevice";
            SerialNumber = "000001";

            IsFlashUpload = true;
            IsEepromUpload = true;
            IsBackupBeforeUpload = false;
        }

        /// <summary>
        /// Select FLASH file to upload
        /// </summary>
        public void SelectFlashForUpload()
        {
            FlashUploadFile = "FLASH HEX file";
        }

        /// <summary>
        /// Select EEPROM file to upload
        /// </summary>
        public void SelectEepromForUpload()
        {
            EepromUploadFile = "EEPROM HEX file";
        }

        /// <summary>
        /// Select directory, where pre-upload backups will be placed
        /// </summary>
        public void SelectBackupsDirectory()
        {
            UploadBackupsDirectory = "Backups directory";
        }

        /// <summary>
        /// Upload to MCU
        /// </summary>
        public void Upload()
        {
            ConsoleText += $"Upload{ Environment.NewLine }";
        }

        /// <summary>
        /// Select FLASH file for download
        /// </summary>
        public void SelectFlashForDownload()
        {
            FlashDownloadFile = "FLASH HEX file for download";
        }

        /// <summary>
        /// Select EEPROM file for download
        /// </summary>
        public void SelectEepromForDownload()
        {
            EepromDownloadFile = "EEPROM HEX file for download";
        }

        /// <summary>
        /// Download from MCU
        /// </summary>
        public void Download()
        {
            ConsoleText += $"Download{ Environment.NewLine }";
        }

        /// <summary>
        /// Adds a new text line to console.static Feed it to logger
        /// </summary>
        public void AddLineToConsole(string line)
        {
            ConsoleText += $"{ line }{ Environment.NewLine }";
        }
    }
}
