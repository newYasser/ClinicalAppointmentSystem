import { Component, computed, inject } from '@angular/core';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Params, Router, RouterLink } from '@angular/router';

import { PatientApi } from '../../core/api/patient-api';
import { PageSize } from '../../core/models/paged-result';
import { DateLabelPipe } from '../../shared/format/date-label.pipe';
import { readPage, readPageSize, readText } from '../../shared/routing/query-params';
import { EmptyState } from '../../shared/ui/empty-state';
import { PageHeader } from '../../shared/ui/page-header';
import { PageSizeSelect } from '../../shared/ui/page-size-select';
import { Pagination } from '../../shared/ui/pagination';

@Component({
  selector: 'app-patient-list',
  imports: [RouterLink, PageHeader, PageSizeSelect, Pagination, EmptyState, DateLabelPipe],
  templateUrl: './patient-list.html',
  styleUrl: './patient-list.scss',
})
export class PatientList {
  private readonly api = inject(PatientApi);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  private readonly queryParams = toSignal(this.route.queryParamMap, { requireSync: true });

  protected readonly search = computed(() => readText(this.queryParams().get('search')));
  protected readonly page = computed(() => readPage(this.queryParams().get('page')));
  protected readonly pageSize = computed(() => readPageSize(this.queryParams().get('pageSize')));

  protected readonly patients = rxResource({
    params: () => ({
      search: this.search(),
      page: this.page(),
      pageSize: this.pageSize(),
    }),
    stream: ({ params }) => this.api.list(params),
  });

  protected applySearch(term: string): void {
    this.updateUrl({ search: term.trim() || null, page: null });
  }

  protected clearSearch(): void {
    this.updateUrl({ search: null, page: null });
  }

  protected changePageSize(size: PageSize): void {
    this.updateUrl({ pageSize: size, page: null });
  }

  protected goToPage(page: number): void {
    this.updateUrl({ page });
  }

  private updateUrl(changes: Params): void {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: changes,
      queryParamsHandling: 'merge',
    });
  }
}
