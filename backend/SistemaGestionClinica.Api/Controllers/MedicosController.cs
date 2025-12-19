using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionClinica.Api.Data;
using SistemaGestionClinica.Api.Models;

namespace SistemaGestionClinica.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MedicosController(AppDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var medicos = await _db.Medicos
                .Where(m => m.Activo)
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            return Ok(medicos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Medico medico)
        {
            if (string.IsNullOrWhiteSpace(medico.Nombres) ||
                string.IsNullOrWhiteSpace(medico.Apellidos) ||
                string.IsNullOrWhiteSpace(medico.Especialidad))
            {
                return BadRequest("Nombres, Apellidos y Especialidad son obligatorios.");
            }

            medico.Id = 0;
            medico.Activo = true;
            medico.CreatedAt = DateTime.Now;

            _db.Medicos.Add(medico);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = medico.Id }, medico);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Medico input)
        {
            var medico = await _db.Medicos.FirstOrDefaultAsync(m => m.Id == id);
            if (medico == null) return NotFound("Médico no encontrado.");

            if (string.IsNullOrWhiteSpace(input.Nombres) ||
                string.IsNullOrWhiteSpace(input.Apellidos) ||
                string.IsNullOrWhiteSpace(input.Especialidad))
            {
                return BadRequest("Nombres, Apellidos y Especialidad son obligatorios.");
            }

            medico.Nombres = input.Nombres;
            medico.Apellidos = input.Apellidos;
            medico.Especialidad = input.Especialidad;
            medico.Telefono = input.Telefono;
            medico.Email = input.Email;
            medico.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            return Ok(medico);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var medico = await _db.Medicos.FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null)
                return NotFound(new { message = "Médico no encontrado." });

            medico.Activo = false;
            medico.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Médico desactivado correctamente." });
        }

    }
}
