import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { authGuard, loginGuard } from './auth.guard';

describe('authGuard', () => {
  let authService: AuthService;
  let routerSpy: jest.Mocked<Router>;

  beforeEach(() => {
    routerSpy = { navigate: jest.fn() } as unknown as jest.Mocked<Router>;
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: routerSpy },
        AuthService
      ]
    });

    authService = TestBed.inject(AuthService);
  });

  afterEach(() => localStorage.clear());

  it('returns true when authenticated', () => {
    authService.setToken('t');
    const result = TestBed.runInInjectionContext(() =>
      authGuard({} as any, {} as any)
    );
    expect(result).toBe(true);
  });

  it('redirects to /login when not authenticated', () => {
    const result = TestBed.runInInjectionContext(() =>
      authGuard({} as any, {} as any)
    );
    expect(result).toBe(false);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });
});

describe('loginGuard', () => {
  let authService: AuthService;
  let routerSpy: jest.Mocked<Router>;

  beforeEach(() => {
    routerSpy = { navigate: jest.fn() } as unknown as jest.Mocked<Router>;
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: routerSpy },
        AuthService
      ]
    });

    authService = TestBed.inject(AuthService);
  });

  afterEach(() => localStorage.clear());

  it('returns true when NOT authenticated', () => {
    const result = TestBed.runInInjectionContext(() =>
      loginGuard({} as any, {} as any)
    );
    expect(result).toBe(true);
  });

  it('redirects to /dashboard when authenticated', () => {
    authService.setToken('t');
    const result = TestBed.runInInjectionContext(() =>
      loginGuard({} as any, {} as any)
    );
    expect(result).toBe(false);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/dashboard']);
  });
});
