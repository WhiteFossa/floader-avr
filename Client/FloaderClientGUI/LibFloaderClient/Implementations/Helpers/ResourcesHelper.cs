using System;
using System.Collections.Generic;
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
                throw new ArgumentException($"Assembly { asmType } not found.", nameof(asmType));
            }

            var resourceStream = assembly.GetManifestResourceStream(name);

            if (resourceStream == null)
            {
                throw new ArgumentException($"Resource { name } not found.", nameof(name));
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
