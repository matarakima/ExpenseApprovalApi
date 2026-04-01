import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { LoginService } from './login.service';
import { AuthService } from './auth.service';

describe('LoginService', () => {
  let service: LoginService;
  let httpMock: HttpTestingController;
  let authServiceSpy: jest.Mocked<AuthService>;

  beforeEach(() => {
    authServiceSpy = {
      setSession: jest.fn()
    } as unknown as jest.Mocked<AuthService>;

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        LoginService,
        { provide: AuthService, useValue: authServiceSpy }
      ]
    });

    service = TestBed.inject(LoginService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('login sends POST and stores token', () => {
    const mockResponse = { userId: 'u1', accessToken: 'abc', tokenType: 'Bearer', expiresIn: 3600 };

    service.login({ email: 'a@b.com', password: '123456' }).subscribe((res) => {
      expect(res.accessToken).toBe('abc');
    });

    const req = httpMock.expectOne((r) => r.url.includes('/auth/login'));
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'a@b.com', password: '123456' });
    req.flush(mockResponse);

    expect(authServiceSpy.setSession).toHaveBeenCalledWith('abc', 'u1');
  });

  it('login propagates HTTP error', () => {
    service.login({ email: 'a@b.com', password: 'bad' }).subscribe({
      error: (err) => {
        expect(err.status).toBe(401);
      }
    });

    const req = httpMock.expectOne((r) => r.url.includes('/auth/login'));
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });
  });
});
