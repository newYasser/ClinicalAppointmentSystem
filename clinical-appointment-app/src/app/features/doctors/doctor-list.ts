import { Component } from '@angular/core';
import { PageHeader } from '../../shared/ui/page-header';

@Component({
  selector: 'app-doctor-list',
  imports: [PageHeader],
  template: `<app-page-header kicker="Staff" heading="Doctors" />`,
})
export class DoctorList {}
