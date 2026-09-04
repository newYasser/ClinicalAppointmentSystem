import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  AppointmentDetail,
  AppointmentListItem,
  AppointmentRequest,
  ClinicSlots,
} from '../models/appointment';
import { AppointmentStatus } from '../models/appointment-status';
import { DayBoard } from '../models/day-board';
import { PagedResult } from '../models/paged-result';
import { IsoDate } from '../models/primitives';
import { PageQuery, toHttpParams } from './query';

export type AppointmentSortBy = 'scheduledAt' | 'patientName' | 'doctorName' | 'status';

export interface AppointmentListQuery extends PageQuery {
  search?: string;
  from?: IsoDate;
  to?: IsoDate;
  doctorId?: number;
  patientId?: number;
  status?: AppointmentStatus;
  sortBy?: AppointmentSortBy;
}

export interface DayBoardQuery {
  date: IsoDate;
  specialtyId?: number;
  doctorId?: number;
}

@Injectable({ providedIn: 'root' })
export class AppointmentApi {
  private readonly http = inject(HttpClient);


  list(query: AppointmentListQuery = {}): Observable<PagedResult<AppointmentListItem>> {
    return this.http.get<PagedResult<AppointmentListItem>>('appointments', {
      params: toHttpParams({ ...query }),
    });
  }

  slots(): Observable<ClinicSlots> {
    return this.http.get<ClinicSlots>('appointments/slots');
  }

  dayBoard(query: DayBoardQuery): Observable<DayBoard> {
    return this.http.get<DayBoard>('appointments/day-board', {
      params: toHttpParams({ ...query }),
    });
  }

  get(id: number): Observable<AppointmentDetail> {
    return this.http.get<AppointmentDetail>(`appointments/${id}`);
  }

  create(request: AppointmentRequest): Observable<AppointmentDetail> {
    return this.http.post<AppointmentDetail>('appointments', request);
  }

  update(id: number, request: AppointmentRequest): Observable<AppointmentDetail> {
    return this.http.put<AppointmentDetail>(`appointments/${id}`, request);
  }


  remove(id: number): Observable<void> {
    return this.http.delete<void>(`appointments/${id}`);
  }

  cancel(id: number): Observable<AppointmentDetail> {
    return this.http.post<AppointmentDetail>(`appointments/${id}/cancel`, null);
  }

  complete(id: number): Observable<AppointmentDetail> {
    return this.http.post<AppointmentDetail>(`appointments/${id}/complete`, null);
  }
}
