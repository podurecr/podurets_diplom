import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { LoginResponse } from '../models/auth/login-response.model';
import { User } from '../models/user.model';
import { LoginRequest } from '../models/auth/login-request.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:7003/api/auth';

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request)
      .pipe(
        tap(response => {
          localStorage.setItem('token', response.token);
          localStorage.setItem('tokenExpiresAt', response.tokenExpiresAt.toString());
          localStorage.setItem('user', JSON.stringify(response.user));
          localStorage.setItem('userId', response.user.id.toString());
          localStorage.setItem('roleName', response.user.roleName);
        })
      );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('tokenExpiresAt');
    localStorage.removeItem('user');
    localStorage.removeItem('userId');
    localStorage.removeItem('roleName');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getCurrentUser(): User | null {
    const userJson = localStorage.getItem('user');

    if (!userJson) {
      return null;
    }

    return JSON.parse(userJson) as User;
  }

getRole(): string | null {
  const roleFromStorage = localStorage.getItem('roleName');

  if (roleFromStorage && roleFromStorage !== 'undefined' && roleFromStorage !== 'null') {
    return roleFromStorage;
  }

  const user = this.getCurrentUser();

  return user?.roleName ?? user?.role?.name ?? null;
}

isLoggedIn(): boolean {
  return !!this.getCurrentUser() || !!this.getToken();
}

hasRole(roles: string[]): boolean {
  const role = this.getRole();

  console.log('ROLE FROM AUTH SERVICE:', role);
  console.log('ALLOWED ROLES:', roles);

  if (!role) {
    return false;
  }

  return roles.includes(role);
}
}