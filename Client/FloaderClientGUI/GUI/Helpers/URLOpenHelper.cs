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
