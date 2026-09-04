import { Pipe, PipeTransform } from '@angular/core';
import { IsoDate } from '../../core/models/primitives';
import { formatDateLabel } from './date-label';

@Pipe({ name: 'dateLabel' })
export class DateLabelPipe implements PipeTransform {
  transform(date: IsoDate | null | undefined): string {
    return formatDateLabel(date);
  }
}
