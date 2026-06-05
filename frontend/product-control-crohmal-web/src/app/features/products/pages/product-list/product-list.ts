import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { Product } from '../../../../core/models/product.model';
import { ProductService } from '../../../../core/services/product.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css'
})
export class ProductList implements OnInit {
  products: Product[] = [];

  isLoading = false;
  errorMessage = '';

    constructor(
    private productService: ProductService,
    private cdr: ChangeDetectorRef
    ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

loadProducts(): void {
  console.log('LOAD PRODUCTS START');

  this.isLoading = true;
  this.errorMessage = '';

  this.productService.getProducts()
    .pipe(
      finalize(() => {
        console.log('LOAD PRODUCTS FINALIZE');
        this.isLoading = false;
        this.cdr.detectChanges();
      })
    )
    .subscribe({
      next: (data) => {
        console.log('PRODUCTS RESPONSE:', data);
        this.products = data;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('PRODUCTS ERROR:', error);
        this.errorMessage = 'Не вдалося завантажити список продуктів.';
        this.cdr.detectChanges();
      }
    });
}

  getStatusText(isActive: boolean): string {
    return isActive ? 'Активний' : 'Неактивний';
  }

  getStatusClass(isActive: boolean): string {
    return isActive ? 'status-active' : 'status-inactive';
  }
}