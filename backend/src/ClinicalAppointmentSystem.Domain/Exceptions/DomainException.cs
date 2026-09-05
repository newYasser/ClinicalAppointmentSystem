using ClinicalAppointmentSystem.Domain.Common;

namespace ClinicalAppointmentSystem.Domain.Exceptions;

public abstract class DomainException(string errorCode, string message) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;

    public Dictionary<string, object?> Extensions { get; } = [];

    public DomainException With(string key, object? value)
    {
        Extensions[key] = value;
        return this;
    }
}

public sealed class NotFoundException(string errorCode, string message)
    : DomainException(errorCode, message);

public sealed class ConflictException(string errorCode, string message)
    : DomainException(errorCode, message);

public sealed class UnauthorizedException(string errorCode, string message)
    : DomainException(errorCode, message);

public sealed class DomainValidationException(string errorCode, string message)
    : DomainException(errorCode, message)
{
    public Dictionary<string, string[]> Errors { get; } = [];

    public static DomainValidationException ForField(string field, string message)
    {
        var exception = new DomainValidationException(ErrorCodes.ValidationFailed, message);
        exception.Errors[field] = [message];
        return exception;
    }
}
