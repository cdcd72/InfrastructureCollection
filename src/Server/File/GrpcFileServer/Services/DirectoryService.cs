using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcFileServer.Common;
using Infra.Core.FileAccess.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GrpcFileServer.Services
{
    public class DirectoryService : DirectoryTransfer.DirectoryTransferBase
    {
        private readonly ILogger<FileService> _logger;
        private readonly Env _env;
        private readonly IFileAccess _fileAccess;

        public DirectoryService(ILogger<FileService> logger, IConfiguration config, IFileAccess fileAccess)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = new Env(config);
            _fileAccess = fileAccess;
        }

        public override async Task<CreateDirectoryResponse> CreateDirectory(
            CreateDirectoryRequest request,
            ServerCallContext context)
        {
            var startTime = DateTime.Now;
            var mark = request.Mark;
            var directoryPath = Path.Combine(_env.DirectoryRoot, request.DirectoryName);
            var reply = new CreateDirectoryResponse
            {
                Mark = mark
            };

            try
            {
                _logger.LogInformation($"【{mark}】Currently create directory {directoryPath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

                _fileAccess.CreateDirectory(directoryPath);

                _logger.LogInformation($"【{mark}】Create directory completed. SpentTime:{DateTime.Now - startTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"【{mark}】Create directory unexpected exception happened.({ex.GetType()}):{ex.Message}");
            }

            return reply;
        }

        public override async Task<IsExistDirectoryResponse> IsExistDirectory(
            IsExistDirectoryRequest request,
            ServerCallContext context)
        {
            var startTime = DateTime.Now;
            var mark = request.Mark;
            var directoryPath = Path.Combine(_env.DirectoryRoot, request.DirectoryName);
            var reply = new IsExistDirectoryResponse
            {
                Mark = mark
            };

            try
            {
                _logger.LogInformation($"【{mark}】Currently check directory {directoryPath} exist, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

                reply.Status = _fileAccess.DirectoryExists(directoryPath);

                _logger.LogInformation($"【{mark}】Check directory exist completed. SpentTime:{DateTime.Now - startTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"【{mark}】Check directory exist unexpected exception happened.({ex.GetType()}):{ex.Message}");
            }

            return reply;
        }

        public override async Task<GetFilesResponse> GetFiles(
            GetFilesRequest request,
            ServerCallContext context)
        {
            var startTime = DateTime.Now;
            var mark = request.Mark;
            var directoryPath = Path.Combine(_env.DirectoryRoot, request.DirectoryName);
            var reply = new GetFilesResponse
            {
                Mark = mark
            };

            try
            {
                _logger.LogInformation($"【{mark}】Currently get files from {directoryPath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

                if (Enum.TryParse<SearchOption>(request.SearchOption, out var searchOption))
                    reply.FileNames.AddRange(_fileAccess.GetFiles(directoryPath, request.SearchPattern, searchOption));

                _logger.LogInformation($"【{mark}】Get files completed. SpentTime:{DateTime.Now - startTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"【{mark}】Get files unexpected exception happened.({ex.GetType()}):{ex.Message}");
            }

            return reply;
        }

        public override async Task<DeleteDirectoryResponse> DeleteDirectory(
            DeleteDirectoryRequest request,
            ServerCallContext context)
        {
            var startTime = DateTime.Now;
            var mark = request.Mark;
            var directoryPath = Path.Combine(_env.DirectoryRoot, request.DirectoryName);
            var reply = new DeleteDirectoryResponse
            {
                Mark = mark
            };

            try
            {
                _logger.LogInformation($"【{mark}】Currently delete directory {directoryPath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

                _fileAccess.DeleteDirectory(directoryPath, request.Recursive);

                _logger.LogInformation($"【{mark}】Delete directory completed. SpentTime:{DateTime.Now - startTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"【{mark}】Delete directory unexpected exception happened.({ex.GetType()}):{ex.Message}");
            }

            return reply;
        }

        public override async Task<GetSubDirectoriesResponse> GetSubDirectories(
            GetSubDirectoriesRequest request,
            ServerCallContext context)
        {
            var startTime = DateTime.Now;
            var mark = request.Mark;
            var directoryPath = Path.Combine(_env.DirectoryRoot, request.DirectoryName);
            var reply = new GetSubDirectoriesResponse
            {
                Mark = mark
            };

            try
            {
                _logger.LogInformation($"【{mark}】Currently get subdirectories from {directoryPath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

                if (Enum.TryParse<SearchOption>(request.SearchOption, out var searchOption))
                    reply.DirectoryNames.AddRange(_fileAccess.GetSubDirectories(directoryPath, request.SearchPattern, searchOption));

                _logger.LogInformation($"【{mark}】Get subdirectories completed. SpentTime:{DateTime.Now - startTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"【{mark}】Get subdirectories unexpected exception happened.({ex.GetType()}):{ex.Message}");
            }

            return reply;
        }

        public override async Task<DirectoryCompressResponse> DirectoryCompress(
            DirectoryCompressRequest request,
            ServerCallContext context)
        {
            var startTime = DateTime.Now;
            var mark = request.Mark;
            var directoryPath = Path.Combine(_env.DirectoryRoot, request.DirectoryName);
            var zipFilePath = Path.Combine(_env.DirectoryRoot, request.ZipFileName);
            var reply = new DirectoryCompressResponse
            {
                Mark = mark
            };

            try
            {
                _logger.LogInformation($"【{mark}】Currently compress directory {directoryPath} to {zipFilePath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

                _fileAccess.DirectoryCompress(directoryPath, zipFilePath);

                _logger.LogInformation($"【{mark}】Compress directory completed. SpentTime:{DateTime.Now - startTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"【{mark}】Compress directory unexpected exception happened.({ex.GetType()}):{ex.Message}");
            }

            return reply;
        }
    }
}