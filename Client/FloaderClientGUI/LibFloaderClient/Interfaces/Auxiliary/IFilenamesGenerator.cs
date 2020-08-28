using LibFloaderClient.Models.Device;

namespace LibFloaderClient.Interfaces.Auxiliary
{
    /// <summary>
    /// Interface to generate FLASH/EEPROM filenames for download/backups
    /// </summary>
    public interface IFilenamesGenerator
    {
        /// <summary>
        /// Generates filename for FLASH file
        /// </summary>
        string GenerateFLASHFilename(DeviceIdentifierData identData, bool isBackup);

        /// <summary>
        /// As GenerateFlashName(), but for EEPROM
        /// </summary>
        string GenerateEEPROMFilename(DeviceIdentifierData identData, bool isBackup);
    }
}
