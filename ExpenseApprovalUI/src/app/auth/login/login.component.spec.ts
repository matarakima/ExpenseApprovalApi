import { TestBed } from '@angular/core/testing';
import { provideHttpClient, HttpErrorResponse } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter, Router } from '@angular/router';
import { LoginComponent } from './login.component';
import { LoginService } from '../../core/services/login.service';
import { of, throwError } from 'rxjs';
import { fakeAsync, tick } from '@angular/core/testing';

describe('LoginComponent', () => {
  let loginServiceSpy: jest.Mocked<LoginService>;
  let router: Router;

  beforeEach(async () => {
    loginServiceSpy = {
      login: jest.fn()
    } as unknown as jest.Mocked<LoginService>;

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        { provide: LoginService, useValue: loginServiceSpy }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    jest.spyOn(router, 'navigate').mockResolvedValue(true);
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should initialize form with empty email and password', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const comp = fixture.componentInstance;
    expect(comp.loginForm.get('email')?.value).toBe('');
    expect(comp.loginForm.get('password')?.value).toBe('');
  });

  it('form should be invalid when empty', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    expect(fixture.componentInstance.loginForm.valid).toBe(false);
  });

  it('form should be invalid with bad email', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const comp = fixture.componentInstance;
    comp.loginForm.patchValue({ email: 'not-email', password: '123456' });
    expect(comp.loginForm.valid).toBe(false);
  });

  it('form should be invalid with short password', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const comp = fixture.componentInstance;
    comp.loginForm.patchValue({ email: 'a@b.com', password: '123' });
    expect(comp.loginForm.valid).toBe(false);
  });

  it('form should be valid with correct data', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const comp = fixture.componentInstance;
    comp.loginForm.patchValue({ email: 'user@test.com', password: '123456' });
    expect(comp.loginForm.valid).toBe(true);
  });

  it('onSubmit does nothing when form is invalid', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const comp = fixture.componentInstance;
    comp.onSubmit();
    expect(loginServiceSpy.login).not.toHaveBeenCalled();
  });

  it('onSubmit calls login and navigates on success', () => {
    loginServiceSpy.login.mockReturnValue(of({ accessToken: 't', tokenType: 'Bearer', expiresIn: 3600 }));

    const fixture = TestBed.createComponent(LoginComponent);
    const comp = fixture.componentInstance;
    comp.loginForm.patchValue({ email: 'a@b.com', password: '123456' });
    comp.onSubmit();

    expect(loginServiceSpy.login).toHaveBeenCalledWith({ email: 'a@b.com', password: '123456' });
    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('onSubmit sets errorMessage on 401 error', fakeAsync(() => {
    loginServiceSpy.login.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 401, statusText: 'Unauthorized' }))
    );

    const fixture = TestBed.createComponent(LoginComponent);
    const comp = fixture.componentInstance;
    comp.loginForm.patchValue({ email: 'a@b.com', password: 'wrongpw' });
    comp.onSubmit();
    tick();

    expect(comp.errorMessage).toBe('Invalid email or password.');
    expect(comp.loading).toBe(false);
  }));

  it('onSubmit sets generic errorMessage on other errors', () => {
    loginServiceSpy.login.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 500, statusText: 'Server Error' }))
    );

    const fixture = TestBed.createComponent(LoginComponent);
    const comp = fixture.componentInstance;
    comp.loginForm.patchValue({ email: 'a@b.com', password: '123456' });
    comp.onSubmit();

    expect(comp.errorMessage).toBe('An error occurred. Please try again.');
  });
});
