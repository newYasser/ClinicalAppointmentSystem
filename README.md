# ClinicalAppointmentSystem

Appointment scheduling for a small clinic: a register of patients and doctors, a
booking flow with conflict checking, and a day board showing every doctor's slots
for a given date. Sign-in is Google SSO.

An appointment is a fixed 30-minute slot between 08:00 and 17:30. The system refuses
a booking when the slot is in the past, when the doctor is already busy at that time,
or when the patient already has another appointment then. A booked appointment can be
completed or cancelled; cancelling frees the slot for rebooking, completing does not.

## Stack

| Part     | Built with                                                   |
| -------- | ------------------------------------------------------------ |
| Frontend | Angular 21, standalone components, signals                    |
| Backend  | ASP.NET Core (.NET 10), layered Domain/Application/Infrastructure/Api |
| Data     | MySQL 8.4 via EF Core 9 (Pomelo)                              |
| Auth     | Google Identity Services → API-issued JWT bearer tokens       |

## Running with Docker

The only prerequisite is Docker. Nothing else needs installing — the .NET SDK and
Node both run inside the build images.

```bash
cd docker
cp .env.example .env
```

Fill in `.env`:

| Variable                              | Value                                                        |
| ------------------------------------- | ------------------------------------------------------------ |
| `MYSQL_ROOT_PASSWORD`, `MYSQL_PASSWORD` | Anything — the database is a local container                 |
| `JWT_SIGNING_KEY`                     | Your own, 32 bytes or more: `openssl rand -base64 48`         |
| `GOOGLE_CLIENT_ID`                    | From the Google Cloud Console (see below)                     |
| `GOOGLE_CLIENT_SECRET`                | Leave empty — this sign-in flow never uses it                 |

Then:

```bash
docker compose up -d --build
```

The app is on <http://localhost:8080>. The API is published on `:7001` and MySQL on
`:7000` for direct access while debugging; change any of them in `.env` if they clash.

The API container applies EF migrations on startup, so the schema and its seed data —
40 patients, 20 doctors, 10 specialties — appear on first run. Appointments start
empty; book them through the app.

`docker compose logs -f api` to follow the API, `docker compose down` to stop, and
`docker compose down -v` to also drop the database volume and start clean.

## Google sign-in setup

Sign-in needs an OAuth client, and one client works for every developer on the project.

In the Google Cloud Console, under **APIs & Services → Credentials**, create an
**OAuth client ID** of type **Web application**. Add the origins the app is served
from under **Authorized JavaScript origins**:

```
http://localhost:8080     Docker
http://localhost:4200     ng serve
```

Under **Authorized redirect URIs**, add:

```
http://localhost:8080/dashboard
```

While the consent screen is in Testing mode, each person signing in must be listed
under **OAuth consent screen → Test users**.

Copy the client ID into `GOOGLE_CLIENT_ID`. It is public by design and reaches the
browser either way, so it is safe to share with the team. The **client secret is not**,
and this application never reads it.

## Running without Docker

Needs the .NET 10 SDK, Node 20+, and a MySQL instance — `docker compose up -d mysql`
gives you one on `:7000`.

Configuration lives in user-secrets, never in `appsettings.json`:

```bash
cd backend/src/ClinicalAppointmentSystem.Api
dotnet user-secrets set "ConnectionStrings:ClinicDb" "Server=localhost;Port=7000;Database=clinical_appointments;User Id=clinic;Password=<your password>;"
dotnet user-secrets set "Authentication:Jwt:SigningKey" "<openssl rand -base64 48>"
dotnet user-secrets set "Authentication:Google:ClientId" "<id>.apps.googleusercontent.com"
```

Create the schema, then run both halves:

```bash
cd backend
dotnet tool restore
dotnet ef database update --project src/ClinicalAppointmentSystem.Infrastructure --startup-project src/ClinicalAppointmentSystem.Api

dotnet run --project src/ClinicalAppointmentSystem.Api          # http://localhost:5288
cd ../clinical-appointment-app && npm install && npm start      # http://localhost:4200
```

`ng serve` proxies `/api` to `:5288`, so the client calls the same relative paths it
does in Docker.
