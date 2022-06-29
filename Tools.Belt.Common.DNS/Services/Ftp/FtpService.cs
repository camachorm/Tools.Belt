using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Common.Services.Ftp
{
    // TODO: We exclude this from code coverage as it is currently reporting coverage wrongly on most of the covered methods until we can properly investigate and identify causes for that
    [ExcludeFromCodeCoverage]
    public class FtpService : IFtpService
    {
        private readonly IFtpClient _client;

        public FtpService(IFtpClient client)
        {
            _client = client;
        }

        #region Connection Methods

        public void Connect(
            ILogger logger,
            IConfigurationService config,
            string host,
            string user,
            string password,
            int port = 21,
            SslProtocols protocol = SslProtocols.Tls12,
            FtpEncryptionMode encryptionMode = FtpEncryptionMode.Explicit,
            FtpDataConnectionType dataConnectionType = FtpDataConnectionType.PASV,
            bool dataConnectionEncryption = true)
        {
            Task t = ConnectAsync(
                logger,
                config,
                host,
                user,
                password,
                port,
                protocol,
                encryptionMode,
                dataConnectionType,
                dataConnectionEncryption);

            t.Wait();
        }

        public async Task ConnectAsync(
            ILogger logger,
            IConfigurationService config,
            string host,
            string user,
            string password,
            int port = 21,
            SslProtocols protocol = SslProtocols.Tls12,
            FtpEncryptionMode encryptionMode = FtpEncryptionMode.Explicit,
            FtpDataConnectionType dataConnectionType = FtpDataConnectionType.PASV,
            bool dataConnectionEncryption = true)
        {
            _client.Host = host;
            _client.Port = port;
            _client.Credentials = new NetworkCredential(user, password);
            _client.SslProtocols = protocol;
            _client.EncryptionMode = encryptionMode;
            _client.DataConnectionType = dataConnectionType;
            _client.DataConnectionEncryption = dataConnectionEncryption;
            logger.LogTrace(
                $"Connecting to Host: {host}, Port: {port}, User: {user}, Password: {password.ToPrintableSecret(5, 1)}, SslProtocol: {protocol}, EncryptionMode: {_client.EncryptionMode}");

            await _client.ConnectAsync().ConfigureAwait(false);
            logger.LogTrace(
                $"Connected to Host: {host}, Port: {port}, User: {user}, Password: {password.ToPrintableSecret(5, 1)}, SslProtocol: {protocol}, EncryptionMode: {_client.EncryptionMode}");
        }

        #endregion

        #region Write File

        [ExcludeFromCodeCoverage]
        public FtpStatus WriteFile(
            ILogger logger,
            IConfigurationService config,
            string folderPath,
            string fileName,
            byte[] fileContent,
            FtpRemoteExists exists = FtpRemoteExists.Overwrite,
            bool createRemoteDir = false,
            IProgress<FtpProgress> progress = null,
            CancellationToken token = default)
        {
            Task<FtpStatus> t = WriteFileAsync(
                logger,
                config,
                folderPath,
                fileName,
                fileContent,
                exists,
                createRemoteDir,
                progress,
                token);

            t.Wait(token);
            return t.Result;
        }

        [ExcludeFromCodeCoverage]
        public async Task<FtpStatus> WriteFileAsync(ILogger logger,
            IConfigurationService config,
            string folderPath,
            string fileName,
            byte[] fileContent,
            FtpRemoteExists exists = FtpRemoteExists.Overwrite,
            bool createRemoteDir = false,
            IProgress<FtpProgress> progress = null,
            CancellationToken token = default)
        {
            if (fileContent == null) throw new ArgumentNullException(nameof(fileContent));
            logger.LogTrace($"Writing to Folder: {folderPath}, File: {fileName}, Content Size: {fileContent.Length}");
            logger.LogTrace(
                $"\tWriting Behaviour Settings FtpExists: {exists}, createRemoteDir: {createRemoteDir}, IProgress<FtpProgress>: {progress}");
            using MemoryStream memoryStream = new MemoryStream(fileContent);
            return await _client.UploadAsync(
                memoryStream,
                Path.Combine(folderPath, fileName),
                exists,
                createRemoteDir,
                progress,
                token).ConfigureAwait(false);
        }

        #endregion

        #region Read File

        [ExcludeFromCodeCoverage]
        public string ReadFileAsString(
            ILogger logger,
            IConfigurationService config,
            string fileFullPath,
            CancellationToken token)
        {
            Task<string> t = ReadFileAsStringAsync(logger, config, fileFullPath, token);
            t.Wait(token);
            return t.Result;
        }

        [ExcludeFromCodeCoverage]
        public async Task<string> ReadFileAsStringAsync(
            ILogger logger,
            IConfigurationService config,
            string fileFullPath,
            CancellationToken token)
        {
            logger.LogTrace($"Reading File As String: '{fileFullPath}'");

            using (Stream stream = await _client.OpenReadAsync(fileFullPath, token).ConfigureAwait(false))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    return await sr.ReadToEndAsync().ConfigureAwait(false);
                }
            }
        }

        [ExcludeFromCodeCoverage]
        public byte[] ReadFileAsBinary(
            ILogger logger,
            IConfigurationService config,
            string fileFullPath,
            CancellationToken token)
        {
            Task<byte[]> t = ReadFileAsBinaryAsync(logger, config, fileFullPath, token);
            t.Wait(token);
            return t.Result;
        }

        [ExcludeFromCodeCoverage]
        public async Task<byte[]> ReadFileAsBinaryAsync(
            ILogger logger,
            IConfigurationService config,
            string fileFullPath,
            CancellationToken token)
        {
            logger.LogTrace($"Reading File As Binary: '{fileFullPath}'");

            using (Stream stream = await _client.OpenReadAsync(fileFullPath, token).ConfigureAwait(false))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms).ConfigureAwait(false);
                    return ms.ToArray();
                }
            }
        }

        #endregion

        #region Check File

        [ExcludeFromCodeCoverage]
        public bool Exists(ILogger logger, IConfigurationService config, string path,
            OperationTargetType operationTargetType)
        {
            Task<bool> t = ExistsAsync(logger, config, path, operationTargetType, default);
            t.Wait();
            return t.Result;
        }

        [ExcludeFromCodeCoverage]
        public async Task<bool> ExistsAsync(ILogger logger, IConfigurationService config, string path,
            OperationTargetType operationTargetType, CancellationToken token)
        {
            logger.LogTrace($"Checking if {operationTargetType}, Exists as {path}");
            return operationTargetType == OperationTargetType.Directory
                ? await _client.DirectoryExistsAsync(path, token).ConfigureAwait(false)
                : await _client.FileExistsAsync(path, token).ConfigureAwait(false);
        }

        #endregion

        #region Delete File

        [ExcludeFromCodeCoverage]
        public bool Delete(ILogger logger, IConfigurationService config, string path,
            OperationTargetType operationTargetType)
        {
            Task<bool> t = DeleteAsync(logger, config, path, operationTargetType, default);
            t.Wait();
            return t.Result;
        }

        [ExcludeFromCodeCoverage]
        public async Task<bool> DeleteAsync(
            ILogger logger,
            IConfigurationService config,
            string fileFullPath,
            OperationTargetType operationTargetType,
            CancellationToken cancellationToken)
        {
            logger.LogTrace($"Deleting {operationTargetType}, Exists as {fileFullPath}");
            if (OperationTargetType.Directory == operationTargetType)
                await _client.DeleteDirectoryAsync(fileFullPath, cancellationToken).ConfigureAwait(false);
            else
                await _client.DeleteFileAsync(fileFullPath, cancellationToken).ConfigureAwait(false);

            return true;
        }

        public IEnumerable<FtpListItem> ListFolder(ILogger logger, IConfigurationService config, string fullPath,
            FtpListOption ftpListOption = FtpListOption.AllFiles)
        {
            Task<IEnumerable<FtpListItem>> t = ListFolderAsync(logger, config, fullPath, default, ftpListOption);
            t.Wait();
            return t.Result;
        }

        public async Task<IEnumerable<FtpListItem>> ListFolderAsync(
            ILogger logger,
            IConfigurationService config,
            string fullPath,
            CancellationToken token,
            FtpListOption ftpListOption = FtpListOption.AllFiles)
        {
            return await _client.GetListingAsync(fullPath, ftpListOption, token).ConfigureAwait(false);
        }

        #endregion

        #region IService Implementations

        [ExcludeFromCodeCoverage]
        async Task<FtpStatus>
            IService<FtpStatus, string, string, byte[], FtpRemoteExists, bool, IProgress<FtpProgress>, CancellationToken
            >.
            ExecuteAsync(
                ILogger logger,
                IConfigurationService config,
                string folderPath,
                string fileName,
                byte[] fileContent,
                FtpRemoteExists exists,
                bool createRemoteDir,
                IProgress<FtpProgress> progress,
                CancellationToken token)
        {
            return await WriteFileAsync(
                logger,
                config,
                folderPath,
                fileName,
                fileContent,
                exists,
                createRemoteDir,
                progress,
                token).ConfigureAwait(false);
        }

        [ExcludeFromCodeCoverage]
        async Task<string> IService<string, string, CancellationToken>.ExecuteAsync(ILogger logger,
            IConfigurationService config, string fileFullPath, CancellationToken token)
        {
            return await ReadFileAsStringAsync(logger, config, fileFullPath, token).ConfigureAwait(false);
        }

        // NOTE: We exclude CancellationToken from the signature to avoid ambiguity with IService<string, string, CancellationToken>
        [ExcludeFromCodeCoverage]
        async Task<byte[]> IService<byte[], string>.ExecuteAsync(ILogger logger, IConfigurationService config,
            string fileFullPath)
        {
            return await ReadFileAsBinaryAsync(logger, config, fileFullPath, default).ConfigureAwait(false);
        }

        [ExcludeFromCodeCoverage]
        public async Task<bool> ExecuteAsync(ILogger logger, IConfigurationService config, string path,
            OperationTargetType operationTargetType, CancellationToken token)
        {
            return await ExistsAsync(logger, config, path, operationTargetType, token).ConfigureAwait(false);
        }

        #endregion
    }
}