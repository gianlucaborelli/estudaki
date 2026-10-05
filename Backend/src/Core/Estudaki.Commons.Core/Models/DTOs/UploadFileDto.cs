using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace Estudaki.Commons.Core.Models.DTOs
{
    public class UploadFileDto
    {
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

        public string FileName { get; init; } = string.Empty;
        public string ContentType { get; init; } = string.Empty;
        public byte[] Content { get; init; } = [];

        private UploadFileDto() { }       

        public Stream OpenReadStream()
        {
            return new MemoryStream(Content);
        }
    }
}
