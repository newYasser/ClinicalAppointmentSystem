import { Pipe, PipeTransform } from '@angular/core';
import { TimeOfDay } from '../../core/models/primitives';
import { formatTimeLabel } from './time-label';

@Pipe({ name: 'timeLabel' })
export class TimeLabelPipe implements PipeTransform {
  transform(time: TimeOfDay | null | undefined): string {
    return formatTimeLabel(time);
  }
}
