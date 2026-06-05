import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AnalysisResult } from '../models/analysis-result.model';
import { SaveBatchAnalysisRequest } from '../save-batch-analysis.dto';

@Injectable({
  providedIn: 'root',
})
export class AnalysisResultService {
  private readonly apiUrl = 'https://localhost:7003/api/analysis-result';

  constructor(private http: HttpClient) {}

  getAnalysisResults(): Observable<AnalysisResult[]> {
    return this.http.get<AnalysisResult[]>(this.apiUrl);
  }

  getResultsByBatchId(batchId: number): Observable<AnalysisResult[]> {
    return this.http.get<AnalysisResult[]>(`${this.apiUrl}/batch/${batchId}`);
  }

  completeBatchAnalysis(batchId: number, request: SaveBatchAnalysisRequest): Observable<void> {
    const userId = Number(localStorage.getItem('userId'));

    return this.http.post<void>(
      `${this.apiUrl}/batch/${batchId}/complete?userId=${userId}`,
      request,
    );
  }

  finishBatchAnalysis(batchId: number): Observable<void> {
    const userId = Number(localStorage.getItem('userId'));

    console.log(batchId);
    console.log(userId);

    return this.http.post<void>(`${this.apiUrl}/batch/${batchId}/finish?userId=${userId}`, {});
  }
}
