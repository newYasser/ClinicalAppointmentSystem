import { Component, computed, inject } from '@angular/core';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { PatientApi } from '../../core/api/patient-api';
import { formatDateLabel } from '../../shared/format/date-label';
import { DateLabelPipe } from '../../shared/format/date-label.pipe';
import { TimeLabelPipe } from '../../shared/format/time-label.pipe';
import { Blueprint } from '../../shared/ui/blueprint';
import { EmptyState } from '../../shared/ui/empty-state';
import { PageHeader } from '../../shared/ui/page-header';
import { StatusTag } from '../../shared/ui/status-tag';

interface Fact {
  readonly label: string;
  readonly value: string;
}

@Component({
  selector: 'app-patient-detail',
  imports: [
    RouterLink,
    Blueprint,
    PageHeader,
    StatusTag,
    EmptyState,
    DateLabelPipe,
    TimeLabelPipe,
  ],
  templateUrl: './patient-detail.html',
  styleUrl: './patient-detail.scss',
})
export class PatientDetailPage {
  private readonly api = inject(PatientApi);
  private readonly route = inject(ActivatedRoute);

  private readonly routeParams = toSignal(this.route.paramMap, { requireSync: true });

  protected readonly patientId = computed(() => Number(this.routeParams().get('id')));

  protected readonly patient = rxResource({
    params: () => this.patientId(),
    stream: ({ params: id }) => this.api.get(id),
  });

  private readonly loaded = computed(() =>
    this.patient.hasValue() ? this.patient.value() : undefined,
  );

  protected readonly facts = computed<readonly Fact[]>(() => {
    const patient = this.loaded();

    if (!patient) {
      return [];
    }

    return [
      { label: 'Date of birth', value: formatDateLabel(patient.dateOfBirth) },
      { label: 'Phone', value: patient.phone },
      { label: 'Email', value: patient.email },
      { label: 'Appointments', value: String(patient.appointmentCount) },
    ];
  });
}
