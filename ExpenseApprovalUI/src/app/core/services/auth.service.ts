import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'access_token';
  private loggedIn$ = new BehaviorSubject<boolean>(this.hasToken());

  isLoggedIn$ = this.loggedIn$.asObservable();

  constructor(private router: Router) {}

  get token(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  get isAuthenticated(): boolean {
    return !!this.token;
  }

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    this.loggedIn$.next(true);
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.loggedIn$.next(false);
    this.router.navigate(['/login']);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.TOKEN_KEY);
  }
}
