using System;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Interfaces;


namespace Preventivatore.Core.DTOs
{
    public class PreventivoFileDto
    {
        public int Id { get; set; }
        public string BlobName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

}
