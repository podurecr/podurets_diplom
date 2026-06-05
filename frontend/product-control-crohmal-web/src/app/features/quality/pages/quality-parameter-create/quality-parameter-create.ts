import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { QualityParameterService } from '../../../../core/services/quality-parameter.service';
import { CreateQualityParameterDto } from '../../../../core/models/requests/create-quality-parameter.dto';

@Component({
  selector: 'app-quality-parameter-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './quality-parameter-create.html',
  styleUrl: './quality-parameter-create.css',
})
export class QualityParameterCreate {
  isLoading = false;
  successMessage = '';
  errorMessage = '';

  parameterForm;

  constructor(
    private fb: FormBuilder,
    private qualityParameterService: QualityParameterService,
    private router: Router,
  ) {
    this.parameterForm = this.fb.group({
      Name: ['', [Validators.required, Validators.maxLength(150)]],
      Unit: ['', [Validators.maxLength(30)]],
      IsActive: [true],
    });
  }

  createQualityParameter(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.parameterForm.invalid) {
      this.parameterForm.markAllAsTouched();
      return;
    }

    const request: CreateQualityParameterDto = {
      Name: this.parameterForm.value.Name ?? '',
      Unit: this.parameterForm.value.Unit || null,
      IsActive: this.parameterForm.value.IsActive ?? true,
    };

    this.isLoading = true;

    this.qualityParameterService.createQualityParameter(request).subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = 'Показник якості успішно створено.';

        setTimeout(() => {
          this.router.navigate(['/quality-parameters']);
        }, 600);
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Помилка під час створення показника якості.';
        this.isLoading = false;
      },
    });
  }
}
