import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { ExpenseListComponent } from './expense-list.component';
import { ExpenseService } from '../../core/services/expense.service';
import { of, throwError } from 'rxjs';
import { ExpenseRequest } from '../../core/models';

describe('ExpenseListComponent', () => {
  let expenseServiceSpy: jest.Mocked<ExpenseService>;

  const mockExpenses: ExpenseRequest[] = [
    { id: '1', category: 'Travel', description: 'Flight', amount: 200, expenseDate: '2025-01-01', requestedBy: 'U', status: 'Pending', createdAt: '2025-01-01', decisionDate: null, decisionBy: null },
    { id: '2', category: 'Food', description: 'Lunch', amount: 15, expenseDate: '2025-01-02', requestedBy: 'U', status: 'Approved', createdAt: '2025-01-02', decisionDate: '2025-01-03', decisionBy: 'M' }
  ];

  beforeEach(async () => {
    expenseServiceSpy = {
      getAll: jest.fn().mockReturnValue(of(mockExpenses)),
      filter: jest.fn().mockReturnValue(of([mockExpenses[0]]))
    } as unknown as jest.Mocked<ExpenseService>;

    await TestBed.configureTestingModule({
      imports: [ExpenseListComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        { provide: ExpenseService, useValue: expenseServiceSpy }
      ]
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(ExpenseListComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('loads all expenses on init', () => {
    const fixture = TestBed.createComponent(ExpenseListComponent);
    fixture.detectChanges();

    const comp = fixture.componentInstance;
    expect(expenseServiceSpy.getAll).toHaveBeenCalled();
    expect(comp.expenses).toEqual(mockExpenses);
    expect(comp.loading).toBe(false);
  });

  it('applies filters when set', () => {
    const fixture = TestBed.createComponent(ExpenseListComponent);
    const comp = fixture.componentInstance;
    fixture.detectChanges();

    comp.filters = { status: 'Pending' };
    comp.applyFilters();

    expect(expenseServiceSpy.filter).toHaveBeenCalledWith({ status: 'Pending' });
  });

  it('clearFilters resets and reloads', () => {
    const fixture = TestBed.createComponent(ExpenseListComponent);
    const comp = fixture.componentInstance;
    fixture.detectChanges();

    comp.filters = { status: 'Rejected' };
    comp.clearFilters();

    expect(comp.filters).toEqual({});
    expect(expenseServiceSpy.getAll).toHaveBeenCalled();
  });

  it('getStatusClass returns correct class', () => {
    const fixture = TestBed.createComponent(ExpenseListComponent);
    const comp = fixture.componentInstance;

    expect(comp.getStatusClass('Approved')).toBe('status-approved');
    expect(comp.getStatusClass('Rejected')).toBe('status-rejected');
    expect(comp.getStatusClass('Pending')).toBe('status-pending');
    expect(comp.getStatusClass('Other')).toBe('');
  });

  it('handles error on load', () => {
    expenseServiceSpy.getAll.mockReturnValue(throwError(() => new Error('fail')));

    const fixture = TestBed.createComponent(ExpenseListComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.loading).toBe(false);
  });
});
