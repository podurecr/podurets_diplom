import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Batch } from '../../../../core/models/batch.model';
import { QualityCertificate } from '../../../../core/models/quality-certificate.model';
import { AnalysisResult } from '../../../../core/models/analysis-result.model';

import { QualityCertificateService } from '../../../../core/services/quality-certificate.service';
import { AnalysisResultService } from '../../../../core/services/analysis-result.service';

import { forkJoin } from 'rxjs';
import { ShipmentDecision } from '../../../../core/models/shipment-decision.model';
import { ShipmentDecisionService } from '../../../../core/services/shipment-decision.service';
import { AuthService } from '../../../../core/services/auth.service';

interface CertificateRow {
  batch: Batch;
  certificate: QualityCertificate;
  shipmentDecision?: ShipmentDecision | null;
}

@Component({
  selector: 'app-certificate-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './certificate-page.html',
  styleUrl: './certificate-page.css',
})
export class CertificatePage implements OnInit {
  rows: CertificateRow[] = [];

  selectedCertificate: QualityCertificate | null = null;
  selectedBatch: Batch | null = null;
  selectedAnalysisResults: AnalysisResult[] = [];

  canManageShipment = false;

  isLoading = false;
  isLoadingDetails = false;

  errorMessage = '';
  successMessage = '';

  constructor(
    private certificateService: QualityCertificateService,
    private analysisResultService: AnalysisResultService,
    private shipmentDecisionService: ShipmentDecisionService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.canManageShipment = this.authService.hasRole(['Адміністратор', 'Працівник складу']);

    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    forkJoin({
      certificates: this.certificateService.getCertificates(),
      shipmentDecisions: this.shipmentDecisionService.getDecisions(),
    }).subscribe({
      next: ({ certificates, shipmentDecisions }) => {
        this.rows = certificates
          .filter((certificate) => !!certificate.batch)
          .map((certificate) => ({
            batch: certificate.batch!,
            certificate,
            shipmentDecision:
              shipmentDecisions.find((decision) => decision.batchId === certificate.batchId) ??
              null,
          }));

        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося завантажити дані сертифікатів якості.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  previewCertificate(row: CertificateRow): void {
    this.selectedBatch = row.batch;
    this.selectedCertificate = row.certificate;
    this.selectedAnalysisResults = [];

    this.isLoadingDetails = true;

    this.analysisResultService.getResultsByBatchId(row.batch.id).subscribe({
      next: (results) => {
        this.selectedAnalysisResults = results;
        this.isLoadingDetails = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося завантажити результати аналізів.';
        this.isLoadingDetails = false;
        this.cdr.detectChanges();
      },
    });
  }

  closePreview(): void {
    this.selectedBatch = null;
    this.selectedCertificate = null;
    this.selectedAnalysisResults = [];
  }

  downloadCertificate(certificate: QualityCertificate): void {
    this.certificateService.downloadPdf(certificate.id).subscribe({
      next: (blob) => {
        const fileUrl = URL.createObjectURL(blob);

        const link = document.createElement('a');
        link.href = fileUrl;
        link.download = `${certificate.certificateNumber}.pdf`;
        link.click();

        URL.revokeObjectURL(fileUrl);
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося завантажити PDF сертифіката.';
      },
    });
  }

  isApproved(batch: Batch): boolean {
    const status = batch.status?.toString().toLowerCase();

    return (
      (status === '2' ||
        status.includes('approved') ||
        status.includes('придат') ||
        status.includes('readyforshipment')) &&
      !this.isRejected(batch)
    );
  }

  isRejected(batch: Batch): boolean {
    const status = batch.status?.toString().toLowerCase();

    return (
      status === '3' ||
      status.includes('rejected') ||
      status.includes('брак') ||
      status.includes('непридат')
    );
  }

  getBatchStatusText(batch: Batch): string {
    const status = batch.status?.toString();

    const statuses: Record<string, string> = {
      '0': 'Зареєстрована',
      '1': 'На аналізі',
      '2': 'Придатна',
      '3': 'Брак',
      '4': 'Дозволена до відвантаження',
      '5': 'Відвантажена',

      Registered: 'Зареєстрована',
      InAnalysis: 'На аналізі',
      Approved: 'Придатна',
      Rejected: 'Брак',
      ReadyForShipment: 'Дозволена до відвантаження',
      Shipped: 'Відвантажена',
    };

    return statuses[status] ?? status ?? 'Невідомо';
  }

  getBatchStatusClass(batch: Batch): string {
    if (this.isApproved(batch)) {
      return 'status-approved';
    }

    if (this.isRejected(batch)) {
      return 'status-rejected';
    }

    return 'status-default';
  }

  getProductName(batch: Batch): string {
    return batch.product?.name ?? '—';
  }

  getAnalysisValue(result: AnalysisResult): string {
    if (result.numericValue !== null && result.numericValue !== undefined) {
      return `${result.numericValue} ${result.qualityParameter?.unit || ''}`;
    }

    if (result.textValue) {
      return result.textValue;
    }

    return '—';
  }

  getAnalysisStatusText(result: AnalysisResult): string {
    if (result.isWithinNorm === true) {
      return 'В нормі';
    }

    if (result.isWithinNorm === false && result.textValue) {
      return 'Не в нормі';
    }

    return 'Текстове значення';
  }

  getAnalysisStatusClass(result: AnalysisResult): string {
    if (result.isWithinNorm === true) {
      return 'status-approved';
    }

    if (result.isWithinNorm === false) {
      return 'status-rejected';
    }

    return 'status-default';
  }

  formatDate(date?: string | null): string {
    if (!date) {
      return '—';
    }

    return new Date(date).toLocaleDateString('uk-UA');
  }

  allowShipment(row: CertificateRow): void {
    const userId = Number(localStorage.getItem('userId'));

    if (!userId) {
      this.errorMessage = 'Не вдалося визначити користувача.';
      return;
    }

    const confirmed = confirm(`Дозволити відвантаження партії ${row.batch.batchNumber}?`);

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.shipmentDecisionService.allowShipment(row.batch.id, userId).subscribe({
      next: (decision) => {
        row.shipmentDecision = decision;
        this.successMessage = 'Відвантаження партії дозволено.';
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося дозволити відвантаження.';
        this.cdr.detectChanges();
      },
    });
  }

  denyShipment(row: CertificateRow): void {
    const userId = Number(localStorage.getItem('userId'));

    if (!userId) {
      this.errorMessage = 'Не вдалося визначити користувача.';
      return;
    }

    const confirmed = confirm(`Заборонити відвантаження партії ${row.batch.batchNumber}?`);

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.shipmentDecisionService.denyShipment(row.batch.id, userId).subscribe({
      next: (decision) => {
        row.shipmentDecision = decision;
        this.successMessage = 'Відвантаження партії заборонено.';
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося заборонити відвантаження.';
        this.cdr.detectChanges();
      },
    });
  }

  getShipmentDecisionText(decision?: ShipmentDecision | null): string {
    if (!decision) {
      return 'Рішення не прийнято';
    }

    const status = decision.status?.toString();

    const statuses: Record<string, string> = {
      '0': 'Не створено',
      '1': 'Відвантаження дозволено',
      '2': 'Відвантаження заборонено',

      NotCreated: 'Не створено',
      Allowed: 'Відвантаження дозволено',
      Forbidden: 'Відвантаження заборонено',
      Approved: 'Відвантаження дозволено',
      Rejected: 'Відвантаження заборонено',
    };

    return statuses[status] ?? status ?? 'Невідомо';
  }

  getShipmentDecisionClass(decision?: ShipmentDecision | null): string {
    if (!decision) {
      return 'status-default';
    }

    const status = decision.status?.toString().toLowerCase();

    if (status === '1' || status.includes('allowed') || status.includes('approved')) {
      return 'status-approved';
    }

    if (status === '2' || status.includes('forbidden') || status.includes('rejected')) {
      return 'status-rejected';
    }

    return 'status-default';
  }
}
