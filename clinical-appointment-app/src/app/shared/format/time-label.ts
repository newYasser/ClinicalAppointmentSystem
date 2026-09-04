import { TimeOfDay } from '../../core/models/primitives';

export function formatTimeLabel(time: TimeOfDay | null | undefined): string {
  return time ? time.slice(0, 5) : '';
}
