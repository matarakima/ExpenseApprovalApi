import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Notification {
  message: string;
  type: 'success' | 'error' | 'info';
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private notification$ = new BehaviorSubject<Notification | null>(null);
  notifications$ = this.notification$.asObservable();

  success(message: string): void {
    this.show({ message, type: 'success' });
  }

  error(message: string): void {
    this.show({ message, type: 'error' });
  }

  info(message: string): void {
    this.show({ message, type: 'info' });
  }

  clear(): void {
    this.notification$.next(null);
  }

  private show(notification: Notification): void {
    this.notification$.next(notification);
    setTimeout(() => this.clear(), 5000);
  }
}
