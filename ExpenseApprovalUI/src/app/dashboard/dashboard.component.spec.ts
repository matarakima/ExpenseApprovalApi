import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { ExpenseService } from '../core/services/expense.service';
import { of, throwError } from 'rxjs';
import { ExpenseMetrics } from '../core/models';

describe('DashboardComponent', () => {
  let expenseServiceSpy: jest.Mocked<ExpenseService>;

  beforeEach(async () => {
    expenseServiceSpy = {
      getMetrics: jest.fn()
    } as unknown as jest.Mocked<ExpenseService>;
  });

  function createComponent() {
    TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        { provide: ExpenseService, useValue: expenseServiceSpy }
      ]
    });

    const fixture = TestBed.createComponent(DashboardComponent);
    return fixture;
  }

  it('should create', () => {
    expenseServiceSpy.getMetrics.mockReturnValue(of({ totalRequests: 0, approvedCount: 0, rejectedCount: 0, pendingCount: 0, totalApprovedAmount: 0 }));
    const fixture = createComponent();
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('loads metrics on init', () => {
    const metrics: ExpenseMetrics = { totalRequests: 10, approvedCount: 5, rejectedCount: 2, pendingCount: 3, totalApprovedAmount: 500 };
    expenseServiceSpy.getMetrics.mockReturnValue(of(metrics));

    const fixture = createComponent();
    fixture.detectChanges();

    const comp = fixture.componentInstance;
    expect(comp.metrics).toEqual(metrics);
    expect(comp.loading).toBe(false);
    expect(comp.error).toBe('');
  });

  it('sets error on metrics failure', () => {
    expenseServiceSpy.getMetrics.mockReturnValue(throwError(() => new Error('fail')));

    const fixture = createComponent();
    fixture.detectChanges();

    const comp = fixture.componentInstance;
    expect(comp.error).toBe('Failed to load metrics.');
    expect(comp.loading).toBe(false);
    expect(comp.metrics).toBeNull();
  });
});
