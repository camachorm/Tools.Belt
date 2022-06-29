// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration;

namespace Tools.Belt.Common.Services.Ftp
{
    public interface IFtpService :
        IService<FtpStatus, string, string, byte[], FtpRemoteExists, bool, IProgress<FtpProgress>, CancellationToken>,
        IService<string, string, CancellationToken>,
        // NOTE: We exclude CancellationToken from the signature to avoid ambiguity with IService<string, string, CancellationToken>
        IService<byte[], string>,
        IService<bool, string, OperationTargetType, CancellationToken>
    {
        void Connect(
            ILogger logger,
            IConfigurationService config,
            string host,
            string user,
            string password,
            int port = 21,
            SslProtocols protocol = SslProtocols.Tls12,
            FtpEncryptionMode encryptionMode = FtpEncryptionMode.Explicit,
            FtpDataConnectionType dataConnectionType = FtpDataConnectionType.PASV,
            bool dataConnectionEncryption = true);

        Task ConnectAsync(
            ILogger logger,
            IConfigurationService config,
            string host,
            string user,
            string password,
            int port = 21,
            SslProtocols protocol = SslProtocols.Tls12,
            FtpEncryptionMode encryptionMode = FtpEncryptionMode.Explicit,
            FtpDataConnectionType dataConnectionType = FtpDataConnectionType.PASV,
            bool dataConnectionEncryption = true);

        FtpStatus WriteFile(
            ILogger logger,
            IConfigurationService config,
            string folderPath,
            string fileName,
            byte[] fileContent,
            FtpRemoteExists exists = FtpRemoteExists.Overwrite,
            bool createRemoteDir = false,
            IProgress<FtpProgress> progress = null,
            CancellationToken token = default);

        Task<FtpStatus> WriteFileAsync(ILogger logger,
            IConfigurationService config,
            string folderPath,
            string fileName,
            byte[] fileContent,
            FtpRemoteExists exists = FtpRemoteExists.Overwrite,
            bool createRemoteDir = false,
            IProgress<FtpProgress> progress = null,
            CancellationToken token = default);

        string ReadFileAsString(
            ILogger logger,
            IConfigurationService config,
            string fileFullPath,
            CancellationToken token);

        Task<string> ReadFileAsStringAsync(
            ILogger logger,
            IConfigurationService config,
            string fileFullPath,
            CancellationToken token);

        byte[] ReadFileAsBinary(
            ILogger logger,
            IConfigurationService config,
            string fileFullPath,
            CancellationToken token);

        Task<byte[]> ReadFileAsBinaryAsync(
            ILogger logger,
            IConfigurationService config,
            string fileFullPath,
            CancellationToken token);

        bool Exists(ILogger logger, IConfigurationService config, string fileFullPath,
            OperationTargetType operationTargetType);

        Task<bool> ExistsAsync(ILogger logger, IConfigurationService config, string fileFullPath,
            OperationTargetType operationTargetType, CancellationToken token);

        bool Delete(ILogger logger, IConfigurationService config, string fileFullPath,
            OperationTargetType operationTargetType);

        Task<bool> DeleteAsync(ILogger logger, IConfigurationService config, string fileFullPath,
            OperationTargetType operationTargetType, CancellationToken token);

        IEnumerable<FtpListItem> ListFolder(ILogger logger, IConfigurationService config, string fullPath,
            FtpListOption ftpListOption = FtpListOption.AllFiles);

        Task<IEnumerable<FtpListItem>> ListFolderAsync(
            ILogger logger,
            IConfigurationService config,
            string fullPath,
            CancellationToken token,
            FtpListOption ftpListOption = FtpListOption.AllFiles);
    }
}