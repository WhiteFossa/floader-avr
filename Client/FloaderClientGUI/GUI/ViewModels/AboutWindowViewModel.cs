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

using FloaderClientGUI.Helpers;
using FloaderClientGUI.Resources;
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
        private string _license;

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

        public string License
        {
            get => _license;
            set => this.RaiseAndSetIfChanged(ref _license, value);
        }

        #endregion

        #region Localization

        public string LocTextSoftwareDescription
        {
            get => Language.LocTextSoftwareDescription;
        }

        public string LocTextAuthors1
        {
            get => Language.LocTextAuthors1;
        }

        public string LocTextAuthors2
        {
            get => Language.LocTextAuthors2;
        }

        public string LocTextLicense
        {
            get => Language.LocTextLicense;
        }

        public string LocTextAgpl
        {
            get => Language.LocTextAgpl;
        }

        public string LocTextOrLaterVersion
        {
            get => Language.LocTextOrLaterVersion;
        }

        public string LocTextSources
        {
            get => Language.LocTextSources;
        }

        public string LocTextEmail
        {
            get => Language.LocTextEmail;
        }

        public string LocTextLicenseText
        {
            get => Language.LocTextLicenseText;
        }

        #endregion

        #region Commands

        public void OpenSourcesURL()
        {
            URLOpenHelper.Open(SourcesAddress);
        }

        public void ComposeEmail()
        {
            URLOpenHelper.Open($"mailto:{ ContactEmail }");
        }

        #endregion

        public AboutWindowViewModel() : base()
        {
            VersionText = Program.GetFullAppName();
            SourcesAddressText = SourcesAddress;
            ContactEmailText = ContactEmail;

            // Loading license
            License = ResourcesHelper.GetResourceAsString(typeof(FloaderClientGUI.ViewModels.AboutWindowViewModel),
                "FloaderClientGUI.Resources.agpl3.txt");
        }
    }
}
