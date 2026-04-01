import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'access_token';
  private readonly USER_ID_KEY = 'user_id';
  private loggedIn$ = new BehaviorSubject<boolean>(this.hasToken());

  isLoggedIn$ = this.loggedIn$.asObservable();

  constructor(private router: Router) {}

  get token(): string | null {
    return sessionStorage.getItem(this.TOKEN_KEY);
  }

  get userId(): string | null {
    return sessionStorage.getItem(this.USER_ID_KEY);
  }

  get isAuthenticated(): boolean {
    return !!this.token;
  }

  setSession(token: string, userId: string): void {
    sessionStorage.setItem(this.TOKEN_KEY, token);
    sessionStorage.setItem(this.USER_ID_KEY, userId);
    this.loggedIn$.next(true);
  }

  logout(): void {
    sessionStorage.removeItem(this.TOKEN_KEY);
    sessionStorage.removeItem(this.USER_ID_KEY);
    this.loggedIn$.next(false);
    this.router.navigate(['/login']);
  }

  private hasToken(): boolean {
    return !!sessionStorage.getItem(this.TOKEN_KEY);
  }
}
