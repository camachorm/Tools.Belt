// ReSharper disable PossibleNullReferenceException

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Polly;
using Tools.Belt.Azure.DNS.Extensions;
using Tools.Belt.Azure.Storage.DNS.Common;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Azure.Storage.DNS.Blob
{
    [ExcludeFromCodeCoverage]
    // ReSharper disable UnusedMember.Global
    public class BlobStorageService : StorageAccountBase, IBlobStorageService

        // ReSharper restore UnusedMember.Global
    {
        private readonly ILogger _logger;

        public BlobStorageService(string connString, IBlobStorageConfiguration configuration, ILogger logger) : this(
            CloudBlobClientFactory(connString, logger), configuration, logger)
        {
            ConnString = connString;
            Configuration = configuration;
            AccountName = connString.ParseStorageAccountNameFromConnectionString();
            logger.LogTrace("BlobStorageService(ConnString: {connString})",
                connString.ParseStorageAccountNameFromConnectionString());
        }

        public BlobStorageService(string clientId, string tenantId, string clientSecret, Uri storageUri,
            IBlobStorageConfiguration configuration, ILogger logger)
            : this(CloudBlobClientFactory(clientId, tenantId, clientSecret, storageUri, logger), configuration, logger)
        { 
            Configuration = configuration;
            AccountName = storageUri.AbsoluteUri;
            logger.LogTrace("BlobStorageService(ClientId: {clientId}, StorageUri: {storageUri})", clientId, storageUri);
        }

        public BlobStorageService(CloudBlobClientWrapper storageClient, IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            ClientWrapper = storageClient;
            Configuration = configuration;
            _logger = logger;
            logger.LogTrace($"CloudBlobClientFactory() ({ClientWrapper != null})");
        }

        public BlobStorageService(Uri storageUri, IBlobStorageConfiguration configuration, ILogger logger)
            : this(CloudBlobClientFactory(storageUri, configuration, logger), configuration, logger)
        {
            AccountName = storageUri.AbsoluteUri;
            logger.LogTrace("BlobStorageService(Uri: {storageUri})", AccountName);
        }

        public IBlobStorageConfiguration Configuration { get; }

        public string ConnString { get; }

        public string AccountName { get; }

        public ICloudBlobClientWrapper ClientWrapper { get; }

        /// <summary>
        ///     Async method that deletes a file if it exists
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be deleted</param>
        /// <returns>A task to await.</returns>
        public async Task<bool> DeleteIfExistsAsync(string containerPath, string fileName)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, fileName);
            return await blobReference.DeleteIfExistsAsync();
        }

        public async Task CreateOrAppendFileAsBlockBlobAsync(string containerPath, string fileName, byte[] fileContent)
        {
            var reference = GetContainerReference(containerPath).GetAppendBlobReference(fileName);
            var count = 0;
            while  (!await reference.ExistsAsync())
            {
                await reference.CreateOrReplaceAsync();
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                if (++count == 10)
                {
                    throw new Exception("Creation of file failed");
                }
            }

            await reference.AppendBlockAsync(new MemoryStream(fileContent));
        }

        /// <summary>
        ///     Async method that copies a file
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be copied</param>
        /// <param name="destinationContainerPath">The container where the new file should be copied to.</param>
        /// <param name="destinationFileName">The name of the new file.</param>
        /// <returns>A task to await.</returns>
        public async Task CopyBlobAsync(string containerPath, string fileName, string destinationContainerPath,
            string destinationFileName)
        {
            CloudBlockBlob sourceBlob = GetBlockBlobReference(containerPath, fileName);

            bool sourceBlobAlreadyLeased = false;

            if (sourceBlob != null)
            {
                await sourceBlob.FetchAttributesAsync();

                sourceBlobAlreadyLeased = sourceBlob.Properties.LeaseState == LeaseState.Leased;
            }

            try
            {
                if (!sourceBlobAlreadyLeased)
                    await sourceBlob.AcquireLeaseAsync(null);

                CloudBlockBlob destBlob = GetBlockBlobReference(destinationContainerPath, destinationFileName);

                // Ensure that the source blob exists.
                if (await sourceBlob.ExistsAsync()) await destBlob.StartCopyAsync(sourceBlob);
            }
            catch (Exception e)
            {
                _logger.LogError($"BlobStorageService copy exception: {e.Message}");
                throw;
            }
            finally
            {
                // Break the lease on the source blob.
                if (sourceBlob != null && !sourceBlobAlreadyLeased)
                {
                    await sourceBlob.FetchAttributesAsync();

                    if (sourceBlob.Properties.LeaseState != LeaseState.Available)
                        await sourceBlob.BreakLeaseAsync(new TimeSpan(0));
                }
            }
        }

        #region Static Factories

        public static CloudBlobClientWrapper CloudBlobClientFactory(string connString, ILogger logger)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);
            logger.LogTrace("CloudStorageAccountWrapper.Parse ({storageAccount}\\{loaded})", connString.ParseStorageAccountNameFromConnectionString(), storageAccount != null);
            return new CloudBlobClientWrapper(storageAccount, logger);
        }

        public static CloudBlobClientWrapper CloudBlobClientFactory(
            string clientId,
            string tenantId,
            string clientSecret,
            Uri storageUri,
            ILogger logger)
        {
            IConfidentialClientApplication application = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
                .WithClientId(clientId)
                .WithClientSecret(clientSecret)
                // ReSharper disable LocalizableElement
                .WithLogging((level, message, pii) => logger.LogTrace($"{level}: {message}"))
                // ReSharper restore LocalizableElement
                .Build();

            string[] scopes = {"https://storage.azure.com/.default"};

            AuthenticationResult ar = application.AcquireTokenForClient(scopes).ExecuteAsync()
                .ReturnAsyncCallSynchronously();

            TokenCredential tokenCredential = new TokenCredential(ar.AccessToken);
            StorageCredentials storageCredentials = new StorageCredentials(tokenCredential);

            return new CloudBlobClientWrapper(storageUri, storageCredentials, logger);
        }

        public static CloudBlobClientWrapper CloudBlobClientFactory(Uri storageUri,
            IBlobStorageConfiguration configuration, ILogger logger)
        {
            return CloudBlobClientFactoryAsync(storageUri, configuration, logger).ReturnAsyncCallSynchronously();
        }


        public static async Task<CloudBlobClientWrapper> CloudBlobClientFactoryAsync(Uri storageUri,
            IBlobStorageConfiguration configuration, ILogger logger)
        {
            return new CloudBlobClientWrapper(storageUri,
                new StorageCredentials(await StorageCredentialsFactoryAsync(storageUri, configuration)), logger);
        }

        #endregion Static Factories

        #region Private Methods

        private CloudBlobContainer GetContainerReference(string containerPath)
        {
            _logger.LogTrace($"GetContainerReference ({containerPath})");
            CloudBlobContainer container = ClientWrapper.GetContainerReference(containerPath);
            _logger.LogTrace($"CloudStorageAccountWrapper.Parse ({container != null})");
            return container;
        }

        private CloudBlockBlob GetBlockBlobReference(string containerPath, string fileName)
        {
            return GetContainerReference(containerPath).GetBlockBlobReference(fileName);
        }

        #endregion

        #region Read methods

        /// <summary>
        ///     Synchronous method that reads a file from a blob into a string object
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be read </param>
        /// <returns>The contents of the specified file in a string object.</returns>
        public string ReadFileAsString(string containerPath, string fileName)
        {
            return ReadFileAsStringAsync(containerPath, fileName).ReturnAsyncCallSynchronously();
        }

        /// <summary>
        ///     Async method that reads a file from a blob into a string object
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be read </param>
        /// <returns>The contents of the specified file in a string object.</returns>
        /// <remarks>
        /// This function uses "DownloadTextAsync", which can return unexpected characters.
        /// Usually the returned text start with a BOM character.
        /// More info: https://github.com/Azure/azure-storage-net/issues/571
        /// Fix: Use ReadFileAsStreamAsync instead.
        /// </remarks>
        public async Task<string> ReadFileAsStringAsync(string containerPath, string fileName)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, fileName);
            return await blobReference.DownloadTextAsync();
        }

        /// <summary>
        ///     Async method that reads a file from a blob into a stream object
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be read </param>
        /// <returns>The contents of the specified file in a stream object.</returns>
        public async Task<Stream> ReadFileAsStreamAsync(string containerPath, string fileName)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, fileName);
            Stream stream = new MemoryStream();
            await blobReference.DownloadToStreamAsync(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task<bool> FileExistsAsync(string containerPath, string fileName)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, fileName);
            return await blobReference.ExistsAsync();
        }

        public Stream OpenRead(string containerPath, string fileName)
        {
            return OpenReadAsync(containerPath, fileName).ReturnAsyncCallSynchronously();
        }

        public async Task<Stream> OpenReadAsync(string containerPath, string fileName)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, fileName);
            
            return await Policy.Handle<Exception>()
                .WaitAndRetryAsync(
                    Configuration.SystemConfiguration.PollyRetryLimit,
                    retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(10, retryAttempt)),
                    async (exception, span) =>
                    {
                        if (exception is Microsoft.Azure.Storage.StorageException && 
                            exception.Message.Contains("Server failed to authenticate the request. Make sure the value of Authorization header is formed correctly including the signature."))
                        {
                            ClientWrapper.ResetCredentials(new StorageCredentials(await StorageCredentialsFactoryAsync(ClientWrapper.BaseUri, Configuration)));
                        }
                    }
                ).ExecuteAsync(async () => await blobReference.OpenReadAsync());
        }

        public Stream OpenWrite(string containerPath, string fileName)
        {
            return OpenWriteAsync(containerPath, fileName).ReturnAsyncCallSynchronously();
        }

        public async Task<Stream> OpenWriteAsync(string containerPath, string fileName)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, fileName);
            return await blobReference.OpenWriteAsync();
        }

        /// <summary>
        ///     Synchronous method that reads a file from a blob into a byte array
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be read </param>
        /// <returns>The contents of the specified file in a byte array.</returns>
        public byte[] ReadFileAsByteArray(string containerPath, string fileName)
        {
            return ReadFileAsByteArrayAsync(containerPath, fileName).ReturnAsyncCallSynchronously();
        }

        /// <summary>
        ///     Async method that reads a file from a blob into a byte array
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be read </param>
        /// <returns>The contents of the specified file in a byte array.</returns>
        public async Task<byte[]> ReadFileAsByteArrayAsync(string containerPath, string fileName)
        {
            string result = await ReadFileAsStringAsync(containerPath, fileName);
            return Encoding.UTF8.GetBytes(result);
        }

        /// <summary>
        ///     Synchronous method that reads a file from a blob into a local file
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="sourceFileName">The name of the file to be read </param>
        /// <param name="destinationFileFullPath">The full path and name of the file to be written</param>
        /// <param name="fileMode">
        ///     The file mode to be used when creating the file handle. Defaults to Create which recreates the
        ///     file when executed even if it already exists.
        /// </param>
        /// <returns>The contents of the specified file in a byte array.</returns>
        public void ReadFileIntoFile(
            string containerPath,
            string sourceFileName,
            string destinationFileFullPath,
            FileMode fileMode = FileMode.Create)
        {
            ReadFileIntoFileAsync(containerPath, sourceFileName, destinationFileFullPath, fileMode).Wait();
        }

        /// <summary>
        ///     Async method that reads a file from a blob into a local file
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="sourceFileName">The name of the file to be read </param>
        /// <param name="destinationFileFullPath">The full path and name of the file to be written</param>
        /// <param name="fileMode">
        ///     The file mode to be used when creating the file handle. Defaults to Create which recreates the
        ///     file when executed even if it already exists.
        /// </param>
        /// <returns>The contents of the specified file in a byte array.</returns>
        public async Task ReadFileIntoFileAsync(
            string containerPath,
            string sourceFileName,
            string destinationFileFullPath,
            FileMode fileMode = FileMode.Create)
        {
            await GetBlockBlobReference(containerPath, sourceFileName)
                .DownloadToFileAsync(destinationFileFullPath, fileMode);
        }

        #endregion

        #region Write Methods

        /// <summary>
        ///     Synchronous method that writes a stream into a Block Blob
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be written.</param>
        /// <param name="fileContent">The content to write into the file</param>
        public void WriteFileAsBlockBlob(string containerPath, string fileName, byte[] fileContent)
        {
            WriteFileAsBlockBlobAsync(containerPath, fileName, fileContent).Wait();
        }

        /// <summary>
        ///     Async method that writes a stream into a Block Blob
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be written.</param>
        /// <param name="fileContent">The content to write into the file</param>
        public async Task WriteFileAsBlockBlobAsync(string containerPath, string fileName, byte[] fileContent)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, fileName);
            await blobReference.UploadFromByteArrayAsync(fileContent, 0, fileContent.Length);
        }

        /// <summary>
        ///     Synchronous method that writes a stream into a Block Blob
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be written.</param>
        /// <param name="fileContent">The content to write into the file</param>
        public void WriteFileAsBlockBlob(string containerPath, string fileName, Stream fileContent)
        {
            WriteFileAsBlockBlobAsync(containerPath, fileName, fileContent).Wait();
        }

        /// <summary>
        ///     Async method that writes a stream into a Block Blob
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be written.</param>
        /// <param name="fileContent">The content to write into the file</param>
        public async Task WriteFileAsBlockBlobAsync(string containerPath, string fileName, Stream fileContent)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, fileName);
            await blobReference.UploadFromStreamAsync(fileContent);
        }

        /// <summary>
        ///     Async method that writes a stream into a Block Blob
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be written.</param>
        /// <param name="fileContent">The content to write into the file.</param>
        /// <param name="leaseId">The Id of the lease of the blob.</param>
        public async Task WriteFileAsBlockBlobAsync(string containerPath, string fileName, Stream fileContent,
            string leaseId)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, fileName);
            await blobReference.UploadFromStreamAsync(fileContent, AccessCondition.GenerateLeaseCondition(leaseId),
                new BlobRequestOptions(), new OperationContext());
        }

        /// <summary>
        ///     Synchronous method that writes a string into a Block Blob. If it exists, it will be overwritten.
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be written.</param>
        /// <param name="fileContent">The content to write into the file</param>
        public void WriteFileAsBlockBlob(string containerPath, string fileName, string fileContent)
        {
            WriteFileAsBlockBlobAsync(containerPath, fileName, fileContent).Wait();
        }

        /// <summary>
        ///     Async method that writes a string into a Block Blob. If it exists, it will be overwritten.
        /// </summary>
        /// <param name="containerPath">The container path where the file is located (e.g. - 'container1/container2')</param>
        /// <param name="fileName">The name of the file to be written.</param>
        /// <param name="fileContent">The content to write into the file</param>
        public async Task WriteFileAsBlockBlobAsync(string containerPath, string fileName, string fileContent)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, fileName);
            await blobReference.UploadTextAsync(fileContent);
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification =
                "This is a Safe method as implied by the prefix 'Try' in its name, should never throw exceptions, any fault should be bypassed for retry purposes. ")]
        public async Task<string> TryAcquireLeaseAsync(string containerPath, string leaseFullName,
            TimeSpan? duration = null, string proposedLeaseId = null)
        {
            try
            {
                CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, leaseFullName);

                if (await blobReference.ExistsAsync())
                    return await blobReference.AcquireLeaseAsync(duration, proposedLeaseId);

                using (MemoryStream ms = new MemoryStream())
                {
                    await blobReference.UploadFromStreamAsync(ms);
                }

                string result = await blobReference.AcquireLeaseAsync(duration, proposedLeaseId);

                if (!Configuration.SystemConfiguration.SystemDebug) return result;

                blobReference.FetchAttributes();

                _logger.LogTrace($"LeaseDuration = {blobReference.Properties.LeaseDuration}");
                _logger.LogTrace($"LeaseState = {blobReference.Properties.LeaseState}");
                _logger.LogTrace($"LeaseStatus = {blobReference.Properties.LeaseStatus}");

                return result;
            }
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                return null;
            }
        }

        public async Task<string> AcquireLeaseAsync(string containerPath, string leaseFullName,
            TimeSpan? duration = null, string proposedLeaseId = null, byte[] defaultFileContent = null,
            bool createIfNotExist = true)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, leaseFullName);

            if (await blobReference.ExistsAsync())
                return await blobReference.AcquireLeaseAsync(duration, proposedLeaseId);

            if (!createIfNotExist)
                return null;

            using (MemoryStream ms = new MemoryStream(defaultFileContent ?? new byte[0]))
            {
                await blobReference.UploadFromStreamAsync(ms);
            }

            string result = await blobReference.AcquireLeaseAsync(duration, proposedLeaseId);

            if (!Configuration.SystemConfiguration.SystemDebug) return result;

            blobReference.FetchAttributes();

            _logger.LogTrace($"LeaseDuration = {blobReference.Properties.LeaseDuration}");
            _logger.LogTrace($"LeaseState = {blobReference.Properties.LeaseState}");
            _logger.LogTrace($"LeaseStatus = {blobReference.Properties.LeaseStatus}");

            return result;
        }

        public async Task ReleaseLeaseAsync(string containerPath, string leaseFullName, string leaseId)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, leaseFullName);
            AccessCondition accessCondition = new AccessCondition
            {
                LeaseId = leaseId
            };

            await blobReference.ReleaseLeaseAsync(accessCondition);

            if (!Configuration.SystemConfiguration.SystemDebug) return;

            blobReference.FetchAttributes();

            _logger.LogTrace($"LeaseDuration = {blobReference.Properties.LeaseDuration}");
            _logger.LogTrace($"LeaseState = {blobReference.Properties.LeaseState}");
            _logger.LogTrace($"LeaseStatus = {blobReference.Properties.LeaseStatus}");
        }

        public bool CreateContainerPath(string path)
        {
            Task<bool> t = CreateContainerPathAsync(path);
            t.Wait();
            return t.Result;
        }

        public async Task<bool> CreateContainerPathAsync(string path)
        {
            _logger.LogTrace($"Creating container if it does not exist: {path}");
            return await ClientWrapper.GetContainerReference(path).CreateIfNotExistsAsync();
        }

        public IEnumerable<ICloudBlobItemWrapper> ListAllBlobs(string rootContainer, string path)
        {
            Task<IEnumerable<ICloudBlobItemWrapper>> t = ListAllBlobsAsync(rootContainer, path);
            t.Wait();
            return t.Result;
        }

        public async Task<IEnumerable<ICloudBlobItemWrapper>> ListAllBlobsAsync(string rootContainer, string path)
        {
            return await ListAllBlobsAsync(rootContainer, path, BlobListingDetails.All);
        }

        public async Task<IEnumerable<ICloudBlobItemWrapper>> ListAllBlobsAsync(string rootContainer, string path, BlobListingDetails blobListingDetails)
        {
            List<CloudBlobItemWrapper> result = new List<CloudBlobItemWrapper>();
            CloudBlobContainer cloudBlobContainer = ClientWrapper.GetContainerReference(rootContainer);
            // List the blobs in the container.
            _logger.LogTrace($"List blobs from accountWrapper {ClientWrapper.BaseUri} in container {path}");
            BlobContinuationToken blobContinuationToken = null;

            do
            {
                BlobResultSegment results = await cloudBlobContainer.ListBlobsSegmentedAsync(
                    path,
                    true,
                    blobListingDetails,
                    int.MaxValue,
                    blobContinuationToken,
                    new BlobRequestOptions(),
                    new OperationContext());

                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                result.AddRange(results.Results.Select(item => new CloudBlobItemWrapper(item)));
                _logger.LogTrace(
                    $"Loaded {results.Results.Count()} blobs, making a current total of {result.Count} blobs.");
            } while (blobContinuationToken != null); // Loop while the continuation token is not null.

            return result;
        }

        public async Task<BlobProperties> GetBlobProperties(string containerPath, string filename)
        {
            CloudBlockBlob blobReference = GetBlockBlobReference(containerPath, filename);
            await blobReference.FetchAttributesAsync().ConfigureAwait(false);

            return blobReference.Properties;
        }

        #endregion

        #region Static Wrappers

        #region Reading Wrappers

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="ReadFileAsString(string,string)" />
        public static string ReadFileAsString(string connString, string containerPath, string fileName,
            IBlobStorageConfiguration configuration, ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            return service.ReadFileAsString(containerPath, fileName);
        }

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="ReadFileAsStringAsync(string,string)" />
        public static async Task<string> ReadFileAsStringAsync(
            string connString,
            string containerPath,
            string fileName,
            IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            return await service.ReadFileAsStringAsync(containerPath, fileName);
        }

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="ReadFileAsByteArray(string,string)" />
        public static byte[] ReadFileAsByteArray(
            string connString,
            string containerPath,
            string fileName,
            IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            return service.ReadFileAsByteArray(containerPath, fileName);
        }

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="ReadFileAsByteArrayAsync(string,string)" />
        public static async Task<byte[]> ReadFileAsByteArrayAsync(
            string connString,
            string containerPath,
            string fileName,
            IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            return await service.ReadFileAsByteArrayAsync(containerPath, fileName);
        }

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="ReadFileIntoFile(string,string,string,System.IO.FileMode)" />
        public static void ReadFileIntoFile(
            string connString,
            string containerPath,
            string fileName,
            string destinationFileFullPath,
            IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            service.ReadFileIntoFile(containerPath, fileName, destinationFileFullPath);
        }

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="ReadFileIntoFileAsync(string,string,string,System.IO.FileMode)" />
        public static async Task ReadFileIntoFileAsync(
            string connString,
            string containerPath,
            string fileName,
            string destinationFileFullPath,
            IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            await service.ReadFileIntoFileAsync(containerPath, fileName, destinationFileFullPath);
        }

        #endregion

        #region Writing Wrappers

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="WriteFileAsBlockBlob(string,string,byte[])" />
        public static void WriteFileAsBlockBlob(
            string connString,
            string containerPath,
            string fileName,
            byte[] fileContent,
            IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            service.WriteFileAsBlockBlob(containerPath, fileName, fileContent);
        }

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="WriteFileAsBlockBlobAsync(string,string,byte[])" />
        public static async Task WriteFileAsBlockBlobAsync(
            string connString,
            string containerPath,
            string fileName,
            byte[] fileContent,
            IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            await service.WriteFileAsBlockBlobAsync(containerPath, fileName, fileContent);
        }

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="WriteFileAsBlockBlob(string,string,Stream)" />
        public static void WriteFileAsBlockBlob(
            string connString,
            string containerPath,
            string fileName,
            Stream fileContent,
            IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            service.WriteFileAsBlockBlob(containerPath, fileName, fileContent);
        }

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="WriteFileAsBlockBlobAsync(string,string,Stream)" />
        public static async Task WriteFileAsBlockBlobAsync(
            string connString,
            string containerPath,
            string fileName,
            Stream fileContent,
            IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            await service.WriteFileAsBlockBlobAsync(containerPath, fileName, fileContent);
        }

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="WriteFileAsBlockBlob(string,string,string)" />
        public static void WriteFileAsBlockBlob(
            string connString,
            string containerPath,
            string fileName,
            string fileContent,
            IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            service.WriteFileAsBlockBlob(containerPath, fileName, fileContent);
        }

        /// <summary>
        ///     Static method wrapper for single transient usage
        /// </summary>
        /// <seealso cref="WriteFileAsBlockBlobAsync(string,string,string)" />
        public static async Task WriteFileAsBlockBlobAsync(
            string connString,
            string containerPath,
            string fileName,
            string fileContent,
            IBlobStorageConfiguration configuration,
            ILogger logger)
        {
            BlobStorageService service = new BlobStorageService(connString, configuration, logger);
            await service.WriteFileAsBlockBlobAsync(containerPath, fileName, fileContent);
        }

        #endregion

        #endregion
    }
}