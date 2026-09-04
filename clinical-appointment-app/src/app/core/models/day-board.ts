import { AppointmentStatus } from './appointment-status';
import { IsoDate, TimeOfDay } from './primitives';

export interface DayBoardDoctor {
  id: number;
  name: string;
  specialty: string;
}


export type DayBoardCellState = 'Booked' | 'Completed' | 'Free' | 'Past';

export interface DayBoardCell {
  doctorId: number;
  state: DayBoardCellState;
  appointmentId: number | null;
  patientId: number | null;

  label: string | null;
  status: AppointmentStatus | null;
}

export interface DayBoardRow {
  startTime: TimeOfDay;
  cells: DayBoardCell[];
}

export interface DayBoard {
  date: IsoDate;
  doctors: DayBoardDoctor[];
  rows: DayBoardRow[];
}
