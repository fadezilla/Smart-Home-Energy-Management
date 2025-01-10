import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { Observable } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

export interface DecodedToken {
    sub: string;  // user id
    email: string;
    role: string;
    // etc
  }
  
@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  signUp(email: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/user/signup`, { email, password });
  }
  

  login(email: string, password: string): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.apiUrl}/api/user/login`, { email, password });
  }

  // Store token in local storage for demo ( store the token in NgRx at a later stage)
  setToken(token: string) {
    localStorage.setItem('token', token);
  
    const decoded = jwtDecode<DecodedToken>(token);
    localStorage.setItem('role', decoded.role);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  removeToken() {
    localStorage.removeItem('token');
  }
}
