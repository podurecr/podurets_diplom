import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Batch } from '../models/batch.model';
import { CreateBatchRequest } from '../models/requests/create-batch-request';

@Injectable({
  providedIn: 'root',
})
export class BatchService {
  private readonly apiUrl = 'https://localhost:7003/api/batches';

  constructor(private http: HttpClient) {}

  getBatches(): Observable<Batch[]> {
    return this.http.get<Batch[]>(this.apiUrl);
  }

  getBatchById(id: number): Observable<Batch> {
    return this.http.get<Batch>(`${this.apiUrl}/${id}`);
  }

  createBatch(request: any): Observable<Batch> {
    console.log('CREATE BATCH REQUEST:', request);
    return this.http.post<Batch>(this.apiUrl, request);
  }
}
