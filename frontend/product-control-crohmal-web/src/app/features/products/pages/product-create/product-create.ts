import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { ProductService } from '../../../../core/services/product.service';
import { Product } from '../../../../core/models/product.model';

@Component({
  selector: 'app-product-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './product-create.html',
  styleUrl: './product-create.css'
})
export class ProductCreate {
  isLoading = false;
  successMessage = '';
  errorMessage = '';

  productForm;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService
  ) {
    this.productForm = this.fb.group({
      Code: ['', [Validators.required, Validators.maxLength(50)]],
      Name: ['', [Validators.required, Validators.maxLength(200)]],
      Unit: ['кг', [Validators.required, Validators.maxLength(30)]],
      Description: ['', [Validators.maxLength(500)]],
      IsActive: [true]
    });
  }

  createProduct(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    const request: Product = {
      code: this.productForm.value.Code ?? '',
      name: this.productForm.value.Name ?? '',
      unit: this.productForm.value.Unit ?? 'кг',
      description: this.productForm.value.Description || undefined,
      isActive: this.productForm.value.IsActive ?? true,
    };

    this.isLoading = true;

    this.productService.createProduct(request).subscribe({
      next: () => {
        this.successMessage = 'Продукт успішно створено.';
        this.errorMessage = '';
        this.isLoading = false;

        this.productForm.reset({
          Code: '',
          Name: '',
          Unit: 'кг',
          Description: '',
          IsActive: true
        });
      },
      error: (error) => {
        console.error(error);
        this.successMessage = '';
        this.errorMessage = 'Помилка під час створення продукту.';
        this.isLoading = false;
      }
    });
  }
}