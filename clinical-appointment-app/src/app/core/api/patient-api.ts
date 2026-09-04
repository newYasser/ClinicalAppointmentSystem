import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AppointmentListItem } from '../models/appointment';
import { AppointmentStatus } from '../models/appointment-status';
import { PagedResult } from '../models/paged-result';
import { PatientDetail, PatientListItem, PatientLookup } from '../models/patient';
import { PageQuery, toHttpParams } from './query';

export type PatientSortBy = 'lastName' | 'firstName' | 'dateOfBirth' | 'appointmentCount';

export interface PatientListQuery extends PageQuery {
  search?: string;
  sortBy?: PatientSortBy;
}

export interface PatientLookupQuery {
  /** Names only*/
  search?: string;
  limit?: number;
}

export interface PatientAppointmentsQuery extends PageQuery {
  status?: AppointmentStatus;
}

@Injectable({ providedIn: 'root' })
export class PatientApi {
  private readonly http = inject(HttpClient);

  list(query: PatientListQuery = {}): Observable<PagedResult<PatientListItem>> {
    return this.http.get<PagedResult<PatientListItem>>('patients', {
      params: toHttpParams({ ...query }),
    });
  }

  lookup(query: PatientLookupQuery = {}): Observable<PatientLookup[]> {
    return this.http.get<PatientLookup[]>('patients/lookup', {
      params: toHttpParams({ ...query }),
    });
  }

  get(id: number): Observable<PatientDetail> {
    return this.http.get<PatientDetail>(`patients/${id}`);
  }

  appointments(
    id: number,
    query: PatientAppointmentsQuery = {},
  ): Observable<PagedResult<AppointmentListItem>> {
    return this.http.get<PagedResult<AppointmentListItem>>(`patients/${id}/appointments`, {
      params: toHttpParams({ ...query }),
    });
  }
}
