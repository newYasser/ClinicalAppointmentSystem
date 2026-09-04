import { Component, inject } from '@angular/core';
import { Toaster } from '../../core/notifications/toaster';
import { Blueprint } from './blueprint';

@Component({
  selector: 'app-toast-host',
  imports: [Blueprint],
  template: `
    @if (toaster.current(); as toast) {
      <div class="anchor">
        <div
          appBlueprint
          class="toast elev-md"
          [class.is-error]="toast.kind === 'error'"
          [attr.role]="toast.kind === 'error' ? 'alert' : 'status'"
          [attr.aria-live]="toast.kind === 'error' ? 'assertive' : 'polite'"
        >
          <span class="mark"></span>
          {{ toast.message }}
        </div>
      </div>
    }
  `,
  styles: `
    .anchor {
      position: fixed;
      left: 50%;
      bottom: 28px;
      transform: translateX(-50%);
      z-index: 50;
      animation: om-toast 0.18s ease-out;
    }

    .toast {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 12px 20px;
      font-size: 14px;
      background: var(--color-accent-900);
      border-color: var(--color-accent-900);
      color: var(--color-bg);
      --color-text: var(--color-bg);
    }

    .toast.is-error {
      background: var(--color-neutral-900);
      border-color: var(--color-neutral-900);
    }

    .mark {
      flex: none;
      display: block;
      width: 6px;
      height: 6px;
      background: var(--color-bg);
    }
  `,
})
export class ToastHost {
  protected readonly toaster = inject(Toaster);
}
