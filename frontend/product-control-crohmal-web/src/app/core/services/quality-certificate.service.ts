import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { QualityCertificate } from '../models/quality-certificate.model';

@Injectable({
  providedIn: 'root',
})
export class QualityCertificateService {
  private readonly apiUrl = 'https://localhost:7003/api/quality-certificates';

  constructor(private http: HttpClient) {}

  getCertificates(): Observable<QualityCertificate[]> {
    return this.http.get<QualityCertificate[]>(this.apiUrl);
  }

  getCertificateById(id: number): Observable<QualityCertificate> {
    return this.http.get<QualityCertificate>(`${this.apiUrl}/${id}`);
  }

  getCertificateByBatchId(batchId: number): Observable<QualityCertificate> {
    return this.http.get<QualityCertificate>(`${this.apiUrl}/batch/${batchId}`);
  }

  generateCertificate(batchId: number, userId: number): Observable<QualityCertificate> {
    return this.http.post<QualityCertificate>(
      `${this.apiUrl}/batch/${batchId}/generate?userId=${userId}`,
      {},
    );
  }

  downloadPdf(id: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/pdf`, {
      responseType: 'blob',
    });
  }
}
