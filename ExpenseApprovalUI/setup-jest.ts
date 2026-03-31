import 'jest-preset-angular/setup-jest';

Object.defineProperty(window, 'getComputedStyle', {
  value: () => ({
    display: 'none',
    appearance: ['-webkit-appearance'],
    getPropertyValue: () => ''
  })
});
