import { Injectable, computed, signal } from '@angular/core';
import { SignInResult, SignedInUser } from '../models/auth';

const STORAGE_KEY = 'clinic.session';

interface StoredSession {
  readonly accessToken: string;
  readonly expiresAtUtc: string;
  readonly user: SignedInUser;
}

/**
 * The signed-in user and the token sent with every API call.
 *
 * Persisted to `localStorage` so a page refresh does not sign the user out. The
 * token is the only credential the client holds, and it is readable by any script
 * on this origin — an httpOnly cookie would not be, which is the trade made when
 * the API hands the SPA a bearer token.
 */
@Injectable({ providedIn: 'root' })
export class Session {
  private readonly state = signal<StoredSession | null>(restore());

  readonly user = computed<SignedInUser | null>(() => this.state()?.user ?? null);

  readonly isSignedIn = computed(() => this.state() !== null);

  get accessToken(): string | null {
    return this.state()?.accessToken ?? null;
  }

  signIn(result: SignInResult): void {
    const session: StoredSession = {
      accessToken: result.accessToken,
      expiresAtUtc: result.expiresAtUtc,
      user: result.user,
    };

    this.state.set(session);
    write(session);
  }

  signOut(): void {
    this.state.set(null);
    write(null);
  }
}

function restore(): StoredSession | null {
  let raw: string | null = null;

  try {
    raw = localStorage.getItem(STORAGE_KEY);
  } catch {
    // Private browsing and blocked site data both throw on access.
    return null;
  }

  if (raw === null) {
    return null;
  }

  try {
    const session = JSON.parse(raw) as StoredSession;

    // A stored token past its expiry is already refused by the API, so treating it
    // as no session sends the user to sign in rather than through a failed call.
    if (!session.accessToken || Date.parse(session.expiresAtUtc) <= Date.now()) {
      write(null);
      return null;
    }

    return session;
  } catch {
    write(null);
    return null;
  }
}

function write(session: StoredSession | null): void {
  try {
    if (session === null) {
      localStorage.removeItem(STORAGE_KEY);
    } else {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
    }
  } catch {
    // Persistence is a convenience; the in-memory signal still drives the app.
  }
}
