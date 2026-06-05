import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { Product } from '../../../../core/models/product.model';
import { ProductService } from '../../../../core/services/product.service';
import { BatchService } from '../../../../core/services/batch.service';

@Component({
  selector: 'app-batch-create',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './batch-create.html',
  styleUrl: './batch-create.css',
})
export class BatchCreate implements OnInit {
  products: Product[] = [];
  userId: string;

  isProductsLoading = false;
  productsErrorMessage = '';

  isSaving = false;
  errorMessage = '';

  batch = {
    batchNumber: '',
    productionDate: '',
    quantity: 1,
    unit: '',
    productId: null as number | null,
    productionLine: '',
    comment: '',
    createdByUserId: '',
  };

  constructor(
    private productService: ProductService,
    private batchService: BatchService,
    private router: Router,
    private cdr: ChangeDetectorRef,
  ) {
    this.userId = localStorage.getItem('userId') ?? '';
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isProductsLoading = true;
    this.productsErrorMessage = '';

    this.productService
      .getProducts()
      .pipe(
        finalize(() => {
          this.isProductsLoading = false;
          this.cdr.detectChanges();
        }),
      )
      .subscribe({
        next: (products) => {
          this.products = products.filter((product) => product.isActive);
        },
        error: (error) => {
          console.error('PRODUCTS ERROR:', error);
          this.productsErrorMessage = 'Не вдалося завантажити список продуктів.';
        },
      });
  }

  onProductChange(): void {
    const selectedProduct = this.products.find(
      (product) => product.id === Number(this.batch.productId),
    );

    this.batch.unit = selectedProduct?.unit || '';
  }

  saveBatch(): void {
    if (!this.batch.productId) {
      this.errorMessage = 'Оберіть продукт.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    this.batch.createdByUserId = this.userId;

    this.batchService
      .createBatch(this.batch)
      .pipe(
        finalize(() => {
          this.isSaving = false;
          this.cdr.detectChanges();
        }),
      )
      .subscribe({
        next: () => {
          this.router.navigate(['/batches']);
        },
        error: (error) => {
          console.error('CREATE BATCH ERROR:', error);
          this.errorMessage = 'Не вдалося створити партію.';
        },
      });
  }
}
