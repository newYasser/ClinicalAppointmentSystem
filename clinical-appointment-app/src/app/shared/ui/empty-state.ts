import { Component } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  template: `<ng-content />`,
  styles: `
    :host {
      display: block;
      padding: 34px;
      text-align: center;
      color: var(--color-neutral-600);
      border: 1px dashed var(--color-divider);
    }
  `,
})
export class EmptyState {}
