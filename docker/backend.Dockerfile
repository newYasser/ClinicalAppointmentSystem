
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/Directory.Build.props backend/Directory.Packages.props backend/nuget.config* ./
COPY backend/src/ClinicalAppointmentSystem.Domain/*.csproj src/ClinicalAppointmentSystem.Domain/
COPY backend/src/ClinicalAppointmentSystem.Application/*.csproj src/ClinicalAppointmentSystem.Application/
COPY backend/src/ClinicalAppointmentSystem.Infrastructure/*.csproj src/ClinicalAppointmentSystem.Infrastructure/
COPY backend/src/ClinicalAppointmentSystem.Api/*.csproj src/ClinicalAppointmentSystem.Api/
RUN dotnet restore src/ClinicalAppointmentSystem.Api/ClinicalAppointmentSystem.Api.csproj

COPY backend/src/ src/
RUN dotnet publish src/ClinicalAppointmentSystem.Api/ClinicalAppointmentSystem.Api.csproj \
    -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

RUN apt-get update \
    && apt-get install -y --no-install-recommends tzdata \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "ClinicalAppointmentSystem.Api.dll"]
