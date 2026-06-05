import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ShipmentDecision } from '../models/shipment-decision.model';
import { Batch } from '../models/batch.model';

@Injectable({
  providedIn: 'root',
})
export class ShipmentDecisionService {
  private readonly apiUrl = 'https://localhost:7003/api/shipment-decisions';

  constructor(private http: HttpClient) {}

  getDecisions(): Observable<ShipmentDecision[]> {
    return this.http.get<ShipmentDecision[]>(this.apiUrl);
  }

  allowShipment(batchId: number, userId: number): Observable<ShipmentDecision> {
    return this.http.post<ShipmentDecision>(
      `${this.apiUrl}/batch/${batchId}/allow?userId=${userId}`,
      {},
    );
  }

  getBatchesAllowedForShipment(): Observable<Batch[]> {
    return this.http.get<Batch[]>(`${this.apiUrl}/allowed-batches`);
  }

  denyShipment(batchId: number, userId: number): Observable<ShipmentDecision> {
    return this.http.post<ShipmentDecision>(
      `${this.apiUrl}/batch/${batchId}/deny?userId=${userId}`,
      {},
    );
  }
}
