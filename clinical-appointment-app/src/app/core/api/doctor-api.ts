import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { DoctorAvailability, DoctorDetail, DoctorListItem, DoctorLookup } from '../models/doctor';
import { PagedResult } from '../models/paged-result';
import { IsoDate } from '../models/primitives';
import { PageQuery, toHttpParams } from './query';

export type DoctorSortBy = 'lastName' | 'firstName' | 'specialty' | 'appointmentCount';

export interface DoctorListQuery extends PageQuery {
 
  search?: string;
  specialtyId?: number;
  sortBy?: DoctorSortBy;
}

export interface DoctorLookupQuery {
  search?: string;
  specialtyId?: number;
}

export interface DoctorAvailabilityQuery {
  date: IsoDate;
  patientId?: number;
  excludeAppointmentId?: number;
}

@Injectable({ providedIn: 'root' })
export class DoctorApi {
  private readonly http = inject(HttpClient);


  list(query: DoctorListQuery = {}): Observable<PagedResult<DoctorListItem>> {
    return this.http.get<PagedResult<DoctorListItem>>('doctors', {
      params: toHttpParams({ ...query }),
    });
  }

  lookup(query: DoctorLookupQuery = {}): Observable<DoctorLookup[]> {
    return this.http.get<DoctorLookup[]>('doctors/lookup', {
      params: toHttpParams({ ...query }),
    });
  }

  get(id: number): Observable<DoctorDetail> {
    return this.http.get<DoctorDetail>(`doctors/${id}`);
  }

  availability(id: number, query: DoctorAvailabilityQuery): Observable<DoctorAvailability> {
    return this.http.get<DoctorAvailability>(`doctors/${id}/availability`, {
      params: toHttpParams({ ...query }),
    });
  }
}
