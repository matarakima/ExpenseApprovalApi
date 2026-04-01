import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from '../models';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class LoginService {
  private readonly url = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.url}/login`, request).pipe(
      tap(response => this.authService.setSession(response.accessToken, response.userId))
    );
  }
}
