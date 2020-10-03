using LibFloaderClient.Implementations.Helpers;
using ReactiveUI;

namespace FloaderClientGUI.ViewModels
{
    public class AboutWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Sources can be obtained here
        /// </summary>
        private const string SourcesAddress = "https://github.com/WhiteFossa/floader-avr";

        /// <summary>
        /// Contact email
        /// </summary>
        private const string ContactEmail = "whitefossa@protonmail.com";

        #region Bound properties

        private string _versionText;
        private string _sourcesAddressText;
        private string _contactEmailText;

        public string VersionText
        {
            get => _versionText;
            set => this.RaiseAndSetIfChanged(ref _versionText, value);
        }

        public string SourcesAddressText
        {
            get => _sourcesAddressText;
            set => this.RaiseAndSetIfChanged(ref _sourcesAddressText, value);
        }

        public string ContactEmailText
        {
            get => _contactEmailText;
            set => this.RaiseAndSetIfChanged(ref _contactEmailText, value);
        }

        #endregion

        #region Commands

        public void OpenSourcesURL()
        {
            OpenUrlHelper.Open(SourcesAddress);
        }

        public void ComposeEmail()
        {
            OpenUrlHelper.Open($"mailto:{ ContactEmail }");
        }

        #endregion

        public AboutWindowViewModel() : base()
        {
            VersionText = Program.GetFullAppName();
            SourcesAddressText = SourcesAddress;
            ContactEmailText = ContactEmail;
        }
    }
}
