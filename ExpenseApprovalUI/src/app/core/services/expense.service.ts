import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ExpenseRequest,
  CreateExpenseRequest,
  UpdateExpenseRequest,
  ExpenseMetrics,
  ExpenseFilter,
  Category
} from '../models';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class ExpenseService {
  private readonly url = `${environment.apiUrl}/expenses`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  getAll(): Observable<ExpenseRequest[]> {
    return this.http.get<ExpenseRequest[]>(this.url);
  }

  getById(id: string): Observable<ExpenseRequest> {
    return this.http.get<ExpenseRequest>(`${this.url}/${id}`);
  }

  create(expense: CreateExpenseRequest): Observable<ExpenseRequest> {
    return this.http.post<ExpenseRequest>(this.url, expense);
  }

  update(id: string, expense: UpdateExpenseRequest): Observable<ExpenseRequest> {
    return this.http.put<ExpenseRequest>(`${this.url}/${id}`, expense);
  }

  approve(id: string): Observable<ExpenseRequest> {
    const params = new HttpParams().set('decisionById', this.authService.userId ?? '');
    return this.http.patch<ExpenseRequest>(`${this.url}/${id}/approve`, {}, { params });
  }

  reject(id: string): Observable<ExpenseRequest> {
    const params = new HttpParams().set('decisionById', this.authService.userId ?? '');
    return this.http.patch<ExpenseRequest>(`${this.url}/${id}/reject`, {}, { params });
  }

  filter(filters: ExpenseFilter): Observable<ExpenseRequest[]> {
    let params = new HttpParams();
    if (filters.status) params = params.set('status', filters.status);
    if (filters.category) params = params.set('category', filters.category);
    if (filters.fromDate) params = params.set('fromDate', filters.fromDate);
    if (filters.toDate) params = params.set('toDate', filters.toDate);
    return this.http.get<ExpenseRequest[]>(`${this.url}/filter`, { params });
  }

  getMetrics(): Observable<ExpenseMetrics> {
    return this.http.get<ExpenseMetrics>(`${this.url}/metrics`);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${environment.apiUrl}/categories`);
  }

}
