import { AppointmentStatus } from './appointment-status';
import { IsoDate, TimeOfDay, UtcTimestamp } from './primitives';

export interface AppointmentListItem {
  id: number;
  date: IsoDate;
  startTime: TimeOfDay;
  endTime: TimeOfDay;
  durationMinutes: number;
  status: AppointmentStatus;
  notes: string | null;
  patientId: number;
  patientName: string;
  doctorId: number;
  doctorName: string;
  specialty: string;
  canComplete: boolean;
  canCancel: boolean;
  isPast: boolean;
}


export interface AppointmentDetail extends AppointmentListItem {
  createdAt: UtcTimestamp;
  updatedAt: UtcTimestamp;
}


export interface AppointmentRequest {
  patientId: number;
  doctorId: number;
  date: IsoDate;

  startTime: TimeOfDay;
  notes: string | null;
}


export interface ClinicSlots {
  durationMinutes: number;

  slots: TimeOfDay[];
}
