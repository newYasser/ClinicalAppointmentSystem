using System.Linq.Expressions;
using ClinicalAppointmentSystem.Domain.Entities;

namespace ClinicalAppointmentSystem.Application.Appointments;

public static class AppointmentSpecifications
{
    public static Expression<Func<Appointment, bool>> ClashingWith(
        DateTime slot,
        int doctorId,
        int patientId,
        int? excludeAppointmentId = null) =>
        appointment =>
            appointment.ActiveSlot == slot
            && (appointment.DoctorId == doctorId || appointment.PatientId == patientId)
            && (excludeAppointmentId == null || appointment.Id != excludeAppointmentId);

    public static Expression<Func<Appointment, bool>> LiveBetween(
        DateTime fromInclusive,
        DateTime toExclusive) =>
        appointment =>
            appointment.ActiveSlot >= fromInclusive
            && appointment.ActiveSlot < toExclusive;
}
