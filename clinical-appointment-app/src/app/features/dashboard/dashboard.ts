import { Component, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { Params, RouterLink } from '@angular/router';

import { DashboardApi } from '../../core/api/dashboard-api';
import { IsoDate } from '../../core/models/primitives';
import { TimeLabelPipe } from '../../shared/format/time-label.pipe';
import { Blueprint } from '../../shared/ui/blueprint';
import { EmptyState } from '../../shared/ui/empty-state';
import { PageHeader } from '../../shared/ui/page-header';
import { StatusTag } from '../../shared/ui/status-tag';

interface StatTile {
  readonly label: string;
  readonly value: number;
  readonly sub: string;
  readonly link: string;
  readonly queryParams: Params;
}

function addDays(date: IsoDate, days: number): IsoDate {
  const shifted = new Date(`${date}T12:00:00`);
  shifted.setDate(shifted.getDate() + days);

  const month = String(shifted.getMonth() + 1).padStart(2, '0');
  const day = String(shifted.getDate()).padStart(2, '0');

  return `${shifted.getFullYear()}-${month}-${day}`;
}

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, Blueprint, PageHeader, StatusTag, EmptyState, TimeLabelPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  private readonly api = inject(DashboardApi);

  protected readonly summary = rxResource({ stream: () => this.api.summary() });

  private readonly loaded = computed(() =>
    this.summary.hasValue() ? this.summary.value() : undefined,
  );

  protected readonly newAppointmentParams = computed<Params>(() => {
    const today = this.loaded()?.today;
    return today ? { date: today } : {};
  });

  protected readonly tiles = computed<readonly StatTile[]>(() => {
    const summary = this.loaded();

    if (!summary) {
      return [];
    }

    return [
      {
        label: 'Registered patients',
        value: summary.totalPatients,
        sub: 'Active register',
        link: '/patients',
        queryParams: {},
      },
      {
        label: 'Doctors',
        value: summary.totalDoctors,
        sub: `${summary.specialtyCount} specialties`,
        link: '/doctors',
        queryParams: {},
      },
      {
        label: 'Today',
        value: summary.todayAppointmentCount,
        sub: 'Appointments scheduled today',
        link: '/appointments',
        queryParams: { view: 'day', date: summary.today },
      },
      {
        label: 'Upcoming',
        value: summary.upcomingAppointmentCount,
        sub: 'Scheduled after today',
        link: '/appointments',
        queryParams: { view: 'list', from: addDays(summary.today, 1), status: 'Scheduled' },
      },
    ];
  });
}
