namespace Tools.Belt.Common.Abstractions.Configuration.Entities
{
    public interface ISystemConfigurationHolder
    {
        /// <summary>
        ///     Wrapper for all the configuration variables common throughout all the Tools.Belt derived project codebase.
        /// </summary>
        ISystemConfiguration SystemConfiguration { get; }
    }
}