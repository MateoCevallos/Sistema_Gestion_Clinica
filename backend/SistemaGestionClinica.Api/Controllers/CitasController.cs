using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionClinica.Api.Data;
using SistemaGestionClinica.Api.DTOs;
using SistemaGestionClinica.Api.Models;

namespace SistemaGestionClinica.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CitasController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var citas = await _db.Citas
                .Where(c => c.Activo)
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .OrderByDescending(c => c.FechaHora)
                .Select(c => new CitaListDto
                {
                    Id = c.Id,
                    FechaHora = c.FechaHora,
                    Motivo = c.Motivo,
                    Estado = c.Estado,

                    PacienteId = c.PacienteId,
                    PacienteNombre = (c.Paciente != null)
                        ? (c.Paciente.Nombres + " " + c.Paciente.Apellidos)
                        : "",

                    MedicoId = c.MedicoId,
                    MedicoNombre = (c.Medico != null)
                        ? (c.Medico.Nombres + " " + c.Medico.Apellidos)
                        : "",
                    MedicoEspecialidad = c.Medico != null ? c.Medico.Especialidad : ""
                })
                .ToListAsync();

            return Ok(citas);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Cita cita)
        {

            if (cita.PacienteId <= 0 || cita.MedicoId <= 0)
                return BadRequest("PacienteId y MedicoId son obligatorios.");

            if (cita.FechaHora == default)
                return BadRequest("FechaHora es obligatoria.");

            var pacienteOk = await _db.Pacientes.AnyAsync(p => p.Id == cita.PacienteId && p.Activo);
            if (!pacienteOk) return BadRequest("El paciente no existe o está inactivo.");

            var medicoOk = await _db.Medicos.AnyAsync(m => m.Id == cita.MedicoId && m.Activo);
            if (!medicoOk) return BadRequest("El médico no existe o está inactivo.");

            var choqueMedico = await _db.Citas.AnyAsync(c =>
                c.Activo &&
                c.MedicoId == cita.MedicoId &&
                c.FechaHora == cita.FechaHora);

            if (choqueMedico)
                return Conflict("El médico ya tiene una cita registrada en esa fecha y hora.");

            cita.Id = 0;
            cita.Activo = true;
            cita.CreatedAt = DateTime.Now;

            _db.Citas.Add(cita);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = cita.Id }, cita);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Cita input)
        {
            var cita = await _db.Citas.FirstOrDefaultAsync(c => c.Id == id);
            if (cita == null) return NotFound("Cita no encontrada.");

            if (input.PacienteId <= 0 || input.MedicoId <= 0)
                return BadRequest("PacienteId y MedicoId son obligatorios.");

            if (input.FechaHora == default)
                return BadRequest("FechaHora es obligatoria.");

            var pacienteOk = await _db.Pacientes.AnyAsync(p => p.Id == input.PacienteId && p.Activo);
            if (!pacienteOk) return BadRequest("El paciente no existe o está inactivo.");

            var medicoOk = await _db.Medicos.AnyAsync(m => m.Id == input.MedicoId && m.Activo);
            if (!medicoOk) return BadRequest("El médico no existe o está inactivo.");

            var choqueMedico = await _db.Citas.AnyAsync(c =>
                c.Activo &&
                c.Id != id &&
                c.MedicoId == input.MedicoId &&
                c.FechaHora == input.FechaHora);

            if (choqueMedico)
                return Conflict("El médico ya tiene una cita registrada en esa fecha y hora.");

            cita.PacienteId = input.PacienteId;
            cita.MedicoId = input.MedicoId;
            cita.FechaHora = input.FechaHora;
            cita.Motivo = input.Motivo;
            cita.Estado = string.IsNullOrWhiteSpace(input.Estado) ? cita.Estado : input.Estado;

            cita.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            return Ok(cita);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cita = await _db.Citas.FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null)
                return NotFound(new { message = "Cita no encontrada." });

            cita.Activo = false;
            cita.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Cita desactivada correctamente." });
        }

    }
}
