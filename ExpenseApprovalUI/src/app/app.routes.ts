import { Routes } from '@angular/router';
import { authGuard, loginGuard } from './core/guards/auth.guard';
import { LayoutComponent } from './shared/layout/layout.component';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [loginGuard],
    loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'expenses', loadComponent: () => import('./expenses/expense-list/expense-list.component').then(m => m.ExpenseListComponent) },
      { path: 'expenses/new', loadComponent: () => import('./expenses/expense-form/expense-form.component').then(m => m.ExpenseFormComponent) },
      { path: 'expenses/:id', loadComponent: () => import('./expenses/expense-detail/expense-detail.component').then(m => m.ExpenseDetailComponent) },
      { path: 'expenses/:id/edit', loadComponent: () => import('./expenses/expense-form/expense-form.component').then(m => m.ExpenseFormComponent) },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
