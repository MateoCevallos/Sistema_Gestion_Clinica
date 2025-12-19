import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ReporteCitasMensual {
  anio: number;
  mes: number;
  total: number;
  programadas: number;
  confirmadas: number;
  canceladas: number;
  atendidas: number;
}

export interface ReporteCitasPorMedico {
  medicoId: number;
  medicoNombre: string;
  especialidad: string;
  total: number;
  programadas: number;
  confirmadas: number;
  canceladas: number;
  atendidas: number;
}

@Injectable({ providedIn: 'root' })
export class ReportesService {
  private api = 'http://localhost:5094/api/Reportes';

  constructor(private http: HttpClient) {}

  citasMensualRango(desde: string, hasta: string): Observable<ReporteCitasMensual[]> {
    return this.http.get<ReporteCitasMensual[]>(
      `${this.api}/citas-mensual-rango?desde=${desde}&hasta=${hasta}`
    );
  }

  citasPorMedico(desde: string, hasta: string): Observable<ReporteCitasPorMedico[]> {
    return this.http.get<ReporteCitasPorMedico[]>(
      `${this.api}/citas-por-medico?desde=${desde}&hasta=${hasta}`
    );
  }

  resumenPdf(
    desde: string,
    hasta: string,
    mensual: boolean,
    medico: boolean
  ): Observable<Blob> {
    const qs = `desde=${desde}&hasta=${hasta}&mensual=${mensual}&medico=${medico}`;
    return this.http.get(`${this.api}/resumen-pdf?${qs}`, { responseType: 'blob' });
  }

}
