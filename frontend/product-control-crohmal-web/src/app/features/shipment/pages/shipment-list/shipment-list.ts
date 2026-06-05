import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { Batch } from '../../../../core/models/batch.model';
import { ShipmentDecisionService } from '../../../../core/services/shipment-decision.service';

@Component({
  selector: 'app-shipment-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './shipment-list.html',
  styleUrl: './shipment-list.css',
})
export class ShipmentList implements OnInit {
  batches: Batch[] = [];

  isLoading = false;
  errorMessage = '';
  successMessage = '';

  processingBatchId: number | null = null;

  constructor(
    private shipmentDecisionService: ShipmentDecisionService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadBatches();
  }

  loadBatches(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.shipmentDecisionService
      .getBatchesAllowedForShipment()
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        }),
      )
      .subscribe({
        next: (data) => {
          this.batches = data;
        },
        error: (error) => {
          console.error('SHIPMENT BATCHES ERROR:', error);
          this.errorMessage = 'Не вдалося завантажити партії для відвантаження.';
        },
      });
  }

  getStatusText(status: string | number): string {
    if (typeof status === 'number') {
      const statuses: Record<number, string> = {
        0: 'Зареєстрована',
        1: 'На аналізі',
        2: 'Придатна',
        3: 'Не придатна',
        4: 'Дозволена до відвантаження',
        5: 'Відвантажена',
      };

      return statuses[status] ?? 'Невідомо';
    }

    const statuses: Record<string, string> = {
      Registered: 'Зареєстрована',
      InAnalysis: 'На аналізі',
      Approved: 'Придатна',
      Rejected: 'Не придатна',
      ReadyForShipment: 'Дозволена до відвантаження',
      Shipped: 'Відвантажена',
    };

    return statuses[status] ?? status;
  }

  getStatusClass(status: string | number): string {
    const value = status.toString();

    if (value === 'Approved' || value === '2') {
      return 'status-approved';
    }

    if (value === 'ReadyForShipment' || value === '4') {
      return 'status-ready';
    }

    if (value === 'Shipped' || value === '5') {
      return 'status-shipped';
    }

    return 'status-default';
  }

  canCreateShipmentDecision(batch: Batch): boolean {
    const status = batch.status?.toString();

    return status === 'Approved' || status === '2';
  }

  isReadyForShipment(batch: Batch): boolean {
    const status = batch.status?.toString();

    return status === 'ReadyForShipment' || status === '4';
  }

  getCertificateNumber(batch: any): string {
    return (
      batch.qualityCertificate?.certificateNumber ??
      batch.certificate?.certificateNumber ??
      batch.qualityCertificates?.[0]?.certificateNumber ??
      '—'
    );
  }

  getCertificateId(batch: any): number | null {
    return (
      batch.qualityCertificate?.id ??
      batch.certificate?.id ??
      batch.qualityCertificates?.[0]?.id ??
      null
    );
  }

  formatDate(date?: string | null): string {
    if (!date) {
      return '—';
    }

    return new Date(date).toLocaleDateString('uk-UA');
  }
}
