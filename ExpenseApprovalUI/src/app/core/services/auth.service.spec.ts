import { AuthService } from './auth.service';
import { Router } from '@angular/router';

describe('AuthService', () => {
  let service: AuthService;
  let routerSpy: jest.Mocked<Router>;

  beforeEach(() => {
    routerSpy = { navigate: jest.fn() } as unknown as jest.Mocked<Router>;
    localStorage.clear();
    service = new AuthService(routerSpy);
  });

  afterEach(() => localStorage.clear());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('isAuthenticated returns false when no token', () => {
    expect(service.isAuthenticated).toBe(false);
  });

  it('isAuthenticated returns true after setToken', () => {
    service.setToken('test-token');
    expect(service.isAuthenticated).toBe(true);
  });

  it('token returns the stored token', () => {
    service.setToken('my-token');
    expect(service.token).toBe('my-token');
  });

  it('setToken emits true on isLoggedIn$', (done) => {
    service.isLoggedIn$.subscribe((val) => {
      if (val) {
        expect(val).toBe(true);
        done();
      }
    });
    service.setToken('t');
  });

  it('logout removes token and navigates to /login', () => {
    service.setToken('t');
    service.logout();
    expect(service.token).toBeNull();
    expect(service.isAuthenticated).toBe(false);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('logout emits false on isLoggedIn$', (done) => {
    service.setToken('t');
    const values: boolean[] = [];
    service.isLoggedIn$.subscribe((v) => values.push(v));
    service.logout();
    setTimeout(() => {
      expect(values).toContain(false);
      done();
    }, 0);
  });
});
