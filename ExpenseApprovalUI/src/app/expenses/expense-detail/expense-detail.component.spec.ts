import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter, ActivatedRoute, Router } from '@angular/router';
import { ExpenseDetailComponent } from './expense-detail.component';
import { ExpenseService } from '../../core/services/expense.service';
import { NotificationService } from '../../core/services/notification.service';
import { of, throwError } from 'rxjs';
import { ExpenseRequest } from '../../core/models';

describe('ExpenseDetailComponent', () => {
  let expenseServiceSpy: jest.Mocked<ExpenseService>;
  let notificationSpy: jest.Mocked<NotificationService>;
  let router: Router;

  const mockExpense: ExpenseRequest = {
    id: 'e1', category: 'Travel', description: 'Flight', amount: 200,
    expenseDate: '2025-01-01', requestedById: 'User', status: 'Pending',
    createdAt: '2025-01-01', decisionDate: null, decisionById: null
  };

  function setup(expense: ExpenseRequest | null = mockExpense, paramId = 'e1') {
    expenseServiceSpy = {
      getById: jest.fn().mockReturnValue(expense ? of(expense) : throwError(() => ({ status: 404 }))),
      approve: jest.fn().mockReturnValue(of({ ...mockExpense, status: 'Approved' })),
      reject: jest.fn().mockReturnValue(of({ ...mockExpense, status: 'Rejected' }))
    } as unknown as jest.Mocked<ExpenseService>;

    notificationSpy = {
      success: jest.fn(),
      error: jest.fn()
    } as unknown as jest.Mocked<NotificationService>;

    TestBed.configureTestingModule({
      imports: [ExpenseDetailComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        { provide: ExpenseService, useValue: expenseServiceSpy },
        { provide: NotificationService, useValue: notificationSpy },
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { paramMap: { get: () => paramId } } }
        }
      ]
    });

    router = TestBed.inject(Router);
    jest.spyOn(router, 'navigate').mockResolvedValue(true);

    return TestBed.createComponent(ExpenseDetailComponent);
  }

  it('should create', () => {
    const fixture = setup();
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('loads expense on init', () => {
    const fixture = setup();
    fixture.detectChanges();

    const comp = fixture.componentInstance;
    expect(comp.expense).toEqual(mockExpense);
    expect(comp.loading).toBe(false);
  });

  it('navigates to /expenses on load error', () => {
    const fixture = setup(null);
    fixture.detectChanges();

    expect(router.navigate).toHaveBeenCalledWith(['/expenses']);
  });

  it('approve calls service and updates status', () => {
    const fixture = setup();
    fixture.detectChanges();

    fixture.componentInstance.approve();

    expect(expenseServiceSpy.approve).toHaveBeenCalledWith('e1');
    expect(notificationSpy.success).toHaveBeenCalledWith('Expense approved successfully.');
    expect(fixture.componentInstance.expense!.status).toBe('Approved');
  });

  it('reject calls service and updates status', () => {
    const fixture = setup();
    fixture.detectChanges();

    fixture.componentInstance.reject();

    expect(expenseServiceSpy.reject).toHaveBeenCalledWith('e1');
    expect(notificationSpy.success).toHaveBeenCalledWith('Expense rejected successfully.');
    expect(fixture.componentInstance.expense!.status).toBe('Rejected');
  });

  it('approve does nothing when expense is null', () => {
    const fixture = setup();
    fixture.detectChanges();
    fixture.componentInstance.expense = null;

    fixture.componentInstance.approve();
    expect(expenseServiceSpy.approve).not.toHaveBeenCalled();
  });

  it('reject does nothing when expense is null', () => {
    const fixture = setup();
    fixture.detectChanges();
    fixture.componentInstance.expense = null;

    fixture.componentInstance.reject();
    expect(expenseServiceSpy.reject).not.toHaveBeenCalled();
  });

  it('getStatusClass returns correct class', () => {
    const fixture = setup();
    const comp = fixture.componentInstance;
    expect(comp.getStatusClass('Approved')).toBe('status-approved');
    expect(comp.getStatusClass('Rejected')).toBe('status-rejected');
    expect(comp.getStatusClass('Pending')).toBe('status-pending');
    expect(comp.getStatusClass('Unknown')).toBe('');
  });

  it('handles approve error', () => {
    expenseServiceSpy.approve = jest.fn().mockReturnValue(throwError(() => new Error('fail')));

    const fixture = setup();
    fixture.detectChanges();
    fixture.componentInstance.approve();

    expect(fixture.componentInstance.actionLoading).toBe(false);
  });
});
