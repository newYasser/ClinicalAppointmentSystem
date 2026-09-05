import { Routes } from '@angular/router';

import { authGuard, guestGuard } from './core/auth/auth-guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'login',
    title: 'Sign in',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/login').then((m) => m.Login),
  },
  {
    path: 'dashboard',
    title: 'Dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
  },
  {
    path: 'patients',
    title: 'Patients',
    canActivate: [authGuard],
    loadComponent: () => import('./features/patients/patient-list').then((m) => m.PatientList),
  },
  {
    path: 'patients/:id',
    title: 'Patient record',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/patients/patient-detail').then((m) => m.PatientDetailPage),
  },
  {
    path: 'doctors',
    title: 'Doctors',
    canActivate: [authGuard],
    loadComponent: () => import('./features/doctors/doctor-list').then((m) => m.DoctorList),
  },
  {
    path: 'appointments',
    title: 'Appointments',
    canActivate: [authGuard],
    loadComponent: () => import('./features/appointments/appointments').then((m) => m.Appointments),
  },
  {
    path: 'appointments/new',
    title: 'New appointment',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/appointments/appointment-form').then((m) => m.AppointmentForm),
  },
  {
    path: 'appointments/:id/edit',
    title: 'Edit appointment',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/appointments/appointment-form').then((m) => m.AppointmentForm),
  },
];
