import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Batch } from '../../../../core/models/batch.model';
import { AnalysisResult } from '../../../../core/models/analysis-result.model';
import { ProductQualitySpecification } from '../../../../core/models/product-quality-specification.model';
import { SaveBatchAnalysisRequest } from '../../../../core/save-batch-analysis.dto';

import { BatchService } from '../../../../core/services/batch.service';
import { AnalysisResultService } from '../../../../core/services/analysis-result.service';
import { ProductQualitySpecificationService } from '../../../../core/services/product-quality-specification.service';
import { AuthService } from '../../../../core/services/auth.service';
import { finalize, switchMap } from 'rxjs';

interface AnalysisWorkRow {
  qualityParameterId: number;
  parameterName: string;
  unit?: string | null;

  minValue?: number | null;
  maxValue?: number | null;
  textNorm?: string | null;

  isRequired: boolean;

  numericValue?: number | null;
  textValue?: string | null;
  comment?: string | null;

  isWithinNorm?: boolean | null;
  resultStatus?: string | null;
}

@Component({
  selector: 'app-analysis-work',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './analysis-work.html',
  styleUrl: './analysis-work.css',
})
export class AnalysisWork implements OnInit {
  batches: Batch[] = [];
  selectedBatchId: number | null = null;
  selectedBatch: Batch | null = null;
  isFinishing: boolean = false;
  rows: AnalysisWorkRow[] = [];

  isLoading = false;
  isSaving = false;

  errorMessage = '';
  successMessage = '';

  isAdmin = false;

  constructor(
    private route: ActivatedRoute,
    private batchService: BatchService,
    private analysisResultService: AnalysisResultService,
    private specificationService: ProductQualitySpecificationService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.hasRole(['Адміністратор']);

    this.loadBatches();

    const batchIdFromRoute = Number(this.route.snapshot.paramMap.get('id'));

    if (batchIdFromRoute) {
      this.selectedBatchId = batchIdFromRoute;
      this.loadBatchForAnalysis(batchIdFromRoute);
    }
  }

  get isFormLocked(): boolean {
    return !!this.selectedBatch?.isAnalysisCompleted && !this.isAdmin;
  }

  loadBatches(): void {
    this.batchService.getBatches().subscribe({
      next: (data) => {
        this.batches = data;
        console.log(data);
        console.log(this.batches);
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося завантажити список партій.';
        this.cdr.detectChanges();
      },
    });
  }

  getIsFormLocked(): boolean {
    return !!this.selectedBatch?.isAnalysisCompleted && !this.isAdmin;
  }

  onBatchChange(): void {
    this.successMessage = '';
    this.errorMessage = '';
    this.rows = [];
    this.selectedBatch = null;

    if (!this.selectedBatchId) {
      return;
    }

    this.loadBatchForAnalysis(Number(this.selectedBatchId));
  }

  loadBatchForAnalysis(batchId: number): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.batchService.getBatchById(batchId).subscribe({
      next: (batch) => {
        console.log(batch);
        this.selectedBatch = batch;
        this.isFinishing = batch.isAnalysisCompleted;
        console.log(this.isFinishing);
        this.loadSpecificationsAndResults(batch);
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося завантажити дані партії.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  loadSpecificationsAndResults(batch: Batch): void {
    this.specificationService.getSpecificationsByProductId(batch.productId).subscribe({
      next: (specifications) => {
        this.analysisResultService.getResultsByBatchId(batch.id).subscribe({
          next: (existingResults) => {
            console.log(specifications);
            this.rows = this.buildRows(specifications, existingResults);
            console.log(this.rows);

            this.isLoading = false;
            this.cdr.detectChanges();
          },
          error: (error) => {
            console.error(error);
            this.errorMessage = 'Не вдалося завантажити існуючі результати аналізів.';
            this.isLoading = false;
            this.cdr.detectChanges();
          },
        });
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося завантажити специфікації якості для продукту.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  buildRows(
    specifications: ProductQualitySpecification[],
    existingResults: AnalysisResult[],
  ): AnalysisWorkRow[] {
    return specifications.map((specification) => {
      const existingResult = existingResults.find(
        (result) => result.qualityParameterId === specification.qualityParameterId,
      );

      const row: AnalysisWorkRow = {
        qualityParameterId: specification.qualityParameterId,
        parameterName: specification.qualityParameter?.name || '—',
        unit: specification.qualityParameter?.unit || null,

        minValue: specification.minValue,
        maxValue: specification.maxValue,
        textNorm: specification.textNorm,

        isRequired: specification.isRequired,

        numericValue: existingResult?.numericValue ?? null,
        textValue: existingResult?.textValue ?? null,
        comment: existingResult?.comment ?? null,

        isWithinNorm: existingResult?.isWithinNorm ?? null,
        resultStatus: existingResult?.resultStatus ?? null,
      };

      if (!row.resultStatus) {
        this.recalculateRow(row);
      }

      return row;
    });
  }

  isTextRow(row: AnalysisWorkRow): boolean {
    const hasTextNorm = !!row.textNorm;
    const hasNumericNorm =
      (row.minValue !== null && row.minValue !== undefined) ||
      (row.maxValue !== null && row.maxValue !== undefined);

    return hasTextNorm && !hasNumericNorm;
  }

  isFilled(row: AnalysisWorkRow): boolean {
    if (this.isTextRow(row)) {
      return !!row.textValue && row.textValue.trim().length > 0;
    }

    return row.numericValue !== null && row.numericValue !== undefined;
  }

  get filledCount(): number {
    return this.rows.filter((row) => this.isFilled(row)).length;
  }

  get totalCount(): number {
    return this.rows.length;
  }

  get progressPercent(): number {
    if (this.totalCount === 0) {
      return 0;
    }

    return Math.round((this.filledCount / this.totalCount) * 100);
  }

  getNormText(row: AnalysisWorkRow): string {
    const hasMin = row.minValue !== null && row.minValue !== undefined;
    const hasMax = row.maxValue !== null && row.maxValue !== undefined;

    if (hasMin && hasMax) {
      return `${row.minValue} – ${row.maxValue} ${row.unit || ''}`;
    }

    if (hasMin && !hasMax) {
      return `від ${row.minValue} ${row.unit || ''}`;
    }

    if (!hasMin && hasMax) {
      return `до ${row.maxValue} ${row.unit || ''}`;
    }

    if (row.textNorm) {
      return row.textNorm;
    }

    return '—';
  }

  recalculateRow(row: AnalysisWorkRow): void {
    if (this.isTextRow(row)) {
      row.isWithinNorm = null;

      if (this.isFilled(row)) {
        row.resultStatus = 'TextReview';
      } else {
        row.resultStatus = null;
      }

      return;
    }

    if (row.numericValue === null || row.numericValue === undefined) {
      row.isWithinNorm = null;
      row.resultStatus = null;
      return;
    }

    const value = Number(row.numericValue);

    const hasMin = row.minValue !== null && row.minValue !== undefined;
    const hasMax = row.maxValue !== null && row.maxValue !== undefined;

    let isBelow = false;
    let isAbove = false;

    if (hasMin && value < Number(row.minValue)) {
      isBelow = true;
    }

    if (hasMax && value > Number(row.maxValue)) {
      isAbove = true;
    }

    if (!isBelow && !isAbove) {
      row.isWithinNorm = true;
      row.resultStatus = 'Norm';
      return;
    }

    row.isWithinNorm = false;

    const deviation = this.calculateDeviation(value, row);
    const limit = this.calculateMinorDeviationLimit(row);

    row.resultStatus = deviation <= limit ? 'MinorDeviation' : 'MajorDeviation';
  }

  calculateDeviation(value: number, row: AnalysisWorkRow): number {
    if (row.minValue !== null && row.minValue !== undefined && value < row.minValue) {
      return row.minValue - value;
    }

    if (row.maxValue !== null && row.maxValue !== undefined && value > row.maxValue) {
      return value - row.maxValue;
    }

    return 0;
  }

  calculateMinorDeviationLimit(row: AnalysisWorkRow): number {
    const hasMin = row.minValue !== null && row.minValue !== undefined;
    const hasMax = row.maxValue !== null && row.maxValue !== undefined;

    if (hasMin && hasMax) {
      const range = Math.abs(Number(row.maxValue) - Number(row.minValue));
      return range * 0.1;
    }

    if (hasMax) {
      return Math.abs(Number(row.maxValue)) * 0.1;
    }

    if (hasMin) {
      return Math.abs(Number(row.minValue)) * 0.1;
    }

    return 0;
  }

  getResultText(row: AnalysisWorkRow): string {
    switch (row.resultStatus) {
      case 'Norm':
        return 'Норма';
      case 'MinorDeviation':
        return 'Невелике відхилення';
      case 'MajorDeviation':
        return 'Значне відхилення';
      case 'TextReview':
        return 'Текстова перевірка';
      default:
        return 'Не заповнено';
    }
  }

  getResultClass(row: AnalysisWorkRow): string {
    switch (row.resultStatus) {
      case 'Norm':
        return 'status-good';
      case 'MinorDeviation':
        return 'status-warning';
      case 'MajorDeviation':
        return 'status-bad';
      case 'TextReview':
        return 'status-info';
      default:
        return 'status-default';
    }
  }

  saveAnalysisResults(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.selectedBatch) {
      this.errorMessage = 'Оберіть партію для внесення аналізів.';
      return;
    }

    if (this.isFormLocked) {
      this.errorMessage = 'Результати аналізів уже завершені і недоступні для редагування.';
      return;
    }

    const unfilledRequiredRows = this.rows.filter((row) => row.isRequired && !this.isFilled(row));

    if (unfilledRequiredRows.length > 0) {
      this.errorMessage = 'Заповніть усі обов’язкові показники перед збереженням.';
      return;
    }

    const request: SaveBatchAnalysisRequest = {
      batchId: this.selectedBatch.id,
      results: this.rows
        .filter((row) => this.isFilled(row))
        .map((row) => ({
          qualityParameterId: row.qualityParameterId,
          numericValue: this.isTextRow(row) ? null : (row.numericValue ?? null),
          textValue: this.isTextRow(row) ? (row.textValue ?? null) : null,
          isWithinNorm: row.isWithinNorm ?? null,
          resultStatus: row.resultStatus ?? null,
          comment: row.comment ?? null,
        })),
    };

    this.isSaving = true;

    this.analysisResultService.completeBatchAnalysis(this.selectedBatch.id, request).subscribe({
      next: () => {
        this.successMessage = 'Результати аналізів успішно збережено.';
        this.isSaving = false;
        this.loadBatchForAnalysis(this.selectedBatch!.id);
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Помилка під час збереження результатів аналізів.';
        this.isSaving = false;
      },
    });
  }

  formatDate(date?: string | null): string {
    if (!date) {
      return '—';
    }

    return new Date(date).toLocaleDateString('uk-UA');
  }

  finishAnalysis(): void {
    const batchId = this.selectedBatch?.id ?? 0;

    if (!batchId) {
      this.errorMessage = 'Оберіть партію.';
      return;
    }

    this.saveAnalysisResults();

    this.analysisResultService.finishBatchAnalysis(batchId).subscribe({
      next: () => {
        this.successMessage = 'Аналіз завершено.';
        this.loadBatchForAnalysis(batchId);
      },
      error: (error) => {
        console.error('FINISH ANALYSIS ERROR:', error);
        this.errorMessage = 'Не вдалося завершити аналіз.';
      },
    });
  }
}
