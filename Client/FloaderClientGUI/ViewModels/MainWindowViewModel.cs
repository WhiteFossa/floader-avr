using ReactiveUI;
using TextCopy;

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

        public MainWindowViewModel() : base()
        {
            ConsoleText = "Yerf!";
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
    }
}
