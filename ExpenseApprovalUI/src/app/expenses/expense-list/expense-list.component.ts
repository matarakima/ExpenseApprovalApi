import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../core/services/expense.service';
import { ExpenseRequest, ExpenseFilter } from '../../core/models';

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './expense-list.component.html',
  styleUrl: './expense-list.component.scss'
})
export class ExpenseListComponent implements OnInit {
  expenses: ExpenseRequest[] = [];
  loading = true;

  filters: ExpenseFilter = {};
  statusOptions = ['Pending', 'Approved', 'Rejected'];

  constructor(private expenseService: ExpenseService) {}

  ngOnInit(): void {
    this.loadExpenses();
  }

  loadExpenses(): void {
    this.loading = true;
    const hasFilters = this.filters.status || this.filters.category || this.filters.fromDate || this.filters.toDate;

    const source = hasFilters
      ? this.expenseService.filter(this.filters)
      : this.expenseService.getAll();

    source.subscribe({
      next: (data) => {
        this.expenses = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.loadExpenses();
  }

  clearFilters(): void {
    this.filters = {};
    this.loadExpenses();
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
