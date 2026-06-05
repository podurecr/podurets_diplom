import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductQualitySpecification } from '../../../../../core/models/product-quality-specification.model';
import { ProductQualitySpecificationService } from '../../../../../core/services/product-quality-specification.service';

@Component({
  selector: 'app-product-quality-specification-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './product-quality-specification-list.html',
  styleUrl: './product-quality-specification-list.css',
})
export class ProductQualitySpecificationList implements OnInit {
  specifications: ProductQualitySpecification[] = [];

  isLoading = false;
  errorMessage = '';

  constructor(
    private specificationService: ProductQualitySpecificationService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadSpecifications();
  }

  loadSpecifications(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.specificationService.getSpecifications().subscribe({
      next: (data) => {
        this.specifications = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Не вдалося завантажити специфікації якості продуктів.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  getNormText(specification: ProductQualitySpecification): string {
    const hasMin = specification.minValue !== null && specification.minValue !== undefined;
    const hasMax = specification.maxValue !== null && specification.maxValue !== undefined;

    if (hasMin && hasMax) {
      return `${specification.minValue} – ${specification.maxValue}`;
    }

    if (hasMin && !hasMax) {
      return `від ${specification.minValue}`;
    }

    if (!hasMin && hasMax) {
      return `до ${specification.maxValue}`;
    }

    if (specification.textNorm) {
      return specification.textNorm;
    }

    return '—';
  }

  getRequiredText(isRequired: boolean): string {
    return isRequired ? 'Так' : 'Ні';
  }

  getRequiredClass(isRequired: boolean): string {
    return isRequired ? 'required-yes' : 'required-no';
  }
}
