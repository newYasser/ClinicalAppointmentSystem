import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthConfig, SignInResult } from '../models/auth';

@Injectable({ providedIn: 'root' })
export class AuthApi {
  private readonly http = inject(HttpClient);

  config(): Observable<AuthConfig> {
    return this.http.get<AuthConfig>('auth/config');
  }

  signInWithGoogle(idToken: string): Observable<SignInResult> {
    return this.http.post<SignInResult>('auth/google', { idToken });
  }
}
