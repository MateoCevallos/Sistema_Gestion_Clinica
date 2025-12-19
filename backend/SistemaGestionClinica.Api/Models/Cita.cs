using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionClinica.Api.Models
{
    [Table("citas")]
    public class Cita
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("paciente_id")]
        public int PacienteId { get; set; }

        public Paciente? Paciente { get; set; }

        [Column("medico_id")]
        public int MedicoId { get; set; }

        public Medico? Medico { get; set; }

        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; }

        [MaxLength(255)]
        [Column("motivo")]
        public string? Motivo { get; set; }

        [MaxLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "Programada";

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
