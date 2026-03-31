import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ExpenseService } from '../../core/services/expense.service';
import { NotificationService } from '../../core/services/notification.service';
import { ExpenseRequest } from '../../core/models';

@Component({
  selector: 'app-expense-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './expense-detail.component.html',
  styleUrl: './expense-detail.component.scss'
})
export class ExpenseDetailComponent implements OnInit {
  expense: ExpenseRequest | null = null;
  loading = true;
  actionLoading = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private expenseService: ExpenseService,
    private notification: NotificationService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.expenseService.getById(id).subscribe({
      next: (data) => {
        this.expense = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.router.navigate(['/expenses']);
      }
    });
  }

  approve(): void {
    if (!this.expense) return;
    this.actionLoading = true;
    this.expenseService.approve(this.expense.id).subscribe({
      next: () => {
        this.notification.success('Expense approved successfully.');
        this.expense!.status = 'Approved';
        this.actionLoading = false;
      },
      error: () => {
        this.actionLoading = false;
      }
    });
  }

  reject(): void {
    if (!this.expense) return;
    this.actionLoading = true;
    this.expenseService.reject(this.expense.id).subscribe({
      next: () => {
        this.notification.success('Expense rejected successfully.');
        this.expense!.status = 'Rejected';
        this.actionLoading = false;
      },
      error: () => {
        this.actionLoading = false;
      }
    });
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'approved': return 'status-approved';
      case 'rejected': return 'status-rejected';
      case 'pending': return 'status-pending';
      default: return '';
    }
  }
}
