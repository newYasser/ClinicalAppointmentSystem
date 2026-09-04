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
import { apiBaseUrlInterceptor } from './core/http/api-base-url.interceptor';
import { apiErrorInterceptor } from './core/http/api-error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(
      withFetch(),
      withInterceptors([apiErrorInterceptor, apiBaseUrlInterceptor]),
    ),
    provideAppInitializer(() => inject(ClinicConfig).load()),
  ],
};
