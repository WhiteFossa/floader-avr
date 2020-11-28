using System;
using FloaderClientGUI.Resources;
using LibFloaderClient.Interfaces.Logger;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace FloaderClientGUI.Helpers
{
    /// <summary>
    /// Helper for exceptions handling
    /// </summary>
    public static class ExceptionsHelper
    {
        /// <summary>
        /// Logs exception and shows message to user.
        /// </summary>
        public static void ProcessUnexpectedException(Exception ex, ILogger logger)
        {
            var message = string.Format(Language.UnexpectedExceptionMessage, ex.GetType(), ex.Message);

            logger.LogError(message);

            MessageBoxManager.GetMessageBoxStandardWindow(
                    new MessageBoxStandardParams()
                    {
                        ContentTitle = Language.UnexpectedExceptionTitle,
                        ContentMessage = message,
                        Icon = Icon.Error,
                        ButtonDefinitions = ButtonEnum.Ok
                    })
                .Show();
        }
    }
}