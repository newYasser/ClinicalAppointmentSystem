import { AfterViewInit, Component, ElementRef, inject, signal, viewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import { AuthApi } from '../../core/api/auth-api';
import { GoogleIdentity } from '../../core/auth/google-identity';
import { Session } from '../../core/auth/session';
import { ClinicConfig } from '../../core/clinic/clinic-config';
import { ApiError } from '../../core/http/api-error';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login implements AfterViewInit {
  private readonly api = inject(AuthApi);
  private readonly google = inject(GoogleIdentity);
  private readonly session = inject(Session);
  private readonly clinicConfig = inject(ClinicConfig);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  private readonly buttonHost = viewChild.required<ElementRef<HTMLElement>>('googleButton');

  protected readonly preparing = signal(true);
  protected readonly signingIn = signal(false);
  protected readonly error = signal<string | null>(null);

  async ngAfterViewInit(): Promise<void> {
    await this.prepare();
  }

  protected retry(): void {
    void this.prepare();
  }

  private async prepare(): Promise<void> {
    this.preparing.set(true);
    this.error.set(null);

    try {
      const config = await firstValueFrom(this.api.config());

      await this.google.renderButton(
        this.buttonHost().nativeElement,
        config.googleClientId,
        (idToken) => void this.completeSignIn(idToken),
      );
    } catch (error) {
      this.error.set(messageFor(error));
    } finally {
      this.preparing.set(false);
    }
  }

  private async completeSignIn(idToken: string): Promise<void> {
    this.signingIn.set(true);
    this.error.set(null);

    try {
      this.session.signIn(await firstValueFrom(this.api.signInWithGoogle(idToken)));

      // Clinic slots are behind authentication, so they can only be loaded now.
      await firstValueFrom(this.clinicConfig.load());

      const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
      await this.router.navigateByUrl(returnUrl ?? '/dashboard');
    } catch (error) {
      this.error.set(messageFor(error));
    } finally {
      this.signingIn.set(false);
    }
  }
}

function messageFor(error: unknown): string {
  if (error instanceof ApiError) {
    return error.message;
  }

  return error instanceof Error ? error.message : 'Sign-in failed. Please try again.';
}
