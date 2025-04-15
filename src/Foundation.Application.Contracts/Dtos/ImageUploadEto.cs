using System;

namespace Foundation.Application.Contracts.Dtos
{
    public class ImageUploadEto
    {
        public Guid ImageId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public byte[] FileContent { get; set; }
    }
}