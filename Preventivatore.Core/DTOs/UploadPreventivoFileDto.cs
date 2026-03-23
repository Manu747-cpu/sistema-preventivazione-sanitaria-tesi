using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Interfaces;


namespace Preventivatore.Core.DTOs
{
    public class UploadPreventivoFileDto
    {
        [Required]
        public IFormFile File { get; set; } = default!;
    }
}
