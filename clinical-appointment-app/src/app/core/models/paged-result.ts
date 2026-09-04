
export interface PagedResult<T> {
  items: T[];

  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  from: number;
  to: number;
}

export const PAGE_SIZES = [10, 25, 50] as const;

export type PageSize = (typeof PAGE_SIZES)[number];

export type SortDirection = 'asc' | 'desc';
