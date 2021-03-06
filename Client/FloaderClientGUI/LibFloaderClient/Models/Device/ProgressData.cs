﻿/*
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

namespace LibFloaderClient.Models.Device
{
    /// <summary>
    /// Information about progress
    /// </summary>
    public class ProgressData
    {
        /// <summary>
        /// Current value, for example 27 for 27th page
        /// </summary>
        public double Current { get; private set; }

        /// <summary>
        /// Maximum value, for example 128 for total of 128 pages
        /// </summary>
        public double Max { get; private set; }

        /// <summary>
        /// Operation description, for example "Writing EEPROM"
        /// </summary>
        public string Operation { get; private set; }

        public ProgressData(double current, double max, string operation)
        {
            Current = current;
            Max = max;

            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
        }
    }
}
