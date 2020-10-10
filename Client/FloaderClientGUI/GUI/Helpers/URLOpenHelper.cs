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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FloaderClientGUI.Helpers
{
    /// <summary>
    /// Class to open URLs in applications.
    /// </summary>
    public static class URLOpenHelper
    {
        /// <summary>
        /// Opens given URL in suiteable application (browser, email client and so on).
        /// </summary>
        public static void Open(string url)
        {
            ProcessStartInfo startInfo;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startInfo = new ProcessStartInfo
                {
                    FileName = url,
                    Arguments = string.Empty,
                    CreateNoWindow = true,
                    UseShellExecute = true
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                startInfo = new ProcessStartInfo
                {
                    FileName = "xdg-open",
                    Arguments = url,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
            }
            else
            {
                throw new InvalidOperationException("Unsupported OS");
            }

            using (var process = Process.Start(startInfo))
            {
            };
        }
    }
}
