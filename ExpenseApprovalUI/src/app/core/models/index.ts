export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
}

export interface ExpenseRequest {
  id: string;
  category: string;
  description: string;
  amount: number;
  expenseDate: string;
  requestedBy: string;
  status: string;
  createdAt: string;
  decisionDate: string | null;
  decisionBy: string | null;
}

export interface CreateExpenseRequest {
  categoryId: string;
  description: string;
  amount: number;
  expenseDate: string;
}

export interface UpdateExpenseRequest {
  categoryId: string;
  description: string;
  amount: number;
  expenseDate: string;
}

export interface ExpenseMetrics {
  totalRequests: number;
  approvedCount: number;
  rejectedCount: number;
  pendingCount: number;
  totalApprovedAmount: number;
}

export interface ExpenseFilter {
  status?: string;
  category?: string;
  fromDate?: string;
  toDate?: string;
}

export interface User {
  id: string;
  auth0Id: string;
  email: string;
  fullName: string;
  roleName: string;
  claims: string[];
}

export interface Role {
  id: string;
  name: string;
  claims: string[];
}

export interface Category {
  id: string;
  name: string;
}
