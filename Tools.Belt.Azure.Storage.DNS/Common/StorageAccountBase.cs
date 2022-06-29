using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Storage.Auth;
using Polly;
using Tools.Belt.Azure.DNS.Abstractions.Configuration.AzureActiveDirectory;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Azure.Storage.DNS.Common
{
    public abstract class StorageAccountBase : IStorageService
    {
        /// <summary>
        ///     Authenticates the currently running user against Azure AD and returns a valid Token as a
        ///     <see cref="StorageCredentials" /> instance.
        ///     It does so by calling <see cref="StorageCredentialsFactoryAsync" /> with no <see cref="CancellationToken" />
        /// </summary>
        /// <param name="storageUri">The uri for the storage accountWrapper.</param>
        /// <param name="configuration">The configuration service instance.</param>
        /// <returns>A valid Token as a <see cref="StorageCredentials" /> instance.</returns>
        protected static TokenCredential TokenCredentialsFactory(Uri storageUri,
            IAzureActiveDirectoryClientCredentialsConfiguration configuration)
        {
            return TokenCredentialsFactory(storageUri, configuration, CancellationToken.None);
        }

        /// <summary>
        ///     Authenticates the currently running user against Azure AD and returns a valid Token as a
        ///     <see cref="StorageCredentials" /> instance.
        /// </summary>
        /// <param name="storageUri">The uri for the storage accountWrapper.</param>
        /// <param name="configuration">The configuration service instance.</param>
        /// <param name="timeSpanToTimeOut">The <see cref="TimeSpan" /> to wait before aborting the operation.</param>
        /// <returns>A valid Token as a <see cref="StorageCredentials" /> instance.</returns>
        protected static TokenCredential TokenCredentialsFactory(Uri storageUri,
            IAzureActiveDirectoryClientCredentialsConfiguration configuration, TimeSpan timeSpanToTimeOut)
        {
            return StorageCredentialsFactoryAsync(storageUri, configuration)
                .ReturnAsyncCallSynchronously(timeSpanToTimeOut);
        }

        /// <summary>
        ///     Authenticates the currently running user against Azure AD and returns a valid Token as a
        ///     <see cref="StorageCredentials" /> instance.
        /// </summary>
        /// <param name="storageUri">The uri for the storage accountWrapper.</param>
        /// <param name="configuration">The configuration service instance.</param>
        /// <param name="millisecondsToTimeOut">The number of milliseconds to wait before aborting the operation.</param>
        /// <returns>A valid Token as a <see cref="StorageCredentials" /> instance.</returns>
        protected static TokenCredential TokenCredentialsFactory(Uri storageUri,
            IAzureActiveDirectoryClientCredentialsConfiguration configuration, int millisecondsToTimeOut)
        {
            return StorageCredentialsFactoryAsync(storageUri, configuration)
                .ReturnAsyncCallSynchronously(millisecondsToTimeOut);
        }

        /// <summary>
        ///     Authenticates the currently running user against Azure AD and returns a valid Token as a
        ///     <see cref="StorageCredentials" /> instance.
        /// </summary>
        /// <param name="storageUri">The uri for the storage accountWrapper.</param>
        /// <param name="configuration">The configuration service instance.</param>
        /// <param name="millisecondsToTimeOut">The number of milliseconds to wait before aborting the operation.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" />.</param>
        /// <returns>A valid Token as a <see cref="StorageCredentials" /> instance.</returns>
        protected static TokenCredential TokenCredentialsFactory(Uri storageUri,
            IAzureActiveDirectoryClientCredentialsConfiguration configuration, int millisecondsToTimeOut,
            CancellationToken cancellationToken)
        {
            return StorageCredentialsFactoryAsync(storageUri, configuration)
                .ReturnAsyncCallSynchronously(millisecondsToTimeOut, cancellationToken);
        }

        /// <summary>
        ///     Authenticates the currently running user against Azure AD and returns a valid Token as a
        ///     <see cref="StorageCredentials" /> instance.
        /// </summary>
        /// <param name="storageUri">The uri for the storage accountWrapper.</param>
        /// <param name="configuration">The configuration service instance.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" />.</param>
        /// <returns>A valid Token as a <see cref="StorageCredentials" /> instance.</returns>
        protected static TokenCredential TokenCredentialsFactory(Uri storageUri,
            IAzureActiveDirectoryClientCredentialsConfiguration configuration, CancellationToken cancellationToken)
        {
            return StorageCredentialsFactoryAsync(storageUri, configuration)
                .ReturnAsyncCallSynchronously(cancellationToken);
        }

        /// <summary>
        ///     Authenticates the currently running user against Azure AD and returns a valid Token as a
        ///     <see cref="StorageCredentials" /> instance.
        /// </summary>
        /// <param name="storageUri">The uri for the storage accountWrapper.</param>
        /// <param name="configuration">The configuration service instance.</param>
        /// <returns>A valid Token as a <see cref="StorageCredentials" /> instance.</returns>
        protected static async Task<TokenCredential> StorageCredentialsFactoryAsync(Uri storageUri,
            IAzureActiveDirectoryClientCredentialsConfiguration configuration)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            return (await Policy.Handle<Exception>()
                .WaitAndRetryAsync(
                    configuration.SystemConfiguration.PollyRetryLimit,
                    retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(100, retryAttempt))
                ).ExecuteAndCaptureAsync(async () =>
                    {
                        try
                        {
                            string token =
                                await azureServiceTokenProvider.GetAccessTokenAsync(storageUri.AbsoluteUri,
                                    configuration.TenantId);

                            // Use the same token provider to request a new token.
                            TokenCredential authResult = new TokenCredential(token);

                            return authResult;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            throw;
                        }
                    }
                )).Result;
        }
    }
}