import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Specialty } from '../models/specialty';

@Injectable({ providedIn: 'root' })
export class SpecialtyApi {
  private readonly http = inject(HttpClient);

  list(): Observable<Specialty[]> {
    return this.http.get<Specialty[]>('specialties');
  }
}
