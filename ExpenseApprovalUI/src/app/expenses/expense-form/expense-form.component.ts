import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ExpenseService } from '../../core/services/expense.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-expense-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './expense-form.component.html',
  styleUrl: './expense-form.component.scss'
})
export class ExpenseFormComponent implements OnInit {
  form!: FormGroup;
  isEdit = false;
  expenseId: string | null = null;
  loading = false;
  pageLoading = true;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private expenseService: ExpenseService,
    private notification: NotificationService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      description: ['', [Validators.required, Validators.maxLength(500)]],
      amount: [null, [Validators.required, Validators.min(0.01)]],
      expenseDate: ['', Validators.required],
      categoryId: ['', Validators.required]
    });

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.expenseId = id;
      this.expenseService.getById(this.expenseId).subscribe({
        next: (exp) => {
          if (exp.status !== 'Pending') {
            this.notification.error('Only pending expenses can be edited.');
            this.router.navigate(['/expenses', this.expenseId]);
            return;
          }
          this.form.patchValue({
            description: exp.description,
            amount: exp.amount,
            expenseDate: exp.expenseDate?.split('T')[0],
            categoryId: exp.category
          });
          this.pageLoading = false;
        },
        error: () => {
          this.router.navigate(['/expenses']);
        }
      });
    } else {
      this.pageLoading = false;
    }
  }

  get f() {
    return this.form.controls;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    const data = this.form.value;

    const action = this.isEdit
      ? this.expenseService.update(this.expenseId!, data)
      : this.expenseService.create(data);

    action.subscribe({
      next: (result) => {
        this.notification.success(
          this.isEdit ? 'Expense updated successfully.' : 'Expense created successfully.'
        );
        this.router.navigate(['/expenses', result.id ?? this.expenseId]);
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
