import { Component, computed, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import { AppointmentApi } from '../../core/api/appointment-api';
import { DoctorApi } from '../../core/api/doctor-api';
import { PatientApi } from '../../core/api/patient-api';
import { ClinicConfig } from '../../core/clinic/clinic-config';
import { ApiError } from '../../core/http/api-error';
import { AppointmentRequest } from '../../core/models/appointment';
import { Toaster } from '../../core/notifications/toaster';
import { isIsoDate } from '../../shared/format/iso-date';
import { TimeLabelPipe } from '../../shared/format/time-label.pipe';
import { readId } from '../../shared/routing/query-params';
import { Blueprint } from '../../shared/ui/blueprint';

type FieldName = 'patientId' | 'doctorId' | 'date' | 'startTime' | 'notes';

const MESSAGES: Record<FieldName, string> = {
  patientId: 'Patient is required.',
  doctorId: 'Doctor is required.',
  date: 'Appointment date is required.',
  startTime: 'Appointment time is required.',
  notes: 'Notes cannot exceed 1000 characters.',
};

@Component({
  selector: 'app-appointment-form',
  imports: [ReactiveFormsModule, RouterLink, Blueprint, TimeLabelPipe],
  templateUrl: './appointment-form.html',
  styleUrl: './appointment-form.scss',
})
export class AppointmentForm {
  private readonly api = inject(AppointmentApi);
  private readonly patientApi = inject(PatientApi);
  private readonly doctorApi = inject(DoctorApi);
  private readonly clinic = inject(ClinicConfig);
  private readonly toaster = inject(Toaster);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected readonly slots = this.clinic.slots;
  protected readonly durationMinutes = this.clinic.durationMinutes;

  private readonly patients = rxResource({
    stream: () => this.patientApi.lookup(),
    defaultValue: [],
  });

  private readonly doctors = rxResource({
    stream: () => this.doctorApi.lookup(),
    defaultValue: [],
  });

  protected readonly patientOptions = computed(() =>
    this.patients.hasValue() ? this.patients.value() : [],
  );

  protected readonly doctorOptions = computed(() =>
    this.doctors.hasValue() ? this.doctors.value() : [],
  );

  protected readonly form = new FormGroup({
    patientId: new FormControl<number | null>(null, Validators.required),
    doctorId: new FormControl<number | null>(null, Validators.required),
    date: new FormControl('', Validators.required),
    startTime: new FormControl('', Validators.required),
    notes: new FormControl('', Validators.maxLength(1000)),
  });

  protected readonly submitted = signal(false);
  protected readonly saving = signal(false);
  protected readonly conflict = signal<string | null>(null);

  private readonly serverErrors = signal<Record<string, string[]>>({});

  constructor() {
    const params = this.route.snapshot.queryParamMap;
    const date = params.get('date');
    const startTime = params.get('startTime');

    this.form.patchValue({
      patientId: readId(params.get('patientId')) ?? null,
      doctorId: readId(params.get('doctorId')) ?? null,
      date: isIsoDate(date) ? date : '',
      startTime: startTime ?? '',
    });
  }

  protected fieldError(name: FieldName): string | null {
    const fromServer = this.serverErrors()[name]?.[0];

    if (fromServer) {
      return fromServer;
    }

    return this.submitted() && this.form.controls[name].invalid ? MESSAGES[name] : null;
  }

  protected async save(): Promise<void> {
    this.submitted.set(true);
    this.conflict.set(null);
    this.serverErrors.set({});

    if (this.form.invalid || this.saving()) {
      return;
    }

    this.saving.set(true);

    try {
      const created = await firstValueFrom(this.api.create(this.toRequest()));

      this.toaster.success('Appointment scheduled.');
      await this.router.navigate(['/appointments'], {
        queryParams: { view: 'day', date: created.date },
      });
    } catch (error) {
      this.report(error);
    } finally {
      this.saving.set(false);
    }
  }

  private toRequest(): AppointmentRequest {
    const value = this.form.getRawValue();

    return {
      patientId: value.patientId as number,
      doctorId: value.doctorId as number,
      date: value.date ?? '',
      startTime: value.startTime ?? '',
      notes: value.notes?.trim() || null,
    };
  }

  private report(error: unknown): void {
    if (!(error instanceof ApiError)) {
      this.conflict.set('Something went wrong. Please try again.');
      return;
    }

    if (Object.keys(error.fieldErrors).length > 0) {
      this.serverErrors.set(error.fieldErrors);
      return;
    }

    this.conflict.set(error.message);
  }
}
