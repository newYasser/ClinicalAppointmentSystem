import { IsoDate, TimeOfDay, UtcTimestamp } from './primitives';

export interface DoctorListItem {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  specialtyId: number;
  specialty: string;
  appointmentCount: number;
}

export interface DoctorDetail extends DoctorListItem {
  createdAt: UtcTimestamp;
  updatedAt: UtcTimestamp;
}

export interface DoctorLookup {
  id: number;
  label: string;
  specialtyId: number;
  specialty: string;
}

export type AvailabilitySlotState = 'Free' | 'Past' | 'TakenByDoctor' | 'TakenByPatient';

export interface AvailabilitySlot {
  startTime: TimeOfDay;
  state: AvailabilitySlotState;
  appointmentId: number | null;
}

export interface DoctorAvailability {
  doctorId: number;
  doctorName: string;
  date: IsoDate;
  slots: AvailabilitySlot[];
}
