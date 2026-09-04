import { Component, inject } from '@angular/core';
import { Confirmer } from '../../core/notifications/confirmer';
import { Blueprint } from './blueprint';

const TITLE_ID = 'confirm-dialog-title';

@Component({
  selector: 'app-confirm-dialog',
  imports: [Blueprint],
  host: { '(document:keydown.escape)': 'confirmer.dismiss()' },
  template: `
    @if (confirmer.current(); as prompt) {
      <div class="dialog-backdrop">
        <div
          appBlueprint
          class="dialog elev-lg"
          role="dialog"
          aria-modal="true"
          [attr.aria-labelledby]="titleId"
        >
          <div class="dialog-title" [id]="titleId">{{ prompt.title }}</div>
          <div class="dialog-body">{{ prompt.body }}</div>
          <div class="dialog-actions">
            <button type="button" class="btn btn-secondary" (click)="confirmer.dismiss()">
              {{ prompt.cancelLabel }}
            </button>
            @if (prompt.confirmLabel) {
              <button type="button" class="btn btn-primary" (click)="confirmer.confirm()">
                {{ prompt.confirmLabel }}
              </button>
            }
          </div>
        </div>
      </div>
    }
  `,
  styles: `
    .dialog-backdrop {
      z-index: 40;
    }

    .dialog {
      background: var(--color-bg);
    }
  `,
})
export class ConfirmDialog {
  protected readonly confirmer = inject(Confirmer);
  protected readonly titleId = TITLE_ID;
}
