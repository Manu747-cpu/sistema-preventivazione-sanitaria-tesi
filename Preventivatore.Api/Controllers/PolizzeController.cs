using Microsoft.AspNetCore.Mvc;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Interfaces;

namespace Preventivatore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PolizzeController : ControllerBase
    {
        private readonly IPolizzaService _svc;
        public PolizzeController(IPolizzaService svc)
            => _svc = svc;

        // POST: api/Polizze
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePolizzaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, null);
        }

        // GET: api/Polizze
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _svc.ListAsync());

        // GET: api/Polizze/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var p = await _svc.GetByIdAsync(id);
            return p is null ? NotFound() : Ok(p);
        }

        // PUT: api/Polizze/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePolizzaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.Id = id;
            await _svc.UpdateAsync(dto);
            return NoContent();
        }

        // DELETE: api/Polizze/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
    }
}
