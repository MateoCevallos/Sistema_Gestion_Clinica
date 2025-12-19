using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionClinica.Api.Data;
using SistemaGestionClinica.Api.Models;

namespace SistemaGestionClinica.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PacientesController(AppDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pacientes = await _db.Pacientes
                .Where(p => p.Activo)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return Ok(pacientes);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Paciente paciente)
        {

            if (string.IsNullOrWhiteSpace(paciente.Nombres) ||
                string.IsNullOrWhiteSpace(paciente.Apellidos) ||
                string.IsNullOrWhiteSpace(paciente.Documento))
            {
                return BadRequest("Nombres, Apellidos y Documento son obligatorios.");
            }

            var existeDocumento = await _db.Pacientes.AnyAsync(p => p.Documento == paciente.Documento);
            if (existeDocumento)
                return Conflict("Ya existe un paciente con ese documento.");

            paciente.Id = 0;
            paciente.Activo = true;
            paciente.CreatedAt = DateTime.Now;

            _db.Pacientes.Add(paciente);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = paciente.Id }, paciente);
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Paciente input)
        {
            var paciente = await _db.Pacientes.FirstOrDefaultAsync(p => p.Id == id);
            if (paciente == null) return NotFound("Paciente no encontrado.");

            if (string.IsNullOrWhiteSpace(input.Nombres) ||
                string.IsNullOrWhiteSpace(input.Apellidos) ||
                string.IsNullOrWhiteSpace(input.Documento))
            {
                return BadRequest("Nombres, Apellidos y Documento son obligatorios.");
            }

            var docOcupado = await _db.Pacientes.AnyAsync(p => p.Documento == input.Documento && p.Id != id);
            if (docOcupado) return Conflict("Ya existe otro paciente con ese documento.");

            paciente.Nombres = input.Nombres;
            paciente.Apellidos = input.Apellidos;
            paciente.Documento = input.Documento;
            paciente.Telefono = input.Telefono;
            paciente.Email = input.Email;
            paciente.FechaNacimiento = input.FechaNacimiento;
            paciente.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            return Ok(paciente);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var paciente = await _db.Pacientes.FirstOrDefaultAsync(p => p.Id == id);

            if (paciente == null)
                return NotFound(new { message = "Paciente no encontrado." });

            paciente.Activo = false;
            paciente.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Paciente desactivado correctamente." });
        }

    }
}
