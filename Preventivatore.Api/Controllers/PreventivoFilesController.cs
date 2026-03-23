// PreventivoFilesController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Preventivatore.Core.Interfaces;
using System;
using System.Threading.Tasks;


namespace Preventivatore.Api.Controllers
{
    [ApiController]
    [Route("api/preventivo/{preventivoId}/files")]
    public class PreventivoFilesController : ControllerBase
    {
        private readonly IPreventivoFileService _fileService;

        public PreventivoFilesController(IPreventivoFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(Guid preventivoId, IFormFile file)
        {
            if (file == null)
                return BadRequest("Nessun file ricevuto.");

            var fileId = await _fileService.UploadAsync(preventivoId, file);
            return CreatedAtAction(nameof(Download), new { preventivoId, fileId }, null);
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> Download(Guid preventivoId, int fileId)
        {
            var stream = await _fileService.DownloadAsync(preventivoId, fileId);
            if (stream == null) return NotFound();
            return File(stream, "application/octet-stream", enableRangeProcessing: true);
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> Delete(Guid preventivoId, int fileId)
        {
            var deleted = await _fileService.DeleteAsync(preventivoId, fileId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
