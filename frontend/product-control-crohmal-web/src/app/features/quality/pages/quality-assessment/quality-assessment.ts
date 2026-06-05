import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { Batch } from '../../../../core/models/batch.model';
import { AnalysisResult } from '../../../../core/models/analysis-result.model';
import { ProductQualitySpecification } from '../../../../core/models/product-quality-specification.model';

import { BatchService } from '../../../../core/services/batch.service';
import { AnalysisResultService } from '../../../../core/services/analysis-result.service';
import { ProductQualitySpecificationService } from '../../../../core/services/product-quality-specification.service';
import { QualityAssessmentService } from '../../../../core/services/quality-assessment.service';

interface QualityAssessmentRow {
  qualityParameterId: number;
  parameterName: string;
  unit?: string | null;

  minValue?: number | null;
  maxValue?: number | null;
  textNorm?: string | null;

  numericValue?: number | null;
  textValue?: string | null;
  isWithinNorm?: boolean | null;
  comment?: string | null;
}

@Component({
  selector: 'app-quality-assessment',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './quality-assessment.html',
  styleUrl: './quality-assessment.css',
})
export class QualityAssessment implements OnInit {
  batchId = 0;

  batch: Batch | null = null;
  rows: QualityAssessmentRow[] = [];

  isLoading = false;
  isSaving = false;
  isAssessmentFinal = false;

  errorMessage = '';
  successMessage = '';

  selectedStatus: 'Approved' | 'Rejected' = 'Approved';
  conclusion = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private batchService: BatchService,
    private analysisResultService: AnalysisResultService,
    private specificationService: ProductQualitySpecificationService,
    private qualityAssessmentService: QualityAssessmentService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.batchId = Number(this.route.snapshot.paramMap.get('id'));

    if (!this.batchId) {
      this.errorMessage = 'Некоректний ID партії.';
      return;
    }

    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.batchService.getBatchById(this.batchId).subscribe({
      next: (batch) => {
        this.batch = batch;

        console.log(batch);

        if (!batch.isAnalysisCompleted) {
          this.errorMessage = 'Аналізи для цієї партії ще не завершені.';
          this.isLoading = false;
          return;
        }

        this.loadSpecificationsAndResults(batch);
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося завантажити партію.';
        this.isLoading = false;
      },
    });
  }

  loadSpecificationsAndResults(batch: Batch): void {
    this.specificationService.getSpecificationsByProductId(batch.productId).subscribe({
      next: (specifications) => {
        this.analysisResultService.getResultsByBatchId(batch.id).subscribe({
          next: (results) => {
            console.log('SPECIFICATIONS:', specifications);
            console.log('ANALYSIS RESULTS:', results);

            this.rows = this.buildRows(specifications, results);
            this.prepareDefaultAssessment();

            this.loadExistingAssessment();

            this.isLoading = false;
            this.cdr.detectChanges();
          },
          error: (error) => {
            console.error('ANALYSIS RESULTS ERROR:', error);

            this.errorMessage = 'Не вдалося завантажити результати аналізів.';
            this.isLoading = false;
            this.cdr.detectChanges();
          },
        });
      },
      error: (error) => {
        console.error('SPECIFICATIONS ERROR:', error);

        this.errorMessage = 'Не вдалося завантажити специфікації якості.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  loadExistingAssessment(): void {
    this.qualityAssessmentService.getByBatchId(this.batchId).subscribe({
      next: (assessment) => {
        console.log(assessment);

        this.selectedStatus = assessment.isApproved ? 'Approved' : 'Rejected';
        this.conclusion = assessment.conclusion;

        this.isAssessmentFinal = assessment.isFinal;

        console.log(assessment);

        this.cdr.detectChanges();
      },
      error: (error) => {
        if (error.status === 404) {
          this.isAssessmentFinal = false;
          return;
        }

        console.error('LOAD ASSESSMENT ERROR:', error);
        this.errorMessage = 'Не вдалося завантажити існуючу оцінку якості.';
        this.cdr.detectChanges();
      },
    });
  }

  finalizeAssessment(): void {
    if (!this.validateAssessment()) {
      return;
    }

    const confirmed = confirm(
      'Після фіналізації оцінку не можна буде редагувати. Якщо партія придатна, сертифікат якості буде сформовано автоматично. Продовжити?',
    );

    if (!confirmed) {
      return;
    }

    const userId = Number(localStorage.getItem('userId'));
    const request = this.buildAssessmentRequest();

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.qualityAssessmentService
      .finalizeAssessment(this.batch!.id, request, userId)
      .pipe(
        finalize(() => {
          this.isSaving = false;
          this.cdr.detectChanges();
        }),
      )
      .subscribe({
        next: (assessment) => {
          this.selectedStatus = assessment.isApproved ? 'Approved' : 'Rejected';
          this.conclusion = assessment.conclusion;
          this.isAssessmentFinal = assessment.isFinal;

          this.successMessage = assessment.isApproved
            ? 'Оцінку якості фіналізовано. Сертифікат якості сформовано автоматично.'
            : 'Оцінку якості фіналізовано. Сертифікат не формується, оскільки партія не відповідає вимогам.';

          this.loadData();
        },
        error: (error) => {
          console.error('FINALIZE ASSESSMENT ERROR:', error);

          this.errorMessage = error.error?.message ?? 'Не вдалося фіналізувати оцінку якості.';
        },
      });
  }

  private buildAssessmentRequest() {
    if (!this.batch) {
      throw new Error('Batch is not loaded.');
    }

    return {
      batchId: this.batch.id,
      isApproved: this.selectedStatus === 'Approved',
      conclusion: this.conclusion.trim(),

      resultDecisions: this.rows.map((row) => ({
        qualityParameterId: row.qualityParameterId,
        isWithinNorm: row.isWithinNorm,
      })),
    };
  }

  private validateAssessment(): boolean {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.batch) {
      this.errorMessage = 'Партія не завантажена.';
      return false;
    }

    const uncheckedRows = this.rows.filter(
      (row) => row.isWithinNorm === null || row.isWithinNorm === undefined,
    );

    if (uncheckedRows.length > 0) {
      this.errorMessage = 'Оцініть усі показники якості перед збереженням або фіналізацією.';
      return false;
    }

    if (!this.conclusion.trim()) {
      this.errorMessage = 'Вкажіть висновок інженера якості.';
      return false;
    }

    const userId = Number(localStorage.getItem('userId'));

    if (!userId) {
      this.errorMessage = 'Користувач не авторизований.';
      return false;
    }

    return true;
  }

  buildRows(
    specifications: ProductQualitySpecification[],
    results: AnalysisResult[],
  ): QualityAssessmentRow[] {
    return specifications.map((specification) => {
      const result = results.find((x) => x.qualityParameterId === specification.qualityParameterId);

      return {
        qualityParameterId: specification.qualityParameterId,
        parameterName: specification.qualityParameter?.name || '—',
        unit: specification.qualityParameter?.unit || null,

        minValue: specification.minValue,
        maxValue: specification.maxValue,
        textNorm: specification.textNorm,

        numericValue: result?.numericValue ?? null,
        textValue: result?.textValue ?? null,

        // стартовая оценка инженера = рекомендация системы
        isWithinNorm: result?.isWithinNorm ?? null,

        comment: result?.comment ?? null,
      };
    });
  }

  prepareDefaultAssessment(): void {
    const hasUncheckedRows = this.rows.some(
      (row) => row.isWithinNorm === null || row.isWithinNorm === undefined,
    );

    if (hasUncheckedRows) {
      this.selectedStatus = 'Rejected';
      this.conclusion = 'Партія потребує завершення оцінювання всіх показників якості.';
      return;
    }

    const allRowsAreApproved = this.rows.every((row) => row.isWithinNorm === true);

    if (allRowsAreApproved) {
      this.selectedStatus = 'Approved';
      this.conclusion = 'Партія відповідає встановленим вимогам специфікації.';
    } else {
      this.selectedStatus = 'Rejected';
      this.conclusion = 'Партія не відповідає встановленим вимогам специфікації.';
    }
  }

  saveAssessment(): void {
    if (!this.validateAssessment()) {
      return;
    }

    const userId = Number(localStorage.getItem('userId'));
    const request = this.buildAssessmentRequest();

    this.isSaving = true;

    this.qualityAssessmentService
      .saveAssessment(this.batch!.id, request, userId)
      .pipe(
        finalize(() => {
          this.isSaving = false;
          this.cdr.detectChanges();
        }),
      )
      .subscribe({
        next: (assessment) => {
          this.successMessage = 'Оцінку якості збережено.';

          this.selectedStatus = assessment.isApproved ? 'Approved' : 'Rejected';
          this.conclusion = assessment.conclusion;
          this.isAssessmentFinal = assessment.isFinal;
        },
        error: (error) => {
          console.error('SAVE ASSESSMENT ERROR:', error);

          this.errorMessage = error.error?.message ?? 'Не вдалося зберегти оцінку якості.';
        },
      });
  }

  isTextRow(row: QualityAssessmentRow): boolean {
    const hasTextNorm = !!row.textNorm;
    const hasNumericNorm =
      (row.minValue !== null && row.minValue !== undefined) ||
      (row.maxValue !== null && row.maxValue !== undefined);

    return hasTextNorm && !hasNumericNorm;
  }

  getNormText(row: QualityAssessmentRow): string {
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

  getFactValue(row: QualityAssessmentRow): string {
    if (row.numericValue !== null && row.numericValue !== undefined) {
      return `${row.numericValue} ${row.unit || ''}`;
    }

    if (row.textValue) {
      return row.textValue;
    }

    return '—';
  }

  getResultText(row: QualityAssessmentRow): string {
    if (row.isWithinNorm === true) {
      return 'В нормі';
    }

    if (row.isWithinNorm === false) {
      return 'Не в нормі';
    }

    if (this.isTextRow(row)) {
      return 'Потребує оцінки інженера';
    }

    return 'Потребує перевірки';
  }

  getResultClass(row: QualityAssessmentRow): string {
    if (row.isWithinNorm === true) {
      return 'result-good';
    }

    if (row.isWithinNorm === false) {
      return 'result-bad';
    }

    return 'result-warning';
  }

  formatDate(date?: string | null): string {
    if (!date) {
      return '—';
    }

    return new Date(date).toLocaleDateString('uk-UA');
  }

  onTextRowDecisionChange(row: QualityAssessmentRow, value: string): void {
    if (value === 'true') {
      row.isWithinNorm = true;
    } else if (value === 'false') {
      row.isWithinNorm = false;
    } else {
      row.isWithinNorm = null;
    }

    this.prepareDefaultAssessment();
  }

  getTextDecisionValue(row: QualityAssessmentRow): string {
    if (row.isWithinNorm === true) {
      return 'true';
    }

    if (row.isWithinNorm === false) {
      return 'false';
    }

    return '';
  }

  onEngineerDecisionChange(row: QualityAssessmentRow, value: string): void {
    if (value === 'true') {
      row.isWithinNorm = true;
    } else if (value === 'false') {
      row.isWithinNorm = false;
    } else {
      row.isWithinNorm = null;
    }

    this.prepareDefaultAssessment();
  }

  getEngineerDecisionValue(row: QualityAssessmentRow): string {
    if (row.isWithinNorm === true) {
      return 'true';
    }

    if (row.isWithinNorm === false) {
      return 'false';
    }

    return '';
  }
}
