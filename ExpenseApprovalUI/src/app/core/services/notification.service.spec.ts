import { NotificationService, Notification } from './notification.service';

describe('NotificationService', () => {
  let service: NotificationService;

  beforeEach(() => {
    jest.useFakeTimers();
    service = new NotificationService();
  });

  afterEach(() => jest.useRealTimers());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('success emits a success notification', (done) => {
    service.notifications$.subscribe((n) => {
      if (n) {
        expect(n.type).toBe('success');
        expect(n.message).toBe('Done');
        done();
      }
    });
    service.success('Done');
  });

  it('error emits an error notification', (done) => {
    service.notifications$.subscribe((n) => {
      if (n) {
        expect(n.type).toBe('error');
        expect(n.message).toBe('Fail');
        done();
      }
    });
    service.error('Fail');
  });

  it('info emits an info notification', (done) => {
    service.notifications$.subscribe((n) => {
      if (n) {
        expect(n.type).toBe('info');
        expect(n.message).toBe('FYI');
        done();
      }
    });
    service.info('FYI');
  });

  it('clear resets notification to null', () => {
    let latest: Notification | null = null;
    service.notifications$.subscribe((n) => (latest = n));
    service.success('X');
    service.clear();
    expect(latest).toBeNull();
  });

  it('auto-clears after 5 seconds', () => {
    let latest: Notification | null = null;
    service.notifications$.subscribe((n) => (latest = n));
    service.success('auto');
    expect(latest).not.toBeNull();
    jest.advanceTimersByTime(5000);
    expect(latest).toBeNull();
  });
});
