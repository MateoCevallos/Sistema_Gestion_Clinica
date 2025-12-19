using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionClinica.Api.Data;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SistemaGestionClinica.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ReportesController(AppDbContext db)
        {
            _db = db;
        }

        private static bool TryParseDate(string value, out DateTime date)
        {

            return DateTime.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date
            );
        }

        private static string MesNombre(int m) => m switch
        {
            1 => "Enero", 2 => "Febrero", 3 => "Marzo", 4 => "Abril",
            5 => "Mayo", 6 => "Junio", 7 => "Julio", 8 => "Agosto",
            9 => "Septiembre", 10 => "Octubre", 11 => "Noviembre", 12 => "Diciembre",
            _ => m.ToString()
        };

        private static IContainer CellHeader(IContainer c) =>
            c.Background(Colors.Grey.Lighten3)
             .Padding(6)
             .Border(1)
             .BorderColor(Colors.Grey.Lighten1);

        private static IContainer Cell(IContainer c) =>
            c.Padding(6)
             .Border(1)
             .BorderColor(Colors.Grey.Lighten2);

        [HttpGet("citas-mensual")]
        public async Task<IActionResult> CitasMensual([FromQuery] int year)
        {
            if (year <= 0) return BadRequest("El parámetro 'year' es obligatorio.");

            var data = await _db.Citas
                .Where(c => c.Activo && c.FechaHora.Year == year)
                .GroupBy(c => new { c.FechaHora.Year, c.FechaHora.Month })
                .Select(g => new
                {
                    anio = g.Key.Year,
                    mes = g.Key.Month,
                    total = g.Count(),
                    programadas = g.Count(x => x.Estado == "Programada"),
                    confirmadas = g.Count(x => x.Estado == "Confirmada"),
                    canceladas = g.Count(x => x.Estado == "Cancelada"),
                    atendidas = g.Count(x => x.Estado == "Atendida")
                })
                .OrderBy(x => x.mes)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("citas-mensual-pdf")]
        public async Task<IActionResult> CitasMensualPdf([FromQuery] int year)
        {
            if (year <= 0) return BadRequest("El parámetro 'year' es obligatorio.");

            var data = await _db.Citas
                .Where(c => c.Activo && c.FechaHora.Year == year)
                .GroupBy(c => new { c.FechaHora.Year, c.FechaHora.Month })
                .Select(g => new
                {
                    anio = g.Key.Year,
                    mes = g.Key.Month,
                    total = g.Count(),
                    programadas = g.Count(x => x.Estado == "Programada"),
                    confirmadas = g.Count(x => x.Estado == "Confirmada"),
                    canceladas = g.Count(x => x.Estado == "Cancelada"),
                    atendidas = g.Count(x => x.Estado == "Atendida")
                })
                .OrderBy(x => x.mes)
                .ToListAsync();

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Text($"Reporte - Citas por mes ({year})")
                        .SemiBold().FontSize(16);

                    page.Content().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Mes
                            columns.RelativeColumn();  // Total
                            columns.RelativeColumn();  // Programadas
                            columns.RelativeColumn();  // Confirmadas
                            columns.RelativeColumn();  // Canceladas
                            columns.RelativeColumn();  // Atendidas
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellHeader).Text("MES");
                            header.Cell().Element(CellHeader).Text("TOTAL");
                            header.Cell().Element(CellHeader).Text("PROG.");
                            header.Cell().Element(CellHeader).Text("CONF.");
                            header.Cell().Element(CellHeader).Text("CANC.");
                            header.Cell().Element(CellHeader).Text("ATEND.");
                        });

                        foreach (var r in data)
                        {
                            table.Cell().Element(Cell).Text(MesNombre(r.mes));
                            table.Cell().Element(Cell).Text(r.total.ToString());
                            table.Cell().Element(Cell).Text(r.programadas.ToString());
                            table.Cell().Element(Cell).Text(r.confirmadas.ToString());
                            table.Cell().Element(Cell).Text(r.canceladas.ToString());
                            table.Cell().Element(Cell).Text(r.atendidas.ToString());
                        }

                        if (data.Count == 0)
                            table.Cell().ColumnSpan(6).Element(Cell).Text("Sin datos");
                    });

                    page.Footer().AlignRight().Text(txt =>
                    {
                        txt.Span("Generado: ").SemiBold();
                        txt.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"reporte_citas_mensual_{year}.pdf");
        }

        [HttpGet("citas-mensual-rango")]
        public async Task<IActionResult> CitasMensualRango([FromQuery] string desde, [FromQuery] string hasta)
        {
            if (string.IsNullOrWhiteSpace(desde) || string.IsNullOrWhiteSpace(hasta))
                return BadRequest("Parámetros 'desde' y 'hasta' son obligatorios (YYYY-MM-DD).");

            if (!TryParseDate(desde, out var d))
                return BadRequest("Formato inválido en 'desde'. Use YYYY-MM-DD.");

            if (!TryParseDate(hasta, out var h))
                return BadRequest("Formato inválido en 'hasta'. Use YYYY-MM-DD.");

            if (h < d)
                return BadRequest("'hasta' no puede ser menor que 'desde'.");

            var hastaInclusive = h.Date.AddDays(1);

            var data = await _db.Citas
                .Where(c => c.Activo && c.FechaHora >= d.Date && c.FechaHora < hastaInclusive)
                .GroupBy(c => new { anio = c.FechaHora.Year, mes = c.FechaHora.Month })
                .Select(g => new
                {
                    anio = g.Key.anio,
                    mes = g.Key.mes,
                    total = g.Count(),
                    programadas = g.Count(x => x.Estado == "Programada"),
                    confirmadas = g.Count(x => x.Estado == "Confirmada"),
                    canceladas = g.Count(x => x.Estado == "Cancelada"),
                    atendidas = g.Count(x => x.Estado == "Atendida")
                })
                .OrderBy(x => x.anio).ThenBy(x => x.mes)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("citas-por-medico")]
        public async Task<IActionResult> CitasPorMedico([FromQuery] string desde, [FromQuery] string hasta)
        {
            if (string.IsNullOrWhiteSpace(desde) || string.IsNullOrWhiteSpace(hasta))
                return BadRequest("Parámetros 'desde' y 'hasta' son obligatorios (YYYY-MM-DD).");

            if (!TryParseDate(desde, out var d))
                return BadRequest("Formato inválido en 'desde'. Use YYYY-MM-DD.");

            if (!TryParseDate(hasta, out var h))
                return BadRequest("Formato inválido en 'hasta'. Use YYYY-MM-DD.");

            if (h < d)
                return BadRequest("'hasta' no puede ser menor que 'desde'.");

            var hastaInclusive = h.Date.AddDays(1);

            var data = await _db.Citas
                .Where(c => c.Activo && c.FechaHora >= d.Date && c.FechaHora < hastaInclusive)
                .Join(_db.Medicos,
                    c => c.MedicoId,
                    m => m.Id,
                    (c, m) => new { c, m })
                .GroupBy(x => new { x.m.Id, x.m.Nombres, x.m.Apellidos, x.m.Especialidad })
                .Select(g => new
                {
                    medicoId = g.Key.Id,
                    medicoNombre = (g.Key.Nombres + " " + g.Key.Apellidos),
                    especialidad = g.Key.Especialidad,
                    total = g.Count(),
                    programadas = g.Count(x => x.c.Estado == "Programada"),
                    confirmadas = g.Count(x => x.c.Estado == "Confirmada"),
                    canceladas = g.Count(x => x.c.Estado == "Cancelada"),
                    atendidas = g.Count(x => x.c.Estado == "Atendida")
                })
                .OrderByDescending(x => x.total)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("resumen-pdf")]
        public async Task<IActionResult> ResumenPdf(
            [FromQuery] string desde,
            [FromQuery] string hasta,
            [FromQuery] bool mensual = true,
            [FromQuery] bool medico = true
        )
        {
            if (!mensual && !medico)
                return BadRequest("Debe seleccionar al menos un reporte (mensual o medico).");

            if (string.IsNullOrWhiteSpace(desde) || string.IsNullOrWhiteSpace(hasta))
                return BadRequest("Parámetros 'desde' y 'hasta' son obligatorios (YYYY-MM-DD).");

            if (!TryParseDate(desde, out var d))
                return BadRequest("Formato inválido en 'desde'. Use YYYY-MM-DD.");

            if (!TryParseDate(hasta, out var h))
                return BadRequest("Formato inválido en 'hasta'. Use YYYY-MM-DD.");

            if (h < d)
                return BadRequest("'hasta' no puede ser menor que 'desde'.");

            var hastaInclusive = h.Date.AddDays(1);

            var mensualData = new List<dynamic>();
            if (mensual)
            {
                mensualData = await _db.Citas
                    .Where(c => c.Activo && c.FechaHora >= d.Date && c.FechaHora < hastaInclusive)
                    .GroupBy(c => new { anio = c.FechaHora.Year, mes = c.FechaHora.Month })
                    .Select(g => new
                    {
                        anio = g.Key.anio,
                        mes = g.Key.mes,
                        total = g.Count(),
                        programadas = g.Count(x => x.Estado == "Programada"),
                        confirmadas = g.Count(x => x.Estado == "Confirmada"),
                        canceladas = g.Count(x => x.Estado == "Cancelada"),
                        atendidas = g.Count(x => x.Estado == "Atendida")
                    })
                    .OrderBy(x => x.anio).ThenBy(x => x.mes)
                    .ToListAsync<dynamic>();
            }

            var porMedicoData = new List<dynamic>();
            if (medico)
            {
                porMedicoData = await _db.Citas
                    .Where(c => c.Activo && c.FechaHora >= d.Date && c.FechaHora < hastaInclusive)
                    .Join(_db.Medicos,
                        c => c.MedicoId,
                        m => m.Id,
                        (c, m) => new { c, m })
                    .GroupBy(x => new { x.m.Id, x.m.Nombres, x.m.Apellidos, x.m.Especialidad })
                    .Select(g => new
                    {
                        medicoId = g.Key.Id,
                        medicoNombre = (g.Key.Nombres + " " + g.Key.Apellidos),
                        especialidad = g.Key.Especialidad,
                        total = g.Count(),
                        programadas = g.Count(x => x.c.Estado == "Programada"),
                        confirmadas = g.Count(x => x.c.Estado == "Confirmada"),
                        canceladas = g.Count(x => x.c.Estado == "Cancelada"),
                        atendidas = g.Count(x => x.c.Estado == "Atendida")
                    })
                    .OrderByDescending(x => x.total)
                    .ToListAsync<dynamic>();
            }

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Text("Reporte General - Sistema Gestión Clínica")
                        .SemiBold().FontSize(16);

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Rango: {d:yyyy-MM-dd} a {h:yyyy-MM-dd}")
                                  .FontSize(11)
                                  .FontColor(Colors.Grey.Darken1);

                        if (mensual)
                        {
                            col.Item().PaddingTop(12).Text("1) Citas por mes (rango)")
                                .SemiBold().FontSize(13);

                            col.Item().PaddingTop(6).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2); 
                                    columns.RelativeColumn();  
                                    columns.RelativeColumn();  
                                    columns.RelativeColumn();  
                                    columns.RelativeColumn();  
                                    columns.RelativeColumn();  
                                });

                                table.Header(hh =>
                                {
                                    hh.Cell().Element(CellHeader).Text("MES");
                                    hh.Cell().Element(CellHeader).Text("TOTAL");
                                    hh.Cell().Element(CellHeader).Text("PROG.");
                                    hh.Cell().Element(CellHeader).Text("CONF.");
                                    hh.Cell().Element(CellHeader).Text("CANC.");
                                    hh.Cell().Element(CellHeader).Text("ATEND.");
                                });

                                foreach (var r in mensualData)
                                {
                                    table.Cell().Element(Cell).Text($"{MesNombre((int)r.mes)} {(int)r.anio}");
                                    table.Cell().Element(Cell).Text(((int)r.total).ToString());
                                    table.Cell().Element(Cell).Text(((int)r.programadas).ToString());
                                    table.Cell().Element(Cell).Text(((int)r.confirmadas).ToString());
                                    table.Cell().Element(Cell).Text(((int)r.canceladas).ToString());
                                    table.Cell().Element(Cell).Text(((int)r.atendidas).ToString());
                                }

                                if (mensualData.Count == 0)
                                    table.Cell().ColumnSpan(6).Element(Cell).Text("Sin datos");
                            });

                            if (medico)
                                col.Item().PaddingTop(14).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        }

                        if (medico)
                        {
                            col.Item().PaddingTop(12).Text("2) Citas por médico (rango)")
                                .SemiBold().FontSize(13);

                            col.Item().PaddingTop(6).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); 
                                    columns.RelativeColumn(2); 
                                    columns.RelativeColumn();  
                                    columns.RelativeColumn();  
                                    columns.RelativeColumn();  
                                    columns.RelativeColumn();  
                                    columns.RelativeColumn();  
                                });

                                table.Header(hh =>
                                {
                                    hh.Cell().Element(CellHeader).Text("MÉDICO");
                                    hh.Cell().Element(CellHeader).Text("ESPECIALIDAD");
                                    hh.Cell().Element(CellHeader).Text("TOTAL");
                                    hh.Cell().Element(CellHeader).Text("PROG.");
                                    hh.Cell().Element(CellHeader).Text("CONF.");
                                    hh.Cell().Element(CellHeader).Text("CANC.");
                                    hh.Cell().Element(CellHeader).Text("ATEND.");
                                });

                                foreach (var r in porMedicoData)
                                {
                                    table.Cell().Element(Cell).Text((string)r.medicoNombre);
                                    table.Cell().Element(Cell).Text((string)r.especialidad);
                                    table.Cell().Element(Cell).Text(((int)r.total).ToString());
                                    table.Cell().Element(Cell).Text(((int)r.programadas).ToString());
                                    table.Cell().Element(Cell).Text(((int)r.confirmadas).ToString());
                                    table.Cell().Element(Cell).Text(((int)r.canceladas).ToString());
                                    table.Cell().Element(Cell).Text(((int)r.atendidas).ToString());
                                }

                                if (porMedicoData.Count == 0)
                                    table.Cell().ColumnSpan(7).Element(Cell).Text("Sin datos");
                            });
                        }
                    });

                    page.Footer().AlignRight().Text(txt =>
                    {
                        txt.Span("Generado: ").SemiBold();
                        txt.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    });
                });
            }).GeneratePdf();

            return File(
                pdfBytes,
                "application/pdf",
                $"reporte_{d:yyyyMMdd}_{h:yyyyMMdd}.pdf"
            );
        }
    }
}
