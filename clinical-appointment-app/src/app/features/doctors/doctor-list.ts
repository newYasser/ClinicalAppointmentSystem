import { Component, computed, inject } from '@angular/core';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Params, Router, RouterLink } from '@angular/router';

import { DoctorApi } from '../../core/api/doctor-api';
import { SpecialtyApi } from '../../core/api/specialty-api';
import { PageSize } from '../../core/models/paged-result';
import { readId, readPage, readPageSize, readText } from '../../shared/routing/query-params';
import { EmptyState } from '../../shared/ui/empty-state';
import { ScreenState } from '../../shared/ui/screen-state';
import { PageHeader } from '../../shared/ui/page-header';
import { PageSizeSelect } from '../../shared/ui/page-size-select';
import { Pagination } from '../../shared/ui/pagination';

@Component({
  selector: 'app-doctor-list',
  imports: [ScreenState, RouterLink, PageHeader, PageSizeSelect, Pagination, EmptyState],
  templateUrl: './doctor-list.html',
  styleUrl: './doctor-list.scss',
})
export class DoctorList {
  private readonly api = inject(DoctorApi);
  private readonly specialtyApi = inject(SpecialtyApi);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  private readonly queryParams = toSignal(this.route.queryParamMap, { requireSync: true });

  protected readonly search = computed(() => readText(this.queryParams().get('search')));
  protected readonly specialtyId = computed(() => readId(this.queryParams().get('specialtyId')));
  protected readonly page = computed(() => readPage(this.queryParams().get('page')));
  protected readonly pageSize = computed(() => readPageSize(this.queryParams().get('pageSize')));

  private readonly specialties = rxResource({
    stream: () => this.specialtyApi.list(),
    defaultValue: [],
  });

  protected readonly specialtyOptions = computed(() =>
    this.specialties.hasValue() ? this.specialties.value() : [],
  );

  protected readonly doctors = rxResource({
    params: () => ({
      search: this.search(),
      specialtyId: this.specialtyId(),
      page: this.page(),
      pageSize: this.pageSize(),
    }),
    stream: ({ params }) => this.api.list(params),
  });

  protected readonly hasFilters = computed(
    () => this.search() !== '' || this.specialtyId() !== undefined,
  );

  protected applySearch(term: string): void {
    this.updateUrl({ search: term.trim() || null, page: null });
  }

  protected applySpecialty(raw: string): void {
    this.updateUrl({ specialtyId: raw || null, page: null });
  }

  protected clearFilters(): void {
    this.updateUrl({ search: null, specialtyId: null, page: null });
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
