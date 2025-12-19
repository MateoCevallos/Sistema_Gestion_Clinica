namespace SistemaGestionClinica.Api.DTOs
{
    public class CitaListDto
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Motivo { get; set; }
        public string Estado { get; set; } = "Programada";

        public int PacienteId { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;

        public int MedicoId { get; set; }
        public string MedicoNombre { get; set; } = string.Empty;
        public string MedicoEspecialidad { get; set; } = string.Empty;
    }
}
