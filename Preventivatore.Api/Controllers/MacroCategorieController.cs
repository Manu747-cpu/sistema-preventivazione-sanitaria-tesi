using Microsoft.AspNetCore.Mvc;
using Preventivatore.Core.Interfaces;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Preventivatore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MacroCategorieController : ControllerBase
    {
        private readonly IMacroCategoriaService _svc;
        private readonly ISubCategoriaService _subSvc;

        public MacroCategorieController(
            IMacroCategoriaService macroSvc,
            ISubCategoriaService subCategoriaSvc)
        {
            _svc = macroSvc;
            _subSvc = subCategoriaSvc;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MacroCategoriaDto>>> GetAll()
        {
            var entities = await _svc.ListAsync();
            var dtos = entities.Select(e => new MacroCategoriaDto
            {
                Id = e.Id,
                Nome = e.Nome,
                Descrizione = e.Descrizione,
                ImageUrl = e.ImageUrl,
                SubCategorie = new List<SubCategoriaDto>()
            });
            return Ok(dtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<MacroCategoriaDto>> Get(int id)
        {
            var e = await _svc.GetByIdAsync(id);
            if (e is null) return NotFound();

            var dto = new MacroCategoriaDto
            {
                Id = e.Id,
                Nome = e.Nome,
                Descrizione = e.Descrizione,
                ImageUrl = e.ImageUrl
            };

            // Qui prendi le subcategorie
            var subs = await _subSvc.ListByMacroIdAsync(id);
            dto.SubCategorie = subs;

            return Ok(dto);
        }

        // ... gli altri action (POST, PUT, DELETE) restano invariati ...
    }
}
