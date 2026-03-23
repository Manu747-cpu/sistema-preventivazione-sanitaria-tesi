using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preventivatore.Infrastructure.Services;
using Preventivatore.Core.DTOs;       // per tutti i Create*/Update*/Login*/RegisterDto
using Preventivatore.Core.Settings;  // per JwtSettings
using Preventivatore.Core.Interfaces;
using AutoMapper;



namespace Preventivatore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public FilesController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpGet]
        public async Task<IActionResult> ListFiles()
        {
            var names = await _storageService.ListAsync();
            return Ok(names);
        }

        [HttpPost]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nessun file ricevuto o file vuoto.");

            var blobName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            await using var stream = file.OpenReadStream();
            var uri = await _storageService.UploadAsync(stream, blobName);

            return Ok(new { FileName = blobName, Url = uri.ToString() });
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> DownloadFile(string name)
        {
            var stream = await _storageService.DownloadAsync(name);
            if (stream == null)
                return NotFound($"File '{name}' non trovato.");

            return File(stream, "application/octet-stream", name);
        }
    }
}
