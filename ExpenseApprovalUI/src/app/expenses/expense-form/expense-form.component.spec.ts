import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter, ActivatedRoute, Router } from '@angular/router';
import { ExpenseFormComponent } from './expense-form.component';
import { ExpenseService } from '../../core/services/expense.service';
import { NotificationService } from '../../core/services/notification.service';
import { of, throwError } from 'rxjs';

describe('ExpenseFormComponent - Create Mode', () => {
  let expenseServiceSpy: jest.Mocked<ExpenseService>;
  let notificationSpy: jest.Mocked<NotificationService>;
  let router: Router;

  beforeEach(async () => {
    expenseServiceSpy = {
      create: jest.fn().mockReturnValue(of({ id: 'new1' })),
      update: jest.fn(),
      getById: jest.fn()
    } as unknown as jest.Mocked<ExpenseService>;

    notificationSpy = {
      success: jest.fn(),
      error: jest.fn()
    } as unknown as jest.Mocked<NotificationService>;

    await TestBed.configureTestingModule({
      imports: [ExpenseFormComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        { provide: ExpenseService, useValue: expenseServiceSpy },
        { provide: NotificationService, useValue: notificationSpy },
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { paramMap: { get: () => null } } }
        }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    jest.spyOn(router, 'navigate').mockResolvedValue(true);
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(ExpenseFormComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('initializes in create mode with no id', () => {
    const fixture = TestBed.createComponent(ExpenseFormComponent);
    fixture.detectChanges();
    const comp = fixture.componentInstance;
    expect(comp.isEdit).toBe(false);
    expect(comp.pageLoading).toBe(false);
  });

  it('form is invalid when empty', () => {
    const fixture = TestBed.createComponent(ExpenseFormComponent);
    fixture.detectChanges();
    expect(fixture.componentInstance.form.valid).toBe(false);
  });

  it('form is valid with all fields', () => {
    const fixture = TestBed.createComponent(ExpenseFormComponent);
    fixture.detectChanges();
    const comp = fixture.componentInstance;
    comp.form.patchValue({
      description: 'Test expense',
      amount: 100,
      expenseDate: '2025-06-01',
      categoryId: 'cat1'
    });
    expect(comp.form.valid).toBe(true);
  });

  it('onSubmit does nothing when form is invalid', () => {
    const fixture = TestBed.createComponent(ExpenseFormComponent);
    fixture.detectChanges();
    fixture.componentInstance.onSubmit();
    expect(expenseServiceSpy.create).not.toHaveBeenCalled();
  });

  it('onSubmit calls create and navigates on success', () => {
    const fixture = TestBed.createComponent(ExpenseFormComponent);
    fixture.detectChanges();
    const comp = fixture.componentInstance;
    comp.form.patchValue({
      description: 'Test', amount: 50, expenseDate: '2025-06-01', categoryId: 'c1'
    });
    comp.onSubmit();

    expect(expenseServiceSpy.create).toHaveBeenCalled();
    expect(notificationSpy.success).toHaveBeenCalledWith('Expense created successfully.');
    expect(router.navigate).toHaveBeenCalledWith(['/expenses', 'new1']);
  });

  it('onSubmit handles create error', () => {
    expenseServiceSpy.create.mockReturnValue(throwError(() => new Error('fail')));

    const fixture = TestBed.createComponent(ExpenseFormComponent);
    fixture.detectChanges();
    const comp = fixture.componentInstance;
    comp.form.patchValue({
      description: 'Test', amount: 50, expenseDate: '2025-06-01', categoryId: 'c1'
    });
    comp.onSubmit();

    expect(comp.loading).toBe(false);
  });
});

describe('ExpenseFormComponent - Edit Mode', () => {
  let expenseServiceSpy: jest.Mocked<ExpenseService>;
  let notificationSpy: jest.Mocked<NotificationService>;
  let router: Router;

  beforeEach(async () => {
    expenseServiceSpy = {
      getById: jest.fn().mockReturnValue(of({
        id: 'e1', category: 'Travel', description: 'Flight', amount: 200,
        expenseDate: '2025-01-01T00:00:00', requestedBy: 'U', status: 'Pending',
        createdAt: '2025-01-01', decisionDate: null, decisionBy: null
      })),
      update: jest.fn().mockReturnValue(of({ id: 'e1' })),
      create: jest.fn()
    } as unknown as jest.Mocked<ExpenseService>;

    notificationSpy = {
      success: jest.fn(),
      error: jest.fn()
    } as unknown as jest.Mocked<NotificationService>;

    await TestBed.configureTestingModule({
      imports: [ExpenseFormComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        { provide: ExpenseService, useValue: expenseServiceSpy },
        { provide: NotificationService, useValue: notificationSpy },
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { paramMap: { get: () => 'e1' } } }
        }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    jest.spyOn(router, 'navigate').mockResolvedValue(true);
  });

  it('initializes in edit mode', () => {
    const fixture = TestBed.createComponent(ExpenseFormComponent);
    fixture.detectChanges();
    const comp = fixture.componentInstance;
    expect(comp.isEdit).toBe(true);
    expect(comp.expenseId).toBe('e1');
  });

  it('populates form with fetched data', () => {
    const fixture = TestBed.createComponent(ExpenseFormComponent);
    fixture.detectChanges();
    const comp = fixture.componentInstance;
    expect(comp.form.get('description')?.value).toBe('Flight');
    expect(comp.form.get('amount')?.value).toBe(200);
  });

  it('onSubmit calls update in edit mode', () => {
    const fixture = TestBed.createComponent(ExpenseFormComponent);
    fixture.detectChanges();
    const comp = fixture.componentInstance;
    comp.onSubmit();

    expect(expenseServiceSpy.update).toHaveBeenCalledWith('e1', expect.any(Object));
    expect(notificationSpy.success).toHaveBeenCalledWith('Expense updated successfully.');
  });

  it('redirects if expense is not Pending', () => {
    expenseServiceSpy.getById.mockReturnValue(of({
      id: 'e1', category: 'Travel', description: 'X', amount: 100,
      expenseDate: '2025-01-01', requestedBy: 'U', status: 'Approved',
      createdAt: '2025-01-01', decisionDate: '2025-01-02', decisionBy: 'M'
    }));

    const fixture = TestBed.createComponent(ExpenseFormComponent);
    fixture.detectChanges();

    expect(notificationSpy.error).toHaveBeenCalledWith('Only pending expenses can be edited.');
    expect(router.navigate).toHaveBeenCalledWith(['/expenses', 'e1']);
  });
});
