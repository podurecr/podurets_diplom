import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Batch } from '../../../../core/models/batch.model';
import { BatchService } from '../../../../core/services/batch.service';
import { AuthService } from '../../../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-batch-list',
  imports: [RouterLink, CommonModule],
  templateUrl: './batch-list.html',
  styleUrl: './batch-list.css',
})
export class BatchList implements OnInit {
  batches: Batch[] = [];

  isLoading = false;
  errorMessage = '';

  isAdmin = false;
  isLaborant = false;
  isQualityEngineer = false;
  isWarehouseWorker = false;

  constructor(
    private batchService: BatchService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.hasRole(['Адміністратор']);
    this.isLaborant = this.authService.hasRole(['Лаборант']);
    this.isQualityEngineer = this.authService.hasRole(['Інженер з якості']);
    this.isWarehouseWorker = this.authService.hasRole(['Працівник складу']);

    this.loadBatches();
  }

  loadBatches(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.batchService.getBatches().subscribe({
      next: (data) => {
        this.batches = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося завантажити список партій.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  canWorkWithAnalysis(batch: Batch): boolean {
    return this.isAdmin || this.isLaborant;
  }

  canAssessQuality(batch: Batch): boolean {
    if (!(this.isAdmin || this.isQualityEngineer)) {
      return false;
    }

    return batch.isAnalysisCompleted === true;
  }

  canCreateCertificate(batch: Batch): boolean {
    if (!(this.isAdmin || this.isQualityEngineer)) {
      return false;
    }

    return this.isApproved(batch.status);
  }

  private isApproved(status: string | number): boolean {
    return status === 'Approved' || status === 2;
  }

  getAnalysisStatusText(batch: Batch): string {
    return batch.isAnalysisCompleted ? 'Аналіз завершено' : 'Очікує аналіз';
  }

  getAnalysisStatusClass(batch: Batch): string {
    return batch.isAnalysisCompleted ? 'analysis-completed' : 'analysis-pending';
  }

  getStatusText(status: string | number): string {
    if (typeof status === 'number') {
      const numericStatuses: Record<number, string> = {
        0: 'Зареєстрована',
        1: 'На аналізі',
        2: 'Approved',
        3: 'Не Approved',
        4: 'Дозволена до відвантаження',
        5: 'Відвантажена',
      };

      return numericStatuses[status] ?? 'Невідомо';
    }

    const textStatuses: Record<string, string> = {
      Registered: 'Зареєстрована',
      InAnalysis: 'На аналізі',
      Approved: 'Approved',
      Rejected: 'Не Approved',
      ReadyForShipment: 'Дозволена до відвантаження',
      Shipped: 'Відвантажена',
    };

    return textStatuses[status] ?? status;
  }

  getStatusClass(status: string | number): string {
    const value = status.toString();

    if (value === 'Approved' || value === '2') {
      return 'status-approved';
    }

    if (value === 'Rejected' || value === '3') {
      return 'status-rejected';
    }

    if (value === 'ReadyForShipment' || value === '4') {
      return 'status-ready';
    }

    return 'status-default';
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('uk-UA');
  }
}
