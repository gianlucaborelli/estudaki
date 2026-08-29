namespace Estudaki.Commons.Core.Storage;

public class StorageSettings
{
    public const string SectionName = "AwsS3";

    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
}
