import { AppointmentStatus } from './appointment-status';
import { IsoDate, TimeOfDay } from './primitives';


export interface TodayScheduleItem {
  id: number;
  startTime: TimeOfDay;
  patientId: number;
  patientName: string;
  doctorId: number;
  doctorName: string;
  specialty: string;
  status: AppointmentStatus;
}

export interface DashboardSummary {
  totalPatients: number;
  totalDoctors: number;
  todayAppointmentCount: number;
  upcomingAppointmentCount: number;
  specialtyCount: number;
  today: IsoDate;
  todaySchedule: TodayScheduleItem[];
}
