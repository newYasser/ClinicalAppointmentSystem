import { Component } from '@angular/core';
import { PageHeader } from '../../shared/ui/page-header';

@Component({
  selector: 'app-appointments',
  imports: [PageHeader],
  template: `<app-page-header kicker="Scheduling &middot; 30-minute slots" heading="Appointments" />`,
})
export class Appointments {}
