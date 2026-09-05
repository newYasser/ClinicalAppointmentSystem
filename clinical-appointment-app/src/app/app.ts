import { DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { GoogleIdentity } from './core/auth/google-identity';
import { Session } from './core/auth/session';

import { ConfirmDialog } from './shared/ui/confirm-dialog';
import { ToastHost } from './shared/ui/toast-host';

interface NavItem {
  readonly path: string;
  readonly label: string;
}

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, DatePipe, ConfirmDialog, ToastHost],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private readonly session = inject(Session);
  private readonly google = inject(GoogleIdentity);
  private readonly router = inject(Router);

  protected readonly user = this.session.user;
  protected readonly isSignedIn = this.session.isSignedIn;

  protected readonly clinicName = 'Clinic';

  protected readonly navItems: readonly NavItem[] = [
    { path: '/dashboard', label: 'Dashboard' },
    { path: '/patients', label: 'Patients' },
    { path: '/doctors', label: 'Doctors' },
    { path: '/appointments', label: 'Appointments' },
  ];

  protected readonly today = new Date();

  protected signOut(): void {
    this.session.signOut();

    // Without this, One Tap signs the same account straight back in.
    this.google.disableAutoSelect();

    void this.router.navigate(['/login']);
  }
}
