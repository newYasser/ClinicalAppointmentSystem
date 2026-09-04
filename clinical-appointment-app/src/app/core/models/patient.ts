import { AppointmentListItem } from './appointment';
import { IsoDate, UtcTimestamp } from './primitives';

export interface PatientListItem {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  dateOfBirth: IsoDate;
  phone: string;
  email: string;
  appointmentCount: number;
}

export interface PatientDetail extends PatientListItem {
  createdAt: UtcTimestamp;
  updatedAt: UtcTimestamp;
  appointments: AppointmentListItem[];
}

export interface PatientLookup {
  id: number;
  label: string;
  dateOfBirth: IsoDate;
}
