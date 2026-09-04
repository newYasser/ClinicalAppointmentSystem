import { Pipe, PipeTransform } from '@angular/core';
import { TimeOfDay } from '../../core/models/primitives';

@Pipe({ name: 'timeLabel' })
export class TimeLabelPipe implements PipeTransform {
  transform(time: TimeOfDay | null | undefined): string {
    return time ? time.slice(0, 5) : '';
  }
}
