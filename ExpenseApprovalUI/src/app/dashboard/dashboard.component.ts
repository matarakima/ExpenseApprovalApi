import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ExpenseService } from '../core/services/expense.service';
import { ExpenseMetrics } from '../core/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  metrics: ExpenseMetrics | null = null;
  loading = true;
  error = '';

  constructor(private expenseService: ExpenseService) {}

  ngOnInit(): void {
    this.expenseService.getMetrics().subscribe({
      next: (data) => {
        this.metrics = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load metrics.';
        this.loading = false;
      }
    });
  }
}
