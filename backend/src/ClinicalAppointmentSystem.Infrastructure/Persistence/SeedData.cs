namespace ClinicalAppointmentSystem.Infrastructure.Persistence;

// Rows are emitted as anonymous objects for HasData, which needs no constructor or
// public setters on the entities.
//
// Everything here must be deterministic. A DateTime.UtcNow in the seed would change the
// model on every run and leave `dotnet ef` reporting a pending model change forever, so
// the audit stamps are a fixed constant.
internal static class SeedData
{
    private static readonly DateTime Timestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static readonly string[] SpecialtyNames =
    [
        "Cardiology", "Dermatology", "Endocrinology", "General Practice", "Neurology",
        "Oncology", "Orthopedics", "Pediatrics", "Psychiatry", "Radiology",
    ];

    private static readonly string[] FirstNames =
    [
        "Anna", "Lukas", "Mira", "Jonas", "Elif", "Tobias", "Greta", "Samir", "Nora", "Felix",
        "Clara", "Benno", "Ines", "Ravi", "Katja", "Milan", "Sofia", "Hendrik", "Lena", "Oskar",
    ];

    private static readonly string[] LastNames =
    [
        "Keller", "Brandt", "Vogel", "Hoffmann", "Ackermann", "Dietrich", "Sommer", "Kraus",
        "Neumann", "Bergmann", "Winkler", "Roth", "Lindner", "Fuchs", "Marek", "Schuster",
        "Haas", "Wendt", "Beckmann", "Kluge",
    ];

    public static object[] Specialties =>
    [
        .. SpecialtyNames.Select((name, i) => new { Id = i + 1, Name = name }),
    ];

    public static object[] Doctors =>
    [
        .. Enumerable.Range(0, 20).Select(i => new
        {
            Id = i + 1,
            FirstName = FirstNames[i],

            // 7 and 20 are coprime, so every doctor gets a distinct surname.
            LastName = LastNames[i * 7 % LastNames.Length],
            SpecialtyId = (i % SpecialtyNames.Length) + 1,
            CreatedAt = Timestamp,
            UpdatedAt = Timestamp,
        }),
    ];

    public static object[] Patients =>
    [
        .. Enumerable.Range(0, 40).Select(i =>
        {
            var first = FirstNames[i % FirstNames.Length];
            var last = LastNames[(i * 3 + 5) % LastNames.Length];

            return new
            {
                Id = i + 1,
                FirstName = first,
                LastName = last,
                DateOfBirth = new DateOnly(
                    1950 + (i * 37 % 55),
                    (i % 12) + 1,
                    (i * 7 % 28) + 1),
                Phone = $"010 {1000000 + (i * 20347)}",
                Email = $"{first}.{last}{i + 1}@example.com".ToLowerInvariant(),
                CreatedAt = Timestamp,
                UpdatedAt = Timestamp,
            };
        }),
    ];
}
