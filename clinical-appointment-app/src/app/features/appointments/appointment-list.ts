import { Component, computed, inject } from '@angular/core';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Params, Router, RouterLink } from '@angular/router';

import { AppointmentApi } from '../../core/api/appointment-api';
import { DoctorApi } from '../../core/api/doctor-api';
import { PageSize } from '../../core/models/paged-result';
import { DateLabelPipe } from '../../shared/format/date-label.pipe';
import { TimeLabelPipe } from '../../shared/format/time-label.pipe';
import {
  readDate,
  readId,
  readPage,
  readPageSize,
  readStatus,
  readText,
} from '../../shared/routing/query-params';
import { EmptyState } from '../../shared/ui/empty-state';
import { PageSizeSelect } from '../../shared/ui/page-size-select';
import { Pagination } from '../../shared/ui/pagination';
import { StatusTag } from '../../shared/ui/status-tag';

@Component({
  selector: 'app-appointment-list',
  imports: [
    RouterLink,
    PageSizeSelect,
    Pagination,
    StatusTag,
    EmptyState,
    DateLabelPipe,
    TimeLabelPipe,
  ],
  templateUrl: './appointment-list.html',
  styleUrl: './appointment-list.scss',
})
export class AppointmentListView {
  private readonly api = inject(AppointmentApi);
  private readonly doctorApi = inject(DoctorApi);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  private readonly queryParams = toSignal(this.route.queryParamMap, { requireSync: true });

  protected readonly search = computed(() => readText(this.queryParams().get('search')));
  protected readonly from = computed(() => readDate(this.queryParams().get('from')));
  protected readonly to = computed(() => readDate(this.queryParams().get('to')));
  protected readonly doctorId = computed(() => readId(this.queryParams().get('doctorId')));
  protected readonly status = computed(() => readStatus(this.queryParams().get('status')));
  protected readonly page = computed(() => readPage(this.queryParams().get('page')));
  protected readonly pageSize = computed(() => readPageSize(this.queryParams().get('pageSize')));

  private readonly doctors = rxResource({
    stream: () => this.doctorApi.lookup(),
    defaultValue: [],
  });

  protected readonly doctorOptions = computed(() =>
    this.doctors.hasValue() ? this.doctors.value() : [],
  );

  protected readonly appointments = rxResource({
    params: () => ({
      search: this.search(),
      from: this.from(),
      to: this.to(),
      doctorId: this.doctorId(),
      status: this.status(),
      page: this.page(),
      pageSize: this.pageSize(),
    }),
    stream: ({ params }) => this.api.list(params),
  });

  protected readonly hasFilters = computed(
    () =>
      this.search() !== '' ||
      this.from() !== undefined ||
      this.to() !== undefined ||
      this.doctorId() !== undefined ||
      this.status() !== undefined,
  );

  protected applySearch(term: string): void {
    this.updateUrl({ search: term.trim() || null, page: null });
  }

  protected applyFrom(raw: string): void {
    this.updateUrl({ from: raw || null, page: null });
  }

  protected applyTo(raw: string): void {
    this.updateUrl({ to: raw || null, page: null });
  }

  protected applyDoctor(raw: string): void {
    this.updateUrl({ doctorId: raw || null, page: null });
  }

  protected applyStatus(raw: string): void {
    this.updateUrl({ status: raw || null, page: null });
  }

  protected clearFilters(): void {
    this.updateUrl({
      search: null,
      from: null,
      to: null,
      doctorId: null,
      status: null,
      page: null,
    });
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
