using Microsoft.AspNetCore.Components.Forms;

namespace Estudaki.Commons.Core.Models.DTOs
{
    public class UploadFileDto
    {
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

        public string FileName { get; init; } = string.Empty;
        public string ContentType { get; init; } = string.Empty;
        public byte[] Content { get; init; } = [];

        private UploadFileDto() { }

        public static async Task<UploadFileDto> CreateAsync(IBrowserFile file)
        {
            using var fileMs = new MemoryStream();

            await file.OpenReadStream(MaxFileSize)
                      .CopyToAsync(fileMs);

            return new UploadFileDto
            {
                FileName = file.Name,
                ContentType = file.ContentType,
                Content = fileMs.ToArray()
            };
        }

        public Stream OpenReadStream()
        {
            return new MemoryStream(Content);
        }
    }
}
