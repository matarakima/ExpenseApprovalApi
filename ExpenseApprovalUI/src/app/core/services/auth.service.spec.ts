import { AuthService } from './auth.service';
import { Router } from '@angular/router';

describe('AuthService', () => {
  let service: AuthService;
  let routerSpy: jest.Mocked<Router>;

  beforeEach(() => {
    routerSpy = { navigate: jest.fn() } as unknown as jest.Mocked<Router>;
    sessionStorage.clear();
    service = new AuthService(routerSpy);
  });

  afterEach(() => sessionStorage.clear());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('isAuthenticated returns false when no token', () => {
    expect(service.isAuthenticated).toBe(false);
  });

  it('isAuthenticated returns true after setSession', () => {
    service.setSession('test-token', 'user-1');
    expect(service.isAuthenticated).toBe(true);
  });

  it('token returns the stored token', () => {
    service.setSession('my-token', 'user-1');
    expect(service.token).toBe('my-token');
  });

  it('userId returns the stored userId', () => {
    service.setSession('my-token', 'user-1');
    expect(service.userId).toBe('user-1');
  });

  it('setSession emits true on isLoggedIn$', (done) => {
    service.isLoggedIn$.subscribe((val) => {
      if (val) {
        expect(val).toBe(true);
        done();
      }
    });
    service.setSession('t', 'u');
  });

  it('logout removes token and userId and navigates to /login', () => {
    service.setSession('t', 'u');
    service.logout();
    expect(service.token).toBeNull();
    expect(service.userId).toBeNull();
    expect(service.isAuthenticated).toBe(false);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('logout emits false on isLoggedIn$', (done) => {
    service.setSession('t', 'u');
    const values: boolean[] = [];
    service.isLoggedIn$.subscribe((v) => values.push(v));
    service.logout();
    setTimeout(() => {
      expect(values).toContain(false);
      done();
    }, 0);
  });
});
