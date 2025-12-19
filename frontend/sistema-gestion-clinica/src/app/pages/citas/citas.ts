import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { CitaCreateUpdate, CitaListDto, CitasService } from '../../services/citas';
import { Paciente, PacientesService } from '../../services/pacientes';
import { Medico, MedicosService } from '../../services/medicos';

@Component({
  selector: 'app-citas',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './citas.html',
  styleUrls: ['./citas.scss']
})
export class CitasComponent implements OnInit {
  citas: CitaListDto[] = [];

  filtradas: CitaListDto[] = [];
  q = '';

  pacientes: Paciente[] = [];
  medicos: Medico[] = [];

  showForm = false;
  editId: number | null = null;
  msg = '';
  loading = false;

  form: CitaCreateUpdate = {
    pacienteId: 0,
    medicoId: 0,
    fechaHora: '',
    motivo: '',
    estado: 'Programada'
  };

  constructor(
    private citasService: CitasService,
    private pacientesService: PacientesService,
    private medicosService: MedicosService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCombos();
    this.loadCitas();
  }

  loadCombos(): void {
    this.pacientesService.getAll().subscribe({
      next: d => {
        this.pacientes = d;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err);
        this.cdr.detectChanges();
      }
    });

    this.medicosService.getAll().subscribe({
      next: d => {
        this.medicos = d;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err);
        this.cdr.detectChanges();
      }
    });
  }

  applyFilter(): void {
    const t = this.q.trim().toLowerCase();

    if (!t) {
      this.filtradas = [...this.citas];
      this.cdr.detectChanges();
      return;
    }

    this.filtradas = this.citas.filter(c =>
      (c.pacienteNombre || '').toLowerCase().includes(t) ||
      (c.medicoNombre || '').toLowerCase().includes(t) ||
      (c.medicoEspecialidad || '').toLowerCase().includes(t) ||
      (c.estado || '').toLowerCase().includes(t) ||
      (c.motivo || '').toLowerCase().includes(t)
    );

    this.cdr.detectChanges();
  }

  loadCitas(): void {
    this.loading = true;
    this.citasService.getAll().subscribe({
      next: d => {
        this.citas = d;

        this.applyFilter();

        this.loading = false;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err);
        this.msg = 'Error cargando citas.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  toggleNuevo(): void {
    this.showForm = !this.showForm;
    if (this.showForm) this.resetForm();
    this.cdr.detectChanges();
  }

  resetForm(): void {
    this.editId = null;
    this.msg = '';
    this.form = {
      pacienteId: 0,
      medicoId: 0,
      fechaHora: '',
      motivo: '',
      estado: 'Programada'
    };
    this.cdr.detectChanges();
  }


  private toIso(dtLocal: string): string {
    return dtLocal.length === 16 ? dtLocal + ':00' : dtLocal;
  }

  onSubmit(): void {
    this.msg = '';

    if (!this.form.pacienteId || !this.form.medicoId || !this.form.fechaHora) {
      this.msg = 'Paciente, Médico y Fecha/Hora son obligatorios.';
      this.cdr.detectChanges();
      return;
    }

    const payload: CitaCreateUpdate = {
      ...this.form,
      fechaHora: this.toIso(this.form.fechaHora)
    };

    if (this.editId) {
      this.citasService.update(this.editId, payload).subscribe({
        next: () => {
          this.showForm = false;
          this.loadCitas();
          this.cdr.detectChanges();
        },
        error: err => {
          console.error(err);
          this.msg = err?.error || 'Error actualizando cita.';
          this.cdr.detectChanges();
        }
      });
    } else {
      this.citasService.create(payload).subscribe({
        next: () => {
          this.showForm = false;
          this.loadCitas();
          this.cdr.detectChanges();
        },
        error: err => {
          console.error(err);
          this.msg = err?.error || 'Error creando cita.';
          this.cdr.detectChanges();
        }
      });
    }
  }

  editar(c: CitaListDto): void {
    this.showForm = true;
    this.editId = c.id;

    const dt = c.fechaHora?.slice(0, 16);

    this.form = {
      pacienteId: c.pacienteId,
      medicoId: c.medicoId,
      fechaHora: dt,
      motivo: c.motivo || '',
      estado: c.estado
    };

    this.cdr.detectChanges();
  }


  eliminar(c: CitaListDto): void {
    const ok = confirm(`¿Eliminar (desactivar) la cita #${c.id}?`);
    if (!ok) return;

    const id = Number(c.id);

    this.citasService.delete(id).subscribe({
      next: () => {

        this.citas = this.citas.filter(x => Number(x.id) !== id);
        this.filtradas = this.filtradas.filter(x => Number(x.id) !== id);


        this.applyFilter();

        this.msg = 'Cita eliminada.';
        this.cdr.detectChanges();
      },
      error: err => {
        console.error(err);
        alert(err?.error || 'Error eliminando cita.');
        this.cdr.detectChanges();
      }
    });
  }

}
