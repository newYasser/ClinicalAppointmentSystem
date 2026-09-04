import { Component, computed, input, output } from '@angular/core';
import { PagedResult } from '../../core/models/paged-result';

@Component({
  selector: 'app-pagination',
  template: `
    <span>{{ label() }}</span>

    <div class="controls">
      <button
        type="button"
        class="btn btn-secondary"
        [disabled]="isFirst()"
        (click)="pageChange.emit(result().page - 1)"
      >
        ← Prev
      </button>

      <span class="position">Page {{ result().page }} of {{ result().totalPages }}</span>

      <button
        type="button"
        class="btn btn-secondary"
        [disabled]="isLast()"
        (click)="pageChange.emit(result().page + 1)"
      >
        Next →
      </button>
    </div>
  `,
  styles: `
    :host {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 20px;
      margin-top: 18px;
      font-size: 13px;
      color: var(--color-neutral-700);
    }

    .controls {
      display: flex;
      align-items: center;
      gap: 6px;
    }

    .position {
      padding: 0 8px;
    }
  `,
})
export class Pagination {
  readonly result = input.required<PagedResult<unknown>>();
  readonly noun = input.required<string>();

  readonly pageChange = output<number>();

  protected readonly label = computed(() => {
    const { totalCount, from, to } = this.result();

    return totalCount === 0
      ? `0 ${this.noun()}`
      : `Showing ${from}–${to} of ${totalCount} ${this.noun()}`;
  });

  protected readonly isFirst = computed(() => this.result().page <= 1);

  protected readonly isLast = computed(() => this.result().page >= this.result().totalPages);
}
