import { Component } from '@angular/core';
import { PageHeader } from '../../shared/ui/page-header';

@Component({
  selector: 'app-patient-detail',
  imports: [PageHeader],
  template: `<app-page-header kicker="Patient record" heading="Patient" />`,
})
export class PatientDetailPage {}
