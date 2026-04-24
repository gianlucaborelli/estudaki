using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
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
}
