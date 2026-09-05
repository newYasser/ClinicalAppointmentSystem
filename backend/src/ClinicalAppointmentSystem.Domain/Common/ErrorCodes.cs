namespace ClinicalAppointmentSystem.Domain.Common;

public static class ErrorCodes
{
    public const string ValidationFailed = "VALIDATION_FAILED";

    public const string PatientNotFound = "PATIENT_NOT_FOUND";
    public const string DoctorNotFound = "DOCTOR_NOT_FOUND";
    public const string AppointmentNotFound = "APPOINTMENT_NOT_FOUND";

    public const string AppointmentInPast = "APPOINTMENT_IN_PAST";
    public const string DoctorSlotConflict = "DOCTOR_SLOT_CONFLICT";
    public const string PatientSlotConflict = "PATIENT_SLOT_CONFLICT";
    public const string InvalidStatusTransition = "INVALID_STATUS_TRANSITION";
    public const string SlotOutOfBounds = "SLOT_OUT_OF_BOUNDS";

    public const string InvalidGoogleToken = "INVALID_GOOGLE_TOKEN";
}
