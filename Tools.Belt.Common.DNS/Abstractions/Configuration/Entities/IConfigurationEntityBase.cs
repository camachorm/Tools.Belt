namespace Tools.Belt.Common.Abstractions.Configuration.Entities
{
    public interface IConfigurationEntityBase
    {
        /// <summary>
        ///     The <see cref="IConfigurationService" /> instance used to access configuration variables.
        /// </summary>
        IConfigurationService ConfigurationService { get; }
    }
}