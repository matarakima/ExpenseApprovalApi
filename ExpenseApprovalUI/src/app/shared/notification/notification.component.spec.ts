import { TestBed } from '@angular/core/testing';
import { NotificationComponent } from './notification.component';
import { NotificationService } from '../../core/services/notification.service';

describe('NotificationComponent', () => {
  let notificationService: NotificationService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotificationComponent],
      providers: [NotificationService]
    }).compileComponents();

    notificationService = TestBed.inject(NotificationService);
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(NotificationComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('displays a success notification', () => {
    const fixture = TestBed.createComponent(NotificationComponent);
    notificationService.success('Saved!');
    fixture.detectChanges();

    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.toast-success')).toBeTruthy();
    expect(el.querySelector('.toast-message')?.textContent).toContain('Saved!');
  });

  it('displays an error notification', () => {
    const fixture = TestBed.createComponent(NotificationComponent);
    notificationService.error('Oops');
    fixture.detectChanges();

    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.toast-error')).toBeTruthy();
  });

  it('dismiss clears the notification', () => {
    const fixture = TestBed.createComponent(NotificationComponent);
    notificationService.success('Test');
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.toast')).toBeTruthy();

    fixture.componentInstance.dismiss();
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.toast')).toBeFalsy();
  });

  it('shows nothing when no notification', () => {
    const fixture = TestBed.createComponent(NotificationComponent);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.toast')).toBeFalsy();
  });
});
