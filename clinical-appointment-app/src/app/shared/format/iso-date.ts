import { IsoDate } from '../../core/models/primitives';

const ISO_DATE = /^\d{4}-\d{2}-\d{2}$/;

export function toIsoDate(date: Date): IsoDate {
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');

  return `${date.getFullYear()}-${month}-${day}`;
}

export function todayIso(): IsoDate {
  return toIsoDate(new Date());
}

export function shiftIsoDate(date: IsoDate, days: number): IsoDate {
  const shifted = new Date(`${date}T12:00:00`);
  shifted.setDate(shifted.getDate() + days);

  return toIsoDate(shifted);
}

export function isIsoDate(value: string | null | undefined): value is IsoDate {
  return typeof value === 'string' && ISO_DATE.test(value);
}
