import { Component } from '@angular/core';
import { PageHeader } from '../../shared/ui/page-header';

@Component({
  selector: 'app-patient-list',
  imports: [PageHeader],
  template: `<app-page-header kicker="Records" heading="Patients" />`,
})
export class PatientList {}
