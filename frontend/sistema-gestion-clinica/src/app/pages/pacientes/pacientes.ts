import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Paciente, PacientesService } from '../../services/pacientes';

@Component({
  selector: 'app-pacientes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pacientes.html',
  styleUrls: ['./pacientes.scss']
})
export class PacientesComponent implements OnInit {
  pacientes: Paciente[] = [];
  filtrados: Paciente[] = [];

  form: Paciente = { nombres: '', apellidos: '', documento: '' };
  editId: number | null = null;

  showForm = false;
  q = '';
  loading = false;
  msg = '';

  constructor(
    private pacientesService: PacientesService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;

    this.pacientesService.getAll().subscribe({
      next: (data) => {
        this.pacientes = data;
        this.applyFilter();
        this.loading = false;

        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.msg = 'Error cargando pacientes.';
        this.loading = false;

        this.cdr.detectChanges();
      }
    });
  }

  toggleNuevo(): void {
    this.showForm = !this.showForm;
    if (this.showForm) this.resetForm();
  }

  resetForm(): void {
    this.form = {
      nombres: '',
      apellidos: '',
      documento: '',
      telefono: '',
      email: '',
      fechaNacimiento: ''
    };
    this.editId = null;
    this.msg = '';
  }

  applyFilter(): void {
    const t = this.q.trim().toLowerCase();
    if (!t) {
      this.filtrados = [...this.pacientes];
      return;
    }
    this.filtrados = this.pacientes.filter(p =>
      (p.nombres || '').toLowerCase().includes(t) ||
      (p.apellidos || '').toLowerCase().includes(t) ||
      (p.documento || '').toLowerCase().includes(t)
    );
  }

  onSubmit(): void {
    this.msg = '';

    if (!this.form.nombres?.trim() || !this.form.apellidos?.trim() || !this.form.documento?.trim()) {
      this.msg = 'Nombres, Apellidos y Documento son obligatorios.';
      return;
    }

    if (this.editId) {
      this.pacientesService.update(this.editId, this.form).subscribe({
        next: () => {
          this.msg = 'Paciente actualizado.';
          this.showForm = false;
          this.load();
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.msg = err?.error || 'Error actualizando paciente.';
          this.cdr.detectChanges();
        }
      });
    } else {
      this.pacientesService.create(this.form).subscribe({
        next: () => {
          this.msg = 'Paciente creado.';
          this.showForm = false;
          this.load();
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.msg = err?.error || 'Error creando paciente.';
          this.cdr.detectChanges();
        }
      });
    }
  }

  editar(p: Paciente): void {
    this.showForm = true;
    this.editId = p.id ?? null;
    this.form = {
      nombres: p.nombres,
      apellidos: p.apellidos,
      documento: p.documento,
      telefono: p.telefono,
      email: p.email,
      fechaNacimiento: p.fechaNacimiento
    };

    this.cdr.detectChanges();
  }

  eliminar(p: Paciente): void {
    if (!p.id) return;
    const ok = confirm(`¿Eliminar (desactivar) a ${p.nombres} ${p.apellidos}?`);
    if (!ok) return;

    this.pacientesService.delete(p.id).subscribe({
      next: () => {
        this.load();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        alert(err?.error || 'Error eliminando paciente.');
        this.cdr.detectChanges();
      }
    });
  }
}
