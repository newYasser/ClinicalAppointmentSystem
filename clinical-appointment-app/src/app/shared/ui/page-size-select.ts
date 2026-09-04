import { Component, input, output } from '@angular/core';
import { PAGE_SIZES, PageSize } from '../../core/models/paged-result';

@Component({
  selector: 'app-page-size-select',
  template: `
    <div class="field">
      <label for="page-size">Per page</label>
      <select id="page-size" class="input" (change)="onChange($event)">
        @for (size of sizes; track size) {
          <option [value]="size" [selected]="size === pageSize()">{{ size }}</option>
        }
      </select>
    </div>
  `,
  styles: `
    :host {
      display: block;
      width: 110px;
    }
  `,
})
export class PageSizeSelect {
  readonly pageSize = input.required<PageSize>();

  readonly pageSizeChange = output<PageSize>();

  protected readonly sizes = PAGE_SIZES;

  protected onChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSizeChange.emit(value as PageSize);
  }
}
