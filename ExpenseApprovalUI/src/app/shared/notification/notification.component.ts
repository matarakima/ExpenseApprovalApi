import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService, Notification } from '../../core/services/notification.service';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (notification$ | async; as n) {
      <div class="toast" [class]="'toast-' + n.type" (click)="dismiss()">
        <span class="toast-icon">
          @switch (n.type) {
            @case ('success') { ✓ }
            @case ('error') { ✕ }
            @case ('info') { ℹ }
          }
        </span>
        <span class="toast-message">{{ n.message }}</span>
      </div>
    }
  `,
  styles: [`
    .toast {
      position: fixed;
      top: 1rem;
      right: 1rem;
      padding: 0.85rem 1.25rem;
      border-radius: 8px;
      display: flex;
      align-items: center;
      gap: 0.6rem;
      font-size: 0.9rem;
      font-weight: 500;
      cursor: pointer;
      z-index: 9999;
      animation: slideIn 0.3s ease;
      box-shadow: 0 4px 16px rgba(0,0,0,0.12);
      max-width: 400px;
    }
    .toast-success { background: #ecfdf5; color: #065f46; border: 1px solid #a7f3d0; }
    .toast-error { background: #fef2f2; color: #b91c1c; border: 1px solid #fecaca; }
    .toast-info { background: #eff6ff; color: #1e40af; border: 1px solid #bfdbfe; }
    .toast-icon { font-size: 1.1rem; }
    @keyframes slideIn {
      from { transform: translateX(100%); opacity: 0; }
      to { transform: translateX(0); opacity: 1; }
    }
  `]
})
export class NotificationComponent {
  notification$;

  constructor(private notificationService: NotificationService) {
    this.notification$ = this.notificationService.notifications$;
  }

  dismiss(): void {
    this.notificationService.clear();
  }
}
