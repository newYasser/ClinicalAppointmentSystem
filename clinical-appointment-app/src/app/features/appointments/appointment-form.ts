import { Component, computed, effect, inject, signal } from '@angular/core';
import { rxResource, toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import { AppointmentApi } from '../../core/api/appointment-api';
import { DoctorApi } from '../../core/api/doctor-api';
import { PatientApi } from '../../core/api/patient-api';
import { ClinicConfig } from '../../core/clinic/clinic-config';
import { ApiError } from '../../core/http/api-error';
import { AppointmentRequest } from '../../core/models/appointment';
import { AppointmentStatus } from '../../core/models/appointment-status';
import { TimeOfDay } from '../../core/models/primitives';
import { Toaster } from '../../core/notifications/toaster';
import { formatDateLabel } from '../../shared/format/date-label';
import { isIsoDate } from '../../shared/format/iso-date';
import { TimeLabelPipe } from '../../shared/format/time-label.pipe';
import { readId } from '../../shared/routing/query-params';
import { Blueprint } from '../../shared/ui/blueprint';
import { ScreenState } from '../../shared/ui/screen-state';

type FieldName = 'patientId' | 'doctorId' | 'date' | 'startTime' | 'notes';

const MESSAGES: Record<FieldName, string> = {
  patientId: 'Patient is required.',
  doctorId: 'Doctor is required.',
  date: 'Appointment date is required.',
  startTime: 'Appointment time is required.',
  notes: 'Notes cannot exceed 1000 characters.',
};

interface SlotOption {
  readonly time: TimeOfDay;
  readonly picked: boolean;
  readonly free: boolean;
}

@Component({
  selector: 'app-appointment-form',
  imports: [ScreenState, ReactiveFormsModule, RouterLink, Blueprint, TimeLabelPipe],
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

  private readonly routeParams = toSignal(this.route.paramMap, { requireSync: true });

  protected readonly appointmentId = computed(() => readId(this.routeParams().get('id')));
  protected readonly isEdit = computed(() => this.appointmentId() !== undefined);

  protected readonly existing = rxResource({
    params: () => this.appointmentId(),
    stream: ({ params: id }) => this.api.get(id),
  });

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

  private readonly formValue = toSignal(this.form.valueChanges, {
    initialValue: this.form.getRawValue(),
  });

  private readonly chosenDoctorId = computed(() => this.formValue().doctorId ?? null);
  private readonly chosenPatientId = computed(() => this.formValue().patientId ?? null);
  private readonly chosenDate = computed(() => this.formValue().date ?? '');
  private readonly chosenTime = computed(() => this.formValue().startTime ?? '');

  private readonly availability = rxResource({
    params: () => {
      const doctorId = this.chosenDoctorId();
      const date = this.chosenDate();

      if (doctorId === null || !isIsoDate(date)) {
        return undefined;
      }

      return {
        doctorId,
        date,
        patientId: this.chosenPatientId() ?? undefined,
        excludeAppointmentId: this.appointmentId(),
      };
    },
    stream: ({ params }) =>
      this.doctorApi.availability(params.doctorId, {
        date: params.date,
        patientId: params.patientId,
        excludeAppointmentId: params.excludeAppointmentId,
      }),
  });

  protected readonly title = computed(() =>
    this.isEdit() ? 'Edit appointment' : 'New appointment',
  );

  protected readonly status = computed<AppointmentStatus>(() =>
    this.existing.hasValue() ? this.existing.value().status : 'Scheduled',
  );

  protected readonly availabilityLabel = computed(() => {
    const loaded = this.availability.hasValue() ? this.availability.value() : undefined;

    return loaded ? `Availability — ${loaded.doctorName} · ${formatDateLabel(loaded.date)}` : '';
  });

  protected readonly availabilityLoading = this.availability.isLoading;

  protected readonly slotOptions = computed<readonly SlotOption[]>(() => {
    if (!this.availability.hasValue()) {
      return [];
    }

    const picked = this.chosenTime();

    return this.availability.value().slots.map((slot) => ({
      time: slot.startTime,
      picked: slot.startTime === picked,
      free: slot.state === 'Free',
    }));
  });

  protected readonly submitted = signal(false);
  protected readonly saving = signal(false);
  protected readonly conflict = signal<string | null>(null);

  private readonly serverErrors = signal<Record<string, string[]>>({});
  private seededId: number | null = null;

  constructor() {
    if (!this.isEdit()) {
      this.seedFromQueryParams();
    }

    effect(() => {
      if (!this.existing.hasValue()) {
        return;
      }

      const appointment = this.existing.value();

      if (this.seededId === appointment.id) {
        return;
      }

      this.seededId = appointment.id;
      this.form.patchValue({
        patientId: appointment.patientId,
        doctorId: appointment.doctorId,
        date: appointment.date,
        startTime: appointment.startTime,
        notes: appointment.notes ?? '',
      });
    });
  }

  protected pickSlot(time: TimeOfDay): void {
    this.form.patchValue({ startTime: time });
    this.conflict.set(null);
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

    const id = this.appointmentId();
    const request = this.toRequest();

    try {
      const saved = await firstValueFrom(
        id === undefined ? this.api.create(request) : this.api.update(id, request),
      );

      this.toaster.success(
        id === undefined ? 'Appointment scheduled.' : 'Appointment updated.',
      );
      await this.router.navigate(['/appointments'], {
        queryParams: { view: 'day', date: saved.date },
      });
    } catch (error) {
      this.report(error);
    } finally {
      this.saving.set(false);
    }
  }

  private seedFromQueryParams(): void {
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
    this.availability.reload();
  }
}
