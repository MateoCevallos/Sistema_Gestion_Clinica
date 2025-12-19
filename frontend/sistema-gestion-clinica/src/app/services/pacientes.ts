import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Paciente {
  id?: number;
  nombres: string;
  apellidos: string;
  documento: string;
  telefono?: string;
  email?: string;
  fechaNacimiento?: string;
}

@Injectable({ providedIn: 'root' })
export class PacientesService {
  private api = 'http://localhost:5094/api/Pacientes';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Paciente[]> {
    return this.http.get<Paciente[]>(this.api);
  }

  create(p: Paciente): Observable<any> {
    return this.http.post(this.api, p);
  }

  update(id: number, p: Paciente): Observable<any> {
    return this.http.put(`${this.api}/${id}`, p);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.api}/${id}`);
  }
}
