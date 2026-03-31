import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { LayoutComponent } from './layout.component';
import { AuthService } from '../../core/services/auth.service';

describe('LayoutComponent', () => {
  let authServiceSpy: jest.Mocked<AuthService>;

  beforeEach(async () => {
    authServiceSpy = {
      logout: jest.fn(),
      isAuthenticated: true
    } as unknown as jest.Mocked<AuthService>;

    await TestBed.configureTestingModule({
      imports: [LayoutComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(LayoutComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('menuOpen starts as false', () => {
    const fixture = TestBed.createComponent(LayoutComponent);
    expect(fixture.componentInstance.menuOpen).toBe(false);
  });

  it('toggleMenu flips menuOpen', () => {
    const fixture = TestBed.createComponent(LayoutComponent);
    const comp = fixture.componentInstance;
    comp.toggleMenu();
    expect(comp.menuOpen).toBe(true);
    comp.toggleMenu();
    expect(comp.menuOpen).toBe(false);
  });

  it('logout calls authService.logout', () => {
    const fixture = TestBed.createComponent(LayoutComponent);
    fixture.componentInstance.logout();
    expect(authServiceSpy.logout).toHaveBeenCalled();
  });
});
