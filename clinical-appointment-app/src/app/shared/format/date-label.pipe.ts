import { Pipe, PipeTransform } from '@angular/core';
import { IsoDate } from '../../core/models/primitives';

const WEEKDAYS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
const MONTHS = [
  'Jan',
  'Feb',
  'Mar',
  'Apr',
  'May',
  'Jun',
  'Jul',
  'Aug',
  'Sep',
  'Oct',
  'Nov',
  'Dec',
];

@Pipe({ name: 'dateLabel' })
export class DateLabelPipe implements PipeTransform {
  transform(date: IsoDate | null | undefined): string {
    if (!date) {
      return '—';
    }

    const parsed = new Date(`${date}T12:00:00`);

    if (Number.isNaN(parsed.getTime())) {
      return '—';
    }

    return `${WEEKDAYS[parsed.getDay()]} ${parsed.getDate()} ${MONTHS[parsed.getMonth()]} ${parsed.getFullYear()}`;
  }
}
