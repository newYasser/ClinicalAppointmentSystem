import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, catchError, of, tap } from 'rxjs';
import { AppointmentApi } from '../api/appointment-api';
import { ClinicSlots } from '../models/appointment';
import { TimeOfDay } from '../models/primitives';


const FALLBACK_DURATION_MINUTES = 30;


@Injectable({ providedIn: 'root' })
export class ClinicConfig {
  private readonly api = inject(AppointmentApi);

  private readonly config = signal<ClinicSlots | null>(null);

  readonly slots = computed<readonly TimeOfDay[]>(() => this.config()?.slots ?? []);

  readonly durationMinutes = computed(
    () => this.config()?.durationMinutes ?? FALLBACK_DURATION_MINUTES,
  );

  readonly isLoaded = computed(() => this.config() !== null);

  load(): Observable<ClinicSlots | null> {
    return this.api.slots().pipe(
      tap((config) => this.config.set(config)),
      catchError(() => of(null)),
    );
  }
}
