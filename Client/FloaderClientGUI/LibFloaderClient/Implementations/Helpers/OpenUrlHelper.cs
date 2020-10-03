using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace LibFloaderClient.Implementations.Helpers
{
    /// <summary>
    /// Class to open URLs in applications.
    /// </summary>
    public static class OpenUrlHelper
    {
        /// <summary>
        /// Opens given URL in suiteable application (browser, email client and so on).
        /// </summary>
        public static void Open(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (var process = Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    Arguments = string.Empty,
                    CreateNoWindow = true,
                    UseShellExecute = true
                }))
                { };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using (var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "xdg-open",
                    Arguments = url,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }))
                { };
            }
            else
            {
                throw new InvalidOperationException("Unsupported OS");
            }
        }
    }
}
