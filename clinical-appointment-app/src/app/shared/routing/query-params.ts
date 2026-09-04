import { PAGE_SIZES, PageSize } from '../../core/models/paged-result';

const DEFAULT_PAGE_SIZE: PageSize = 10;

export function readPage(raw: string | null): number {
  const page = Number(raw);
  return Number.isInteger(page) && page > 0 ? page : 1;
}

export function readPageSize(raw: string | null): PageSize {
  const size = Number(raw);
  return PAGE_SIZES.includes(size as PageSize) ? (size as PageSize) : DEFAULT_PAGE_SIZE;
}

export function readText(raw: string | null): string {
  return raw?.trim() ?? '';
}
