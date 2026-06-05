import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ProductQualitySpecification } from '../models/product-quality-specification.model';

@Injectable({
  providedIn: 'root',
})
export class ProductQualitySpecificationService {
  private readonly apiUrl = 'https://localhost:7003/api/quality-specifications';

  constructor(private http: HttpClient) {}

  getSpecifications(): Observable<ProductQualitySpecification[]> {
    return this.http.get<ProductQualitySpecification[]>(this.apiUrl);
  }

  getSpecificationById(id: number): Observable<ProductQualitySpecification> {
    return this.http.get<ProductQualitySpecification>(`${this.apiUrl}/${id}`);
  }

  getSpecificationsByProductId(productId: number): Observable<ProductQualitySpecification[]> {
    return this.http.get<ProductQualitySpecification[]>(`${this.apiUrl}/product/${productId}`);
  }
}
