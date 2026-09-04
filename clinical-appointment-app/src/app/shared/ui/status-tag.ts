import { Component, computed, input } from '@angular/core';
import { AppointmentStatus } from '../../core/models/appointment-status';

const TAG_CLASS: Record<AppointmentStatus, string> = {
  Scheduled: 'tag-accent',
  Completed: 'tag-neutral',
  Cancelled: 'tag-outline',
};

@Component({
  selector: 'app-status-tag',
  template: `{{ status() }}`,
  host: { '[class]': 'classes()' },
})
export class StatusTag {
  readonly status = input.required<AppointmentStatus>();

  protected readonly classes = computed(() => `tag ${TAG_CLASS[this.status()]}`);
}
