import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Medico, MedicosService } from '../../services/medicos';

@Component({
  selector: 'app-medicos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './medicos.html',
  styleUrls: ['./medicos.scss']
})
export class MedicosComponent implements OnInit {
  medicos: Medico[] = [];
  filtrados: Medico[] = [];

  form: Medico = { nombres: '', apellidos: '', especialidad: '' };
  editId: number | null = null;

  showForm = false;
  q = '';
  loading = false;
  msg = '';

  constructor(
    private medicosService: MedicosService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.medicosService.getAll().subscribe({
      next: (data) => {
        this.medicos = data;
        this.applyFilter();
        this.loading = false;

        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.msg = 'Error cargando médicos.';
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
    this.form = { nombres: '', apellidos: '', especialidad: '', telefono: '', email: '' };
    this.editId = null;
    this.msg = '';
    this.cdr.detectChanges();
  }

  applyFilter(): void {
    const t = this.q.trim().toLowerCase();
    if (!t) {
      this.filtrados = [...this.medicos];
      return;
    }

    this.filtrados = this.medicos.filter(m =>
      (m.nombres || '').toLowerCase().includes(t) ||
      (m.apellidos || '').toLowerCase().includes(t) ||
      (m.especialidad || '').toLowerCase().includes(t)
    );

    this.cdr.detectChanges();
  }

  onSubmit(): void {
    this.msg = '';

    if (!this.form.nombres?.trim() || !this.form.apellidos?.trim() || !this.form.especialidad?.trim()) {
      this.msg = 'Nombres, Apellidos y Especialidad son obligatorios.';
      this.cdr.detectChanges();
      return;
    }

    if (this.editId) {
      this.medicosService.update(this.editId, this.form).subscribe({
        next: () => {
          this.showForm = false;
          this.load();
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.msg = err?.error || 'Error actualizando médico.';
          this.cdr.detectChanges();
        }
      });
    } else {
      this.medicosService.create(this.form).subscribe({
        next: () => {
          this.showForm = false;
          this.load();
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error(err);
          this.msg = err?.error || 'Error creando médico.';
          this.cdr.detectChanges();
        }
      });
    }
  }

  editar(m: Medico): void {
    this.showForm = true;
    this.editId = m.id ?? null;
    this.form = {
      nombres: m.nombres,
      apellidos: m.apellidos,
      especialidad: m.especialidad,
      telefono: m.telefono,
      email: m.email
    };

    this.cdr.detectChanges();
  }

  eliminar(m: Medico): void {
    if (!m.id) return;
    const ok = confirm(`¿Eliminar (desactivar) a ${m.nombres} ${m.apellidos}?`);
    if (!ok) return;

    this.medicosService.delete(m.id).subscribe({
      next: () => {
        this.load();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        alert(err?.error || 'Error eliminando médico.');
        this.cdr.detectChanges();
      }
    });
  }
}
