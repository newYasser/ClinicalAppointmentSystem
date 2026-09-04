import { Component, input } from '@angular/core';


@Component({
  selector: 'app-page-header',
  template: `
    <div>
      @if (kicker()) {
        <div class="kicker">{{ kicker() }}</div>
      }
      <h1>{{ heading() }}</h1>
    </div>
    <div class="actions">
      <ng-content />
    </div>
  `,
  styles: `
    :host {
      display: flex;
      align-items: flex-end;
      justify-content: space-between;
      gap: 20px;
      margin-bottom: 26px;
    }

    .kicker {
      font-size: 10px;
      letter-spacing: 0.18em;
      text-transform: uppercase;
      color: var(--color-accent);
      margin-bottom: 6px;
    }

    h1 {
      margin: 0;
    }

    .actions {
      display: flex;
      align-items: center;
      gap: 10px;
    }
  `,
})
export class PageHeader {
  readonly kicker = input<string>();
  readonly heading = input.required<string>();
}
