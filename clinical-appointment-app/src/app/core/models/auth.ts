export interface AuthConfig {
  googleClientId: string;
}

export interface SignedInUser {
  subject: string;
  email: string;
  displayName: string;
  pictureUrl: string | null;
}

export interface SignInResult {
  accessToken: string;
  tokenType: string;
  expiresInSeconds: number;
  expiresAtUtc: string;
  user: SignedInUser;
}
