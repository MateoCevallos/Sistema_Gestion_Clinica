import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Medico {
  id?: number;
  nombres: string;
  apellidos: string;
  especialidad: string;
  telefono?: string;
  email?: string;
}

@Injectable({ providedIn: 'root' })
export class MedicosService {
  private api = 'http://localhost:5094/api/Medicos';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Medico[]> {
    return this.http.get<Medico[]>(this.api);
  }

  create(m: Medico): Observable<any> {
    return this.http.post(this.api, m);
  }

  update(id: number, m: Medico): Observable<any> {
    return this.http.put(`${this.api}/${id}`, m);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.api}/${id}`);
  }
}
