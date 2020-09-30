using ReactiveUI;

namespace FloaderClientGUI.ViewModels
{
    public class AboutWindowViewModel : ViewModelBase
    {
        #region Bound properties

        private string _versionText;

        public string VersionText
        {
            get => _versionText;
            set => this.RaiseAndSetIfChanged(ref _versionText, value);
        }

        #endregion

        public AboutWindowViewModel() : base()
        {
            VersionText = Program.GetFullAppName();
        }
    }
}
