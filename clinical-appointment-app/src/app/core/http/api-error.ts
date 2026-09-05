import { HttpErrorResponse } from '@angular/common/http';
import { ApiErrorCode, ApiProblem } from '../models/api-problem';

const NETWORK_MESSAGE =
  'The server could not be reached. Check that the API is running, then try again.';
const GENERIC_MESSAGE = 'Something went wrong. Please try again.';

function isApiProblem(body: unknown): body is ApiProblem {
  return typeof body === 'object' && body !== null && 'errorCode' in body;
}

/**
 * The API reports validation failures under **PascalCase** keys (the server's
 * CLR property names). Lower-casing the first letter puts them back in the
 * casing the client sent, which is also its form control names — so a form can
 * look up `fieldError('patientId')` without knowing about the server's casing.
 */
function toCamelCaseKeys(errors: Record<string, string[]> | undefined): Record<string, string[]> {
  const normalized: Record<string, string[]> = {};

  for (const [key, messages] of Object.entries(errors ?? {})) {
    normalized[key.charAt(0).toLowerCase() + key.slice(1)] = messages;
  }

  return normalized;
}

/**
 * A failed API call, normalized. Thrown by `apiErrorInterceptor` in place of
 * Angular's `HttpErrorResponse`, so callers branch on a stable `code` instead
 * of digging through an untyped response body.
 *
 * `message` is always safe to display: the server's `detail` when there is one
 * (written to be shown verbatim), otherwise a generic fallback.
 */
export class ApiError extends Error {
  readonly status: number;
  readonly code: ApiErrorCode | null;
  readonly problem: ApiProblem | null;
  readonly fieldErrors: Readonly<Record<string, string[]>>;
  readonly conflictingAppointmentId: number | null;

  constructor(response: HttpErrorResponse) {
    const problem = isApiProblem(response.error) ? response.error : null;

    super(
      problem?.detail ??
        problem?.title ??
        (response.status === 0 ? NETWORK_MESSAGE : GENERIC_MESSAGE),
    );

    this.name = 'ApiError';
    this.status = response.status;
    this.problem = problem;
    this.code = problem?.errorCode ?? null;
    this.fieldErrors = toCamelCaseKeys(problem?.errors);
    this.conflictingAppointmentId = problem?.conflictingAppointmentId ?? null;
  }

  is(...codes: ApiErrorCode[]): boolean {
    return this.code !== null && codes.includes(this.code);
  }

  fieldError(field: string): string | null {
    return this.fieldErrors[field]?.[0] ?? null;
  }

  get isOffline(): boolean {
    return this.status === 0;
  }

  get isUnauthorized(): boolean {
    return this.status === 401;
  }

  get isNotFound(): boolean {
    return this.status === 404;
  }

  get isConflict(): boolean {
    return this.status === 409;
  }
}
