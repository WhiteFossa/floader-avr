using ReactiveUI;
using TextCopy;
using System;

namespace FloaderClientGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <Summary>
        /// Console log
        /// </Summary>
        private string _consoleText;

        public string ConsoleText
        {
            get => _consoleText;
            set => this.RaiseAndSetIfChanged(ref _consoleText, value);
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
            ConsoleText += $"Port select{ Environment.NewLine }";
        }

        /// <summary>
        /// Command to poll device
        /// </summary>
        public void PollDevice()
        {
            ConsoleText += $"Device poll{ Environment.NewLine }";
        }

        /// <summary>
        /// Select FLASH file to upload
        /// </summary>
        public void SelectFlashForUpload()
        {
            ConsoleText += $"Select FLASH file for upload{ Environment.NewLine }";
        }

        /// <summary>
        /// Select EEPROM file to upload
        /// </summary>
        public void SelectEepromForUpload()
        {
            ConsoleText += $"Select EEPROM file for upload{ Environment.NewLine }";
        }

        /// <summary>
        /// Select directory, where pre-upload backups will be placed
        /// </summary>
        public void SelectBackupsDirectory()
        {
            ConsoleText += $"Select backups directory{ Environment.NewLine }";
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
            ConsoleText += $"Select FLASH for download{ Environment.NewLine }";
        }

        /// <summary>
        /// Select EEPROM file for download
        /// </summary>
        public void SelectEepromForDownload()
        {
            ConsoleText += $"Select EEPROM for download{ Environment.NewLine }";
        }

        /// <summary>
        /// Download from MCU
        /// </summary>
        public void Download()
        {
            ConsoleText += $"Download{ Environment.NewLine }";
        }
    }
}
