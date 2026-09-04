import { Component, computed, inject } from '@angular/core';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Params, Router, RouterLink } from '@angular/router';

import { AppointmentApi } from '../../core/api/appointment-api';
import { SpecialtyApi } from '../../core/api/specialty-api';
import { IsoDate } from '../../core/models/primitives';
import { formatDateLabel } from '../../shared/format/date-label';
import { isIsoDate, shiftIsoDate, todayIso } from '../../shared/format/iso-date';
import { TimeLabelPipe } from '../../shared/format/time-label.pipe';
import { readId } from '../../shared/routing/query-params';
import { EmptyState } from '../../shared/ui/empty-state';

@Component({
  selector: 'app-day-board',
  imports: [RouterLink, EmptyState, TimeLabelPipe],
  templateUrl: './day-board.html',
  styleUrl: './day-board.scss',
})
export class DayBoardView {
  private readonly api = inject(AppointmentApi);
  private readonly specialtyApi = inject(SpecialtyApi);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  private readonly queryParams = toSignal(this.route.queryParamMap, { requireSync: true });

  protected readonly date = computed<IsoDate>(() => {
    const raw = this.queryParams().get('date');
    return isIsoDate(raw) ? raw : todayIso();
  });

  protected readonly specialtyId = computed(() => readId(this.queryParams().get('specialtyId')));
  protected readonly doctorId = computed(() => readId(this.queryParams().get('doctorId')));

  private readonly specialties = rxResource({
    stream: () => this.specialtyApi.list(),
    defaultValue: [],
  });

  protected readonly specialtyOptions = computed(() =>
    this.specialties.hasValue() ? this.specialties.value() : [],
  );

  protected readonly board = rxResource({
    params: () => ({
      date: this.date(),
      specialtyId: this.specialtyId(),
      doctorId: this.doctorId(),
    }),
    stream: ({ params }) => this.api.dayBoard(params),
  });

  protected readonly heading = computed(() => formatDateLabel(this.date()));

  protected stepDay(days: number): void {
    this.updateUrl({ date: shiftIsoDate(this.date(), days) });
  }

  protected goToToday(): void {
    this.updateUrl({ date: todayIso() });
  }

  protected goToDay(raw: string): void {
    if (isIsoDate(raw)) {
      this.updateUrl({ date: raw });
    }
  }

  protected applySpecialty(raw: string): void {
    this.updateUrl({ specialtyId: raw || null });
  }

  protected showAllDoctors(): void {
    this.updateUrl({ doctorId: null });
  }

  private updateUrl(changes: Params): void {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: changes,
      queryParamsHandling: 'merge',
    });
  }
}
