import { Routes } from '@angular/router';
import { PacientesComponent } from './pages/pacientes/pacientes';
import { MedicosComponent } from './pages/medicos/medicos';
import { CitasComponent } from './pages/citas/citas';
import { ReportesComponent } from './pages/reportes/reportes';

export const routes: Routes = [
  { path: '', redirectTo: 'pacientes', pathMatch: 'full' },
  { path: 'pacientes', component: PacientesComponent },
  { path: 'medicos', component: MedicosComponent },
  { path: 'citas', component: CitasComponent },
  { path: 'reportes', component: ReportesComponent },
  { path: '**', redirectTo: 'pacientes' }
];
