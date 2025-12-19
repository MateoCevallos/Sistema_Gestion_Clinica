import { Component, ChangeDetectorRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import {
  ReportesService,
  ReporteCitasMensual,
  ReporteCitasPorMedico
} from '../../services/reportes';

@Component({
  selector: 'app-reportes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reportes.html',
  styleUrls: ['./reportes.scss']
})
export class ReportesComponent implements OnInit {

  private currentYear = new Date().getFullYear();

  desde = `${this.currentYear}-01-01`;
  hasta = `${this.currentYear}-12-31`;

  selMensual = true;
  selMedico = true;

  mensual: ReporteCitasMensual[] = [];
  porMedico: ReporteCitasPorMedico[] = [];

  loadingMensual = false;
  loadingMedico = false;
  loadingPdf = false;

  msg = '';

  constructor(private reportes: ReportesService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.generarSeleccion();
  }

  generarSeleccion(): void {
    this.msg = '';

    this.loadingMensual = false;
    this.loadingMedico = false;

    if (!this.selMensual && !this.selMedico) {
      this.mensual = [];
      this.porMedico = [];
      this.msg = 'Selecciona al menos un reporte.';
      this.cdr.detectChanges();
      return;
    }

    if (this.selMensual) this.cargarMensual();
    else this.mensual = [];

    if (this.selMedico) this.cargarPorMedico();
    else this.porMedico = [];

    this.cdr.detectChanges();
  }

  cargarMensual(): void {
    this.loadingMensual = true;

    this.reportes.citasMensualRango(this.desde, this.hasta)
      .pipe(finalize(() => {
        this.loadingMensual = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: d => {
          this.mensual = d;
        },
        error: err => {
          console.error('citasMensualRango ERROR:', err);
          this.msg = 'Error cargando reporte mensual.';
        }
      });
  }

  cargarPorMedico(): void {
    this.loadingMedico = true;

    this.reportes.citasPorMedico(this.desde, this.hasta)
      .pipe(finalize(() => {
        this.loadingMedico = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: d => {
          this.porMedico = d;
        },
        error: err => {
          console.error('citasPorMedico ERROR:', err);
          this.msg = 'Error cargando reporte por médico.';
        }
      });
  }

  descargarPdfGeneral(): void {
    this.msg = '';

    if (!this.selMensual && !this.selMedico) {
      this.msg = 'Selecciona al menos un reporte para exportar.';
      this.cdr.detectChanges();
      return;
    }

    this.loadingPdf = true;

    this.reportes.resumenPdf(this.desde, this.hasta, this.selMensual, this.selMedico)
      .pipe(finalize(() => {
        this.loadingPdf = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(new Blob([blob], { type: 'application/pdf' }));
          const a = document.createElement('a');
          a.href = url;
          a.download = `reporte_${this.desde}_a_${this.hasta}.pdf`;
          a.click();
          window.URL.revokeObjectURL(url);
        },
        error: (err) => {
          console.error('PDF ERROR:', err);
          this.msg = 'Error generando PDF.';
        }
      });
  }

  imprimir(): void {
    if (!this.selMensual && !this.selMedico) {
      this.msg = 'Selecciona al menos un reporte para imprimir.';
      this.cdr.detectChanges();
      return;
    }
    window.print();
  }

  mesNombre(m: number): string {
    const meses = ['Ene','Feb','Mar','Abr','May','Jun','Jul','Ago','Sep','Oct','Nov','Dic'];
    return meses[m - 1] ?? `${m}`;
  }
}
