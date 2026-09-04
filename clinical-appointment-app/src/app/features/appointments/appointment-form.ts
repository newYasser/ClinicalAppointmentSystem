import { Component } from '@angular/core';
import { PageHeader } from '../../shared/ui/page-header';

@Component({
  selector: 'app-appointment-form',
  imports: [PageHeader],
  template: `<app-page-header heading="New appointment" />`,
})
export class AppointmentForm {}
