using Amazon.S3;
using Amazon.S3.Model;
using Estudaki.Commons.Core.Storage;
using Microsoft.Extensions.Options;

namespace Estudaki.Infrastructure.Crosscutting.Storage;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly StorageSettings _settings;

    public S3StorageService(IAmazonS3 s3Client, IOptions<StorageSettings> settings)
    {
        _s3Client = s3Client;
        _settings = settings.Value;
    }
    
    public string GetFileUrl()
    {
        // Retorna apenas a URL base do bucket
        return _settings.BaseUrl.TrimEnd('/');
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = fileName,
            InputStream = fileStream,
            ContentType = contentType
        };

        await _s3Client.PutObjectAsync(putRequest);

        return $"{GetFileUrl()}/{fileName}";
    }

    public async Task<bool> FileExistsAsync(string fileName)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _settings.BucketName,
                Key = fileName
            };

            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task DeleteFileAsync(string fileName)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = fileName
        };

        await _s3Client.DeleteObjectAsync(deleteRequest);
    }

    public async Task<List<string>> ListFilesAsync(string folderPath)
    {
        var files = new List<string>();
        string? continuationToken = null;

        do
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _settings.BucketName,
                Prefix = folderPath.TrimStart('/'),
                ContinuationToken = continuationToken
            };

            var response = await _s3Client.ListObjectsV2Async(request);
            files.AddRange(response.S3Objects.Select(o => o.Key));
            continuationToken = response.NextContinuationToken;

        } while (continuationToken != null);

        return files;
    }

    public async Task<string> UploadImageFromLocalAsync(string localFilePath, string s3FolderPath)
    {
        if (!File.Exists(localFilePath))
            throw new FileNotFoundException($"Arquivo local não encontrado: {localFilePath}");

        var extension = Path.GetExtension(localFilePath);
        var guid = Guid.NewGuid().ToString();
        var newFileName = $"{guid}{extension}";
        var s3Key = $"{s3FolderPath.TrimStart('/')}/{newFileName}";

        var contentType = extension.ToLower() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };

        using var fileStream = File.OpenRead(localFilePath);
        await UploadFileAsync(fileStream, s3Key, contentType);

        // Retorna apenas o GUID sem extensão (mantendo consistência com ImageBlock.Key)
        return guid;
    }
}
