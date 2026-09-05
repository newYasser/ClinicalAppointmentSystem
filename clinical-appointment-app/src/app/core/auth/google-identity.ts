import { Injectable } from '@angular/core';

interface GoogleCredentialResponse {
  credential: string;
}

interface GoogleButtonOptions {
  type?: 'standard' | 'icon';
  theme?: 'outline' | 'filled_blue' | 'filled_black';
  size?: 'small' | 'medium' | 'large';
  text?: 'signin_with' | 'signup_with' | 'continue_with';
  shape?: 'rectangular' | 'pill';
  width?: number;
}

interface GoogleAccountsId {
  initialize(config: {
    client_id: string;
    callback: (response: GoogleCredentialResponse) => void;
    auto_select?: boolean;
    cancel_on_tap_outside?: boolean;
  }): void;
  renderButton(parent: HTMLElement, options: GoogleButtonOptions): void;
  disableAutoSelect(): void;
}

declare global {
  interface Window {
    google?: { accounts: { id: GoogleAccountsId } };
  }
}

const SCRIPT_URL = 'https://accounts.google.com/gsi/client';
const SCRIPT_TIMEOUT_MS = 10_000;

/**
 * Wraps Google Identity Services, which is a global script rather than a module.
 *
 * The script is loaded on demand rather than from `index.html` so that a signed-in
 * user never pays for it, and so a blocked or offline load surfaces as a rejected
 * promise the login page can render — instead of a button that silently never appears.
 */
@Injectable({ providedIn: 'root' })
export class GoogleIdentity {
  private loading: Promise<GoogleAccountsId> | null = null;

  renderButton(
    parent: HTMLElement,
    clientId: string,
    onCredential: (idToken: string) => void,
  ): Promise<void> {
    return this.load().then((accounts) => {
      accounts.initialize({
        client_id: clientId,
        callback: (response) => onCredential(response.credential),
        cancel_on_tap_outside: true,
      });

      accounts.renderButton(parent, {
        type: 'standard',
        theme: 'outline',
        size: 'large',
        text: 'continue_with',
        shape: 'rectangular',
      });
    });
  }

  /** Stops One Tap from signing the user straight back in after they sign out. */
  disableAutoSelect(): void {
    window.google?.accounts.id.disableAutoSelect();
  }

  private load(): Promise<GoogleAccountsId> {
    if (this.loading !== null) {
      return this.loading;
    }

    this.loading = new Promise<GoogleAccountsId>((resolve, reject) => {
      const existing = window.google?.accounts.id;
      if (existing) {
        resolve(existing);
        return;
      }

      const script = document.createElement('script');
      script.src = SCRIPT_URL;
      script.async = true;
      script.defer = true;

      const timer = setTimeout(() => {
        reject(new Error('Google sign-in did not load. Check your connection and try again.'));
      }, SCRIPT_TIMEOUT_MS);

      script.onload = () => {
        clearTimeout(timer);
        const accounts = window.google?.accounts.id;

        if (accounts) {
          resolve(accounts);
        } else {
          reject(new Error('Google sign-in loaded but is unavailable.'));
        }
      };

      script.onerror = () => {
        clearTimeout(timer);
        // Let a later attempt retry rather than caching the failure forever.
        this.loading = null;
        reject(new Error('Google sign-in could not be reached. Check your connection.'));
      };

      document.head.appendChild(script);
    });

    return this.loading;
  }
}
