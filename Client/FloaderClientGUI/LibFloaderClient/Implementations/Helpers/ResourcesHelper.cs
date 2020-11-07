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

using LibFloaderClient.Implementations.Resources;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace LibFloaderClient.Implementations.Helpers
{
    /// <summary>
    /// Useful stuff to work with resources
    /// </summary>
    public static class ResourcesHelper
    {
        /// <summary>
        /// Returns given resource from given assembly as a stream
        /// </summary>
        public static Stream GetResourceAsStream(Type asmType, string name)
        {
            if (asmType == null)
            {
                throw new ArgumentNullException(nameof(asmType));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            var assembly = asmType.GetTypeInfo().Assembly;

            if (assembly == null)
            {
                throw new ArgumentException(string.Format(Language.AssemblyNotFound, asmType), nameof(asmType));
            }

            var resourceStream = assembly.GetManifestResourceStream(name);

            if (resourceStream == null)
            {
                throw new ArgumentException(string.Format(Language.ResourceNotFound, name), nameof(name));
            }

            return resourceStream;
        }

        /// <summary>
        /// As GetResourceAsStream, but tries to read resource as UTF-8 string
        /// </summary>
        public static string GetResourceAsString(Type asmType, string name)
        {
            string result;
            using (var reader = new StreamReader(GetResourceAsStream(asmType, name), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }
    }
}
