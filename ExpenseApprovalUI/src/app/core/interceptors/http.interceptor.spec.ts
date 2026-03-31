import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient, withInterceptors, HttpClient, HttpErrorResponse } from '@angular/common/http';
import { authInterceptor, errorInterceptor } from './http.interceptor';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

describe('authInterceptor', () => {
  let httpMock: HttpTestingController;
  let http: HttpClient;
  let authService: AuthService;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        AuthService,
        NotificationService
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    authService = TestBed.inject(AuthService);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('adds Authorization header when token exists', () => {
    authService.setToken('my-token');

    http.get('/api/test').subscribe();

    const req = httpMock.expectOne('/api/test');
    expect(req.request.headers.get('Authorization')).toBe('Bearer my-token');
    req.flush({});
  });

  it('does not add Authorization header when no token', () => {
    http.get('/api/test').subscribe();

    const req = httpMock.expectOne('/api/test');
    expect(req.request.headers.has('Authorization')).toBe(false);
    req.flush({});
  });
});

describe('errorInterceptor', () => {
  let httpMock: HttpTestingController;
  let http: HttpClient;
  let notificationService: NotificationService;
  let authService: AuthService;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
        AuthService,
        NotificationService
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    notificationService = TestBed.inject(NotificationService);
    authService = TestBed.inject(AuthService);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('shows error notification on 400', () => {
    const errorSpy = jest.spyOn(notificationService, 'error');

    http.get('/api/test').subscribe({ error: () => {} });

    const req = httpMock.expectOne('/api/test');
    req.flush({ message: 'Bad input' }, { status: 400, statusText: 'Bad Request' });

    expect(errorSpy).toHaveBeenCalledWith('Bad input');
  });

  it('shows default message on 400 without body message', () => {
    const errorSpy = jest.spyOn(notificationService, 'error');

    http.get('/api/test').subscribe({ error: () => {} });

    const req = httpMock.expectOne('/api/test');
    req.flush(null, { status: 400, statusText: 'Bad Request' });

    expect(errorSpy).toHaveBeenCalledWith('Invalid request data.');
  });

  it('calls logout on 401', () => {
    const logoutSpy = jest.spyOn(authService, 'logout');

    http.get('/api/test').subscribe({ error: () => {} });

    const req = httpMock.expectOne('/api/test');
    req.flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(logoutSpy).toHaveBeenCalled();
  });

  it('shows forbidden message on 403', () => {
    const errorSpy = jest.spyOn(notificationService, 'error');

    http.get('/api/test').subscribe({ error: () => {} });

    const req = httpMock.expectOne('/api/test');
    req.flush(null, { status: 403, statusText: 'Forbidden' });

    expect(errorSpy).toHaveBeenCalledWith('You do not have permission to perform this action.');
  });

  it('shows not found message on 404', () => {
    const errorSpy = jest.spyOn(notificationService, 'error');

    http.get('/api/test').subscribe({ error: () => {} });

    const req = httpMock.expectOne('/api/test');
    req.flush(null, { status: 404, statusText: 'Not Found' });

    expect(errorSpy).toHaveBeenCalledWith('The requested resource was not found.');
  });

  it('shows server error on 500', () => {
    const errorSpy = jest.spyOn(notificationService, 'error');

    http.get('/api/test').subscribe({ error: () => {} });

    const req = httpMock.expectOne('/api/test');
    req.flush(null, { status: 500, statusText: 'Server Error' });

    expect(errorSpy).toHaveBeenCalledWith('Internal server error. Please try again later.');
  });
});
