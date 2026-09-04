import { Component, input, output } from '@angular/core';
import { EmptyState } from './empty-state';

@Component({
  selector: 'app-screen-state',
  imports: [EmptyState],
  template: `
    @if (error(); as failure) {
      <app-empty-state role="alert">
        {{ failure.message }}
        <button type="button" class="btn btn-ghost" (click)="retry.emit()">Try again</button>
      </app-empty-state>
    } @else if (loading()) {
      <p class="loading text-muted">{{ message() }}</p>
    }
  `,
  styles: `
    .loading {
      padding: 24px 0;
      margin: 0;
    }
  `,
})
export class ScreenState {
  readonly loading = input(false);
  readonly error = input<Error | undefined>(undefined);
  readonly message = input('Loading…');

  readonly retry = output<void>();
}
