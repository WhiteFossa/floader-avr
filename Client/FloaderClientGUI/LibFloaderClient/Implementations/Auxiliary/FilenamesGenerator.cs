﻿using LibFloaderClient.Interfaces.Auxiliary;
using LibFloaderClient.Models.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibFloaderClient.Implementations.Auxiliary
{
    public class FilenamesGenerator : IFilenamesGenerator
    {
        /// <summary>
        /// Template for filenames common part.
        /// 0 - Vendor ID
        /// 1 - Model ID
        /// 2 - Serial
        /// 3 - Year
        /// 4 - Month
        /// 5 - Day
        /// 6 - Hour
        /// 7 - Minute
        /// 8 - Second
        /// </summary>
        private const string CommonPartTemplate = "VID{0}_MID{1}_SER{2}_Y{3:0000}M{4:00}D{5:00}H{6:00}M{7:00}S{8:00}.hex";

        /// <summary>
        /// Prefix for FLASH files
        /// </summary>
        private const string FlashPrefix = "FLASH_";

        /// <summary>
        /// Prefix for EEPROM files
        /// </summary>
        private const string EepromPrefix = "EEPROM_";

        /// <summary>
        /// Prefix for backups of all sorts
        /// </summary>
        private const string BackupPrefix = "Backup_";

        public string GenerateEEPROMFilename(DeviceIdentifierData identData, bool isBackup)
        {
            var sb = new StringBuilder();

            sb.Append(GenerateBackupPrefix(isBackup));
            sb.Append(EepromPrefix);
            sb.Append(GenerateCommonPart(identData));

            return sb.ToString();
        }

        public string GenerateFLASHFilename(DeviceIdentifierData identData, bool isBackup)
        {
            var sb = new StringBuilder();

            sb.Append(GenerateBackupPrefix(isBackup));
            sb.Append(FlashPrefix);
            sb.Append(GenerateCommonPart(identData));

            return sb.ToString();
        }

        private string GenerateCommonPart(DeviceIdentifierData identData)
        {
            var currentTime = DateTime.Now;

            return string.Format(CommonPartTemplate,
                identData.VendorId,
                identData.ModelId,
                identData.Serial,
                currentTime.Year,
                currentTime.Month,
                currentTime.Day,
                currentTime.Hour,
                currentTime.Minute,
                currentTime.Second);
        }

        private string GenerateBackupPrefix(bool isBackup)
        {
            return isBackup ? BackupPrefix : string.Empty;
        }
    }
}
