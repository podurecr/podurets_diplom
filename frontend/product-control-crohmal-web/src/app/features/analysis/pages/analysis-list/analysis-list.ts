import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { AnalysisResult } from '../../../../core/models/analysis-result.model';
import { AnalysisResultService } from '../../../../core/services/analysis-result.service';

@Component({
  selector: 'app-analysis-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './analysis-list.html',
  styleUrl: './analysis-list.css',
})
export class AnalysisList implements OnInit {
  analysisResults: AnalysisResult[] = [];

  isLoading = false;
  errorMessage = '';

  constructor(
    private analysisResultService: AnalysisResultService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadAnalysisResults();
  }

  loadAnalysisResults(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.analysisResultService.getAnalysisResults().subscribe({
      next: (data) => {
        this.analysisResults = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося завантажити результати лабораторних аналізів.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  getValue(result: AnalysisResult): string {
    if (result.numericValue !== null && result.numericValue !== undefined) {
      return `${result.numericValue} ${result.qualityParameter?.unit || ''}`;
    }

    if (result.textValue) {
      return result.textValue;
    }

    return '—';
  }

  getResultText(status?: string | null): string {
    switch (status) {
      case 'Norm':
        return 'Норма';
      case 'MinorDeviation':
        return 'Невелике відхилення';
      case 'MajorDeviation':
        return 'Значне відхилення';
      case 'TextReview':
        return 'Текстова перевірка';
      default:
        return 'Не визначено';
    }
  }

  getResultClass(status?: string | null): string {
    switch (status) {
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

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('uk-UA');
  }
}
