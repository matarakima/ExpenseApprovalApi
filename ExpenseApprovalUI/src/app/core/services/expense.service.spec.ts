import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ExpenseService } from './expense.service';
import { AuthService } from './auth.service';

describe('ExpenseService', () => {
  let service: ExpenseService;
  let httpMock: HttpTestingController;
  const mockAuthService = { userId: 'user-123', token: 'tok', isAuthenticated: true };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        ExpenseService,
        { provide: AuthService, useValue: mockAuthService }
      ]
    });

    service = TestBed.inject(ExpenseService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getAll sends GET request', () => {
    const mockData = [{ id: '1', category: 'Travel', description: 'Trip', amount: 100, expenseDate: '2025-01-01', requestedBy: 'U', status: 'Pending', createdAt: '2025-01-01', decisionDate: null, decisionBy: null }];

    service.getAll().subscribe((data) => {
      expect(data).toEqual(mockData);
    });

    const req = httpMock.expectOne((r) => r.url.endsWith('/expenses'));
    expect(req.request.method).toBe('GET');
    req.flush(mockData);
  });

  it('getById sends GET with id', () => {
    const mock = { id: 'abc', category: 'Food', description: 'Lunch', amount: 15, expenseDate: '2025-02-01', requestedBy: 'U', status: 'Approved', createdAt: '2025-02-01', decisionDate: '2025-02-02', decisionBy: 'M' };

    service.getById('abc').subscribe((data) => {
      expect(data.id).toBe('abc');
    });

    const req = httpMock.expectOne((r) => r.url.endsWith('/expenses/abc'));
    expect(req.request.method).toBe('GET');
    req.flush(mock);
  });

  it('create sends POST with body', () => {
    const dto = { categoryId: 'c1', description: 'Test', amount: 50, expenseDate: '2025-03-01' };
    const mockResp = { id: 'new1', category: 'Cat', description: 'Test', amount: 50, expenseDate: '2025-03-01', requestedBy: 'U', status: 'Pending', createdAt: '2025-03-01', decisionDate: null, decisionBy: null };

    service.create(dto).subscribe((data) => {
      expect(data.id).toBe('new1');
    });

    const req = httpMock.expectOne((r) => r.url.endsWith('/expenses') && r.method === 'POST');
    expect(req.request.body).toEqual(dto);
    req.flush(mockResp);
  });

  it('update sends PUT with id and body', () => {
    const dto = { categoryId: 'c1', description: 'Updated', amount: 75, expenseDate: '2025-03-01' };

    service.update('x1', dto).subscribe();

    const req = httpMock.expectOne((r) => r.url.endsWith('/expenses/x1'));
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(dto);
    req.flush({});
  });

  it('approve sends PATCH with decisionById', () => {
    service.approve('x1').subscribe();

    const req = httpMock.expectOne((r) => r.url.includes('/expenses/x1/approve'));
    expect(req.request.method).toBe('PATCH');
    expect(req.request.params.get('decisionById')).toBe('user-123');
    req.flush({});
  });

  it('reject sends PATCH with decisionById', () => {
    service.reject('x1').subscribe();

    const req = httpMock.expectOne((r) => r.url.includes('/expenses/x1/reject'));
    expect(req.request.method).toBe('PATCH');
    expect(req.request.params.get('decisionById')).toBe('user-123');
    req.flush({});
  });

  it('filter sends GET with query params', () => {
    service.filter({ status: 'Pending', category: 'Travel' }).subscribe();

    const req = httpMock.expectOne((r) => r.url.includes('/expenses/filter'));
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('status')).toBe('Pending');
    expect(req.request.params.get('category')).toBe('Travel');
    req.flush([]);
  });

  it('filter omits empty params', () => {
    service.filter({ status: 'Approved' }).subscribe();

    const req = httpMock.expectOne((r) => r.url.includes('/expenses/filter'));
    expect(req.request.params.get('status')).toBe('Approved');
    expect(req.request.params.has('category')).toBe(false);
    expect(req.request.params.has('fromDate')).toBe(false);
    req.flush([]);
  });

  it('getMetrics sends GET', () => {
    const metrics = { totalRequests: 10, approvedCount: 5, rejectedCount: 2, pendingCount: 3, totalApprovedAmount: 500 };

    service.getMetrics().subscribe((data) => {
      expect(data.totalRequests).toBe(10);
    });

    const req = httpMock.expectOne((r) => r.url.endsWith('/expenses/metrics'));
    expect(req.request.method).toBe('GET');
    req.flush(metrics);
  });
});
