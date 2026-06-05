import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { QualityParameter } from '../models/quality-parameter.models';
import { CreateQualityParameterDto } from '../models/requests/create-quality-parameter.dto';

@Injectable({
  providedIn: 'root',
})
export class QualityParameterService {
  private readonly apiUrl = 'https://localhost:7003/api/quality-parameters';

  constructor(private http: HttpClient) {}

  getQualityParameters(): Observable<QualityParameter[]> {
    return this.http.get<QualityParameter[]>(this.apiUrl);
  }

  createQualityParameter(request: CreateQualityParameterDto): Observable<QualityParameter> {
    return this.http.post<QualityParameter>(this.apiUrl, request);
  }
}
