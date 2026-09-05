import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import {
  ApplicationConfig,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  inject,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { ClinicConfig } from './core/clinic/clinic-config';
import { Session } from './core/auth/session';
import { apiBaseUrlInterceptor } from './core/http/api-base-url.interceptor';
import { apiErrorInterceptor } from './core/http/api-error.interceptor';
import { authTokenInterceptor } from './core/http/auth-token.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(
      withFetch(),
      withInterceptors([apiErrorInterceptor, authTokenInterceptor, apiBaseUrlInterceptor]),
    ),
    // Clinic slots sit behind authentication, so an anonymous boot would only
    // earn a 401. A returning user still has a token, so their config loads here;
    // a fresh sign-in loads it from the login page instead.
    provideAppInitializer(() => {
      if (inject(Session).isSignedIn()) {
        inject(ClinicConfig).load().subscribe();
      }
    }),
  ],
};
