import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Params, Router, RouterLink } from '@angular/router';

import { Blueprint } from '../../shared/ui/blueprint';
import { PageHeader } from '../../shared/ui/page-header';
import { AppointmentListView } from './appointment-list';
import { DayBoardView } from './day-board';

export type AppointmentsView = 'day' | 'list';

@Component({
  selector: 'app-appointments',
  imports: [RouterLink, Blueprint, PageHeader, DayBoardView, AppointmentListView],
  templateUrl: './appointments.html',
  styleUrl: './appointments.scss',
})
export class Appointments {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  private readonly queryParams = toSignal(this.route.queryParamMap, { requireSync: true });

  protected readonly view = computed<AppointmentsView>(() =>
    this.queryParams().get('view') === 'list' ? 'list' : 'day',
  );

  protected readonly newAppointmentParams = computed<Params>(() => {
    const date = this.queryParams().get('date');
    return date ? { date } : {};
  });

  protected setView(view: AppointmentsView): void {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { view },
      queryParamsHandling: 'merge',
    });
  }
}
