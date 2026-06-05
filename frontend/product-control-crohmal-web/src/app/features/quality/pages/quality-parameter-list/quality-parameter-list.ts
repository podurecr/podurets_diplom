import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { QualityParameterService } from '../../../../core/services/quality-parameter.service';
import { QualityParameter } from '../../../../core/models/quality-parameter.models';

@Component({
  selector: 'app-quality-parameter-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './quality-parameter-list.html',
  styleUrl: './quality-parameter-list.css',
})
export class QualityParameterList implements OnInit {
  qualityParameters: QualityParameter[] = [];

  isLoading = false;
  errorMessage = '';

  constructor(
    private qualityParameterService: QualityParameterService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadQualityParameters();
  }

  loadQualityParameters(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.qualityParameterService.getQualityParameters().subscribe({
      next: (data) => {
        this.qualityParameters = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося завантажити список показників якості.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Активний' : 'Неактивний';
  }

  getStatusClass(isActive: boolean): string {
    return isActive ? 'status-active' : 'status-inactive';
  }
}
