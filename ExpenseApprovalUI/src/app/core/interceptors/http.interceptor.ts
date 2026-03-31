import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.token;

  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq);
};

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const notification = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = 'An unexpected error occurred.';

      switch (error.status) {
        case 0:
          message = 'Unable to connect to the server. Please check your connection.';
          break;
        case 400:
          message = error.error?.message || 'Invalid request data.';
          break;
        case 401:
          message = 'Session expired. Please log in again.';
          authService.logout();
          break;
        case 403:
          message = 'You do not have permission to perform this action.';
          break;
        case 404:
          message = 'The requested resource was not found.';
          break;
        case 500:
          message = 'Internal server error. Please try again later.';
          break;
      }

      notification.error(message);
      return throwError(() => error);
    })
  );
};
