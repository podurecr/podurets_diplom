import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CreateQualityAssessmentRequest {
  batchId: number;
  isApproved: boolean;
  conclusion: string;
}

export interface QualityAssessmentRequest {
  batchId: number;
  isApproved: boolean;
  conclusion: string;
}

export interface QualityAssessmentDto {
  id: number;
  batchId: number;
  isApproved: boolean;
  conclusion: string;
  assessedAt: string;
  assessedByUserId: number;
  isFinal: boolean;
  finalizedAt?: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class QualityAssessmentService {
  private readonly apiUrl = 'https://localhost:7003/api/quality-assessment';

  constructor(private http: HttpClient) {}

  assessBatch(
    batchId: number,
    request: CreateQualityAssessmentRequest,
    userId: number,
  ): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/get-batch/${batchId}?userId=${userId}`, request);
  }

  getByBatchId(batchId: number): Observable<QualityAssessmentDto> {
    return this.http.get<QualityAssessmentDto>(`${this.apiUrl}/batch/${batchId}`);
  }

  saveAssessment(
    batchId: number,
    request: QualityAssessmentRequest,
    userId: number,
  ): Observable<QualityAssessmentDto> {
    return this.http.post<QualityAssessmentDto>(
      `${this.apiUrl}/batch/${batchId}/save?userId=${userId}`,
      request,
    );
  }

  finalizeAssessment(
    batchId: number,
    request: QualityAssessmentRequest,
    userId: number,
  ): Observable<QualityAssessmentDto> {
    return this.http.post<QualityAssessmentDto>(
      `${this.apiUrl}/batch/${batchId}/finalize?userId=${userId}`,
      request,
    );
  }
}
