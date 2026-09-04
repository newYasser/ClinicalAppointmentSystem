import { Component } from '@angular/core';
import { PageHeader } from '../../shared/ui/page-header';

@Component({
  selector: 'app-dashboard',
  imports: [PageHeader],
  template: `<app-page-header kicker="Overview" heading="Clinic at a glance" />`,
})
export class Dashboard {}
