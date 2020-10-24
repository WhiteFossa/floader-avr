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

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace FloaderClientGUI.GUISpecific
{
    /// <summary>
    /// Main window interface state is saved here when long running operations are proceeding
    /// </summary>
    public class MainWindowInterfaceState
    {
        /// <summary>
        /// True when interface is locked
        /// </summary>
        public bool IsInterfaceLocked { get; set; }

        public bool IsSelectPortEnabled { get; set; }
        public bool IsPollDeviceEnabled { get; set; }
        public bool IsRebootEnabled { get; set; }
        public bool IsFlashUploadCheckboxEnabled { get; set; }
        public bool IsFlashUploadFileEnabled { get; set; }
        public bool IsSelectFlashForUploadButtonEnabled { get; set; }
        public bool IsEepromUploadCheckboxEnabled { get; set; }
        public bool IsEepromUploadFileEnabled { get; set; }
        public bool IsSelectEepromForUploadButtonEnabled { get; set; }
        public bool IsUploadBackupsDirectoryEnabled { get; set; }
        public bool IsSelectUploadBackupsDirectoryButtonEnabled { get; set; }
        public bool IsUploadButtonEnabled { get; set; }
        public bool IsFlashDownloadFileEnabled { get; set; }
        public bool IsSelectFlashDownloadFileButtonEnabled { get; set; }
        public bool IsEepromDownloadFileEnabled { get; set; }
        public bool IsSelectEepromDownloadFileButtonEnabled { get; set; }
        public bool IsDownloadButtonEnabled { get; set; }
        public bool IsAboutButtonEnabled { get; set; }
    }
}
