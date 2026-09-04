export type ApiErrorCode =
  | 'VALIDATION_FAILED'
  | 'PATIENT_NOT_FOUND'
  | 'DOCTOR_NOT_FOUND'
  | 'APPOINTMENT_NOT_FOUND'
  | 'APPOINTMENT_IN_PAST'
  | 'DOCTOR_SLOT_CONFLICT'
  | 'PATIENT_SLOT_CONFLICT'
  | 'INVALID_STATUS_TRANSITION'
  | 'SLOT_OUT_OF_BOUNDS';


export interface ApiProblem {
  title: string;
  status: number;
  detail?: string;
  errorCode: ApiErrorCode;
  type?: string;
  instance?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
  conflictingAppointmentId?: number;
}
