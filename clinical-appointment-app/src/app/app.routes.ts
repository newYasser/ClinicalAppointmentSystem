import { Routes } from '@angular/router';


export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'dashboard',
    title: 'Dashboard',
    loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
  },
  {
    path: 'patients',
    title: 'Patients',
    loadComponent: () => import('./features/patients/patient-list').then((m) => m.PatientList),
  },
  {
    path: 'patients/:id',
    title: 'Patient record',
    loadComponent: () =>
      import('./features/patients/patient-detail').then((m) => m.PatientDetailPage),
  },
  {
    path: 'doctors',
    title: 'Doctors',
    loadComponent: () => import('./features/doctors/doctor-list').then((m) => m.DoctorList),
  },
  {
    path: 'appointments',
    title: 'Appointments',
    loadComponent: () => import('./features/appointments/appointments').then((m) => m.Appointments),
  },
  {
    path: 'appointments/new',
    title: 'New appointment',
    loadComponent: () =>
      import('./features/appointments/appointment-form').then((m) => m.AppointmentForm),
  },
  {
    path: 'appointments/:id/edit',
    title: 'Edit appointment',
    loadComponent: () =>
      import('./features/appointments/appointment-form').then((m) => m.AppointmentForm),
  },
];
