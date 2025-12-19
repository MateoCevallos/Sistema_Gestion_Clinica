using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionClinica.Api.Models
{
    [Table("medicos")]
    public class Medico
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Column("nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("especialidad")]
        public string Especialidad { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [MaxLength(120)]
        [Column("email")]
        public string? Email { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
