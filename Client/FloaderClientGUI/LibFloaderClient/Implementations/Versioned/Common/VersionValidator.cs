using LibFloaderClient.Interfaces.Versioned.Common;
using System.Collections.Generic;

namespace LibFloaderClient.Implementations.Versioned.Common
{
    public class VersionValidator : IVersionValidator
    {
        private readonly List<int> SupportedVersions = new List<int>() { 1 };

        public bool Validate(int version)
        {
            return SupportedVersions.Contains(version);
        }
    }
}