import { HttpParams } from '@angular/common/http';
import { PageSize, SortDirection } from '../models/paged-result';

export interface PageQuery {
  page?: number;
  pageSize?: PageSize;
  sortDir?: SortDirection;
}

type QueryValue = string | number | boolean | null | undefined;


export function toHttpParams(query: Record<string, QueryValue>): HttpParams {
  let params = new HttpParams();

  for (const [key, raw] of Object.entries(query)) {
    if (raw === null || raw === undefined) {
      continue;
    }

    const value = typeof raw === 'string' ? raw.trim() : String(raw);

    if (value !== '') {
      params = params.set(key, value);
    }
  }

  return params;
}
