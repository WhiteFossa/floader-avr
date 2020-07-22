namespace LibFloaderClient.Interfaces.Versioned.Common
{
    /// <summary>
    /// Interface to check if given version supported by our client or not
    /// </summary>
    public interface IVersionValidator
    {
        /// <summary>
        /// Returns true if version supported
        /// </summary>
        bool Validate(int version);
    }
}