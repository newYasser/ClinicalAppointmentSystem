import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { Session } from '../auth/session';

// Signing in cannot require a token, and a 401 from these is a rejected sign-in
// rather than an expired session.
const ANONYMOUS_PATHS = ['auth/google', 'auth/config'];

export const authTokenInterceptor: HttpInterceptorFn = (request, next) => {
  const session = inject(Session);
  const router = inject(Router);

  const isAnonymous = ANONYMOUS_PATHS.some((path) => request.url.includes(path));
  const token = session.accessToken;

  const authorized =
    token !== null && !isAnonymous
      ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
      : request;

  return next(authorized).pipe(
    catchError((error: unknown) => {
      // The token was rejected — expired, revoked, or signed with a rotated key.
      // Clearing it here means one stale token cannot produce a 401 on every
      // subsequent call for the rest of the session.
      if (error instanceof HttpErrorResponse && error.status === 401 && !isAnonymous) {
        session.signOut();
        void router.navigate(['/login'], {
          queryParams: { returnUrl: router.url },
        });
      }

      return throwError(() => error);
    }),
  );
};
