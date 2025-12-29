import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;          
  role?: string;
  username?: string;
}


const TOKEN_KEY = 'access_token';
const ROLE_KEY = 'user_role';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private http: HttpClient) {}

  login(payload: LoginRequest) {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/api/auth/login`, payload)
      .pipe(
        tap((res) => {
          if (!res?.token) {
            localStorage.removeItem(TOKEN_KEY);
            throw new Error('Login sin token');
          }

          localStorage.setItem(TOKEN_KEY, res.token);

          if (res.role) localStorage.setItem(ROLE_KEY, res.role);
          else localStorage.removeItem(ROLE_KEY);
        })
      );
  }

  logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(ROLE_KEY);
  }

  getToken(): string | null {
  const t = localStorage.getItem(TOKEN_KEY);
  if (!t || t === 'undefined' || t === 'null') return null;
  return t;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }


  getRole(): string | null {
    return localStorage.getItem(ROLE_KEY);
  }

  
}
