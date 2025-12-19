import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CitaListDto {
  id: number;
  fechaHora: string;
  motivo?: string;
  estado: string;

  pacienteId: number;
  pacienteNombre: string;

  medicoId: number;
  medicoNombre: string;
  medicoEspecialidad: string;
}

export interface CitaCreateUpdate {
  pacienteId: number;
  medicoId: number;
  fechaHora: string;
  motivo?: string;
  estado?: string;
}

@Injectable({ providedIn: 'root' })
export class CitasService {
  private api = 'http://localhost:5094/api/Citas';

  constructor(private http: HttpClient) {}

  getAll(): Observable<CitaListDto[]> {
    return this.http.get<CitaListDto[]>(this.api);
  }

  create(c: CitaCreateUpdate): Observable<any> {
    return this.http.post(this.api, c);
  }

  update(id: number, c: CitaCreateUpdate): Observable<any> {
    return this.http.put(`${this.api}/${id}`, c);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.api}/${id}`);
  }
}
