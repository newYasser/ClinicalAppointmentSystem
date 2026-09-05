import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Session } from './session';

/**
 * Keeps unauthenticated users out of the app shell. This is navigation only —
 * the API enforces access on every request, and is the boundary that matters.
 */
export const authGuard: CanActivateFn = (_route, state) => {
  const session = inject(Session);
  const router = inject(Router);

  if (session.isSignedIn()) {
    return true;
  }

  return router.createUrlTree(['/login'], {
    queryParams: { returnUrl: state.url },
  });
};

/** Keeps a signed-in user off the login page. */
export const guestGuard: CanActivateFn = () => {
  const session = inject(Session);
  const router = inject(Router);

  return session.isSignedIn() ? router.createUrlTree(['/dashboard']) : true;
};
