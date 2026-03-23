using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Interfaces;

namespace Preventivatore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategorieController : ControllerBase
    {
        private readonly ISubCategoriaService _svc;

        public SubCategorieController(ISubCategoriaService svc) => _svc = svc;

        // GET api/SubCategorie/{macroId}
        [HttpGet("{macroId}")]
        public async Task<ActionResult<List<SubCategoriaDto>>> GetByMacro(int macroId)
        {
            var list = await _svc.ListByMacroIdAsync(macroId);
            return Ok(list);
        }

        // GET api/SubCategorie/item/{id}
        [HttpGet("item/{id}", Name = nameof(GetById))]
        public async Task<ActionResult<SubCategoriaDto>> GetById(int id)
        {
            var dto = await _svc.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // POST api/SubCategorie
        [HttpPost]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<ActionResult<SubCategoriaDto>> Create([FromBody] SubCategoriaDto dto)
        {
            var id = await _svc.CreateAsync(dto);
            dto.Id = id;
            return CreatedAtAction(nameof(GetById), new { id }, dto);
        }

        // PUT api/SubCategorie/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] SubCategoriaDto dto)
        {
            await _svc.UpdateAsync(id, dto);
            return NoContent();
        }

        // DELETE api/SubCategorie/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
    }
}
