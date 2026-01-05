# GeoPlaces SPA

A location-aware Single Page Application (SPA) built with **Angular** and **ASP.NET Core**, backed by **PostgreSQL + PostGIS**.

This repository demonstrates how to build a spatially-enabled web application with a **consolidated developer experience**: one backend, one frontend, one deployment unit.

---

##  Features

-  Location-aware browsing of places
-  Nearby search based on user location
-  Adjustable search radius (meters)
-  Deterministic localhost behavior (NYC fallback)
-  Clean API contracts (DTO-based, no EF leakage)
-  Spatial logic handled server-side
-  No CORS or proxy configuration needed

---

##  Architecture Overview

```
Browser
  |
  |  Angular SPA (static assets)
  |
ASP.NET Core Web Host
  |-- /api/*          → REST API
  |-- /*              → Angular SPA
  |
  |-- EF Core + Dapper
  |-- PostGIS (spatial queries)
  |
PostgreSQL (PostGIS enabled)
```

### Design Goals

- **Single-host model** – API and SPA are served together
- **Environment parity** – same behavior in dev, CI, and prod
- **DTO-first contracts** – EF entities stay internal
- **Backend-owned correctness** – frontend consumes primitives only

---

##  Tech Stack

### Backend
- ASP.NET Core (.NET 8)
- Entity Framework Core
- Dapper
- PostgreSQL
- PostGIS
- NetTopologySuite

### Frontend
- Angular (standalone components)
- Angular HttpClient
- FormsModule

### Infrastructure
- Docker / Docker Compose
- Static file hosting via ASP.NET Core

---

## Project Structure (Simplified)

```
GeoPlaces/
│
├── backend/
│   │
│   ├── GeoPlaces.Api/
│   │   ├── Controllers/
│   │   │   ├── PlacesController.cs
│   │   │   └── LocationController.cs
│   │   ├── Middleware/
│   │   │   └── GlobalExceptionHandler.cs
│   │   ├── Program.cs
│   │   └── GeoPlaces.Api.csproj
│   │
│   ├── GeoPlaces.Application/
│   │   ├── Abstractions/
│   │   │   ├── IPlacesService.cs
│   │   │   ├── IPlaceRepository.cs
│   │   │   ├── IPlaceSpatialRepository.cs
│   │   │   └── Idempotency/
│   │   │       └── IIdempotencyStore.cs
│   │   ├── Places/
│   │   │   └── PlacesService.cs
│   │   ├── Validation/
│   │   │   └── PlaceValidators.cs
│   │   └── GeoPlaces.Application.csproj
│   │
│   ├── GeoPlaces.Domain/
│   │   ├── Entities/
│   │   │   └── Place.cs
│   │   ├── ValueObjects/
│   │   │   └── GeoPoint.cs
│   │   └── Common/
│   │       └── PagedItems.cs
│   │
│   ├── GeoPlaces.Contracts/
│   │   ├── Places/
│   │   │   ├── PlaceDto.cs
│   │   │   ├── NearbyPlaceDto.cs
│   │   │   └── CreatePlaceRequest.cs
│   │   └── Location/
│   │       └── MyLocationDto.cs
│   │
│   ├── GeoPlaces.Data/
│   │   ├── Db/
│   │   │   └── PlacesDbContext.cs
│   │   ├── Repositories/
│   │   │   ├── PlaceRepositoryEf.cs
│   │   │   └── PlaceSpatialRepositoryDapper.cs
│   │   ├── Idempotency/
│   │   │   └── IdempotencyStoreDapper.cs
│   │   └── GeoPlaces.Data.csproj
│   │
│   ├── GeoPlaces.Infrastructure/
│   │   ├── GeoIp/
│   │   │   └── GeoIpService.cs
│   │   ├── Paging/
│   │   │   └── PagingHeaders.cs
│   │   └── GeoPlaces.Infrastructure.csproj
│   │
│   └── GeoPlaces.Tests/
│       ├── Places/
│       │   └── PlacesServiceTests.cs
│       └── GeoPlaces.Tests.csproj
│
├── frontend/
│   │
│   └── clientapp/
│       ├── src/
│       │   ├── app/
│       │   │   ├── app.component.ts
│       │   │   ├── app.component.html
│       │   │   ├── places.service.ts
│       │   │   └── location.service.ts
│       │   └── main.ts
│       ├── angular.json
│       └── package.json
│
├── docker-compose.yml
├── README.md
└── GeoPlaces.sln

```

---

## Getting Started

### Prerequisites

- .NET SDK 8.x
- Node.js (LTS)
- Docker Desktop

---

### 1 Start the Database

```bash
docker-compose up -d
```

Ensure PostGIS is enabled in the database.

---

### 2️ Build the Angular App

```bash
cd backend/GeoPlaces.Web/ClientApp
npm install
npm run build
```

Angular assets will be emitted to:

```
backend/GeoPlaces.Web/wwwroot/browser
```

---

### 3️ Run the Backend

```bash
dotnet run --project backend/GeoPlaces.Web
```

Or run **GeoPlaces.Web** from Visual Studio.

---

### 4️ Access the App

- **Angular SPA**  
  http://localhost:5000/

- **Swagger / API Docs**  
  http://localhost:5000/swagger

---

##  Localhost Behavior

When running locally:

- User IP resolves to loopback
- Backend automatically falls back to **New York City**
- Ensures consistent, testable behavior without external dependencies

---

##  API Notes

- DTO-based responses only
- Distances are expressed in **meters**
- Spatial queries use PostGIS geography functions
- Radius values are constrained for safety and performance

---

##  Design Decisions

- **Angular is built, not served**  
  No `ng serve`, no proxy configuration.

- **Spatial types stay backend-only**  
  Frontend never deals with geometry objects.

- **Single deployment unit**  
  Simplifies operations and hosting.

---

##  Possible Enhancements

- Pagination and filtering
- Authentication / authorization
- Caching (GeoIP + read endpoints)
- Telemetry (App Insights / OpenTelemetry)
- Event-driven processing (outbox pattern)
- SSR or pre-rendering for SEO

---

##  Summary

GeoPlaces is a practical example of a **location-aware SPA** that emphasizes:

- Clean separation of concerns
- Predictable developer experience
- Production-aligned architecture
- Spatial correctness without frontend complexity

Ideal as a learning project, interview discussion piece, or foundation for more advanced spatial features.


---

## TODO / Future Improvements

### COMPLETE - Architecture and Boundaries
- Separate concerns: move DB access out of controllers into a repository/service layer. Controllers should be thin orchestration.
- Formalize DTOs: establish read/write DTOs and keep EF entities internal to the data layer. Avoid leaking spatial library types to the API surface.
- Introduce a minimal domain layer so business rules do not live in controllers or infrastructure services.

### COMPLETE - API Design and Contract Quality
- Standardize response shapes (consistent envelopes and error formats).
- Add API versioning (e.g., `/v1` route strategy).
- Add pagination, sorting, and filtering to list endpoints.
- Add validation rules for latitude/longitude bounds, category constraints, and name length.
- Implement idempotency and conflict handling (e.g., prevent duplicate places within a radius).
- Improve OpenAPI documentation with descriptions, examples, and explicit response codes.

### Observability and Operational Readiness
- Add centralized structured logging with correlation IDs and consistent fields.
- Enable distributed tracing across services.
- Integrate Application Insights or OpenTelemetry for metrics and dependency tracking.
- Add health checks (liveness and readiness), including database and GeoIP reachability.
- Add rate limiting, especially for GeoIP lookups.

### Resilience and Reliability
- Harden GeoIP provider usage with timeouts, retries, circuit breakers, caching, and fallbacks.
- Add proper forwarded headers support for reverse proxy deployments.
- Introduce caching for GeoIP results and read-heavy endpoints.
- Define explicit timeout budgets for database queries and external HTTP calls.

### Spatial Correctness and Performance
- Ensure spatial indexes exist on location columns.
- Make SRID usage explicit and consistent across storage and queries.
- Be explicit about distance units (meters vs kilometers) in APIs and naming.
- Add guardrails on radius values (min/max).
- Review nearby-query implementation to avoid unnecessary casts and expensive operations.
- Add data quality controls to reject invalid geometries and enforce constraints at insert time.

### Security and Compliance Basics
- Tighten CORS policies beyond development.
- Add authentication and authorization (e.g., JWT-based).
- Ensure strict input validation and protection against injection attacks.
- Move secrets and connection strings to secure configuration providers.

### Configuration and Environment Management
- Make radius options configurable per environment.
- Add feature flags (e.g., enable/disable GeoIP or switch providers).
- Support per-environment configuration for logging, CORS, and external services.
- Improve Docker Compose quality with health checks and documented ports.

### Testing Strategy
- Add backend unit tests for validation and mapping logic.
- Add backend integration tests using containerized Postgres/PostGIS.
- Add contract tests to protect API response stability.
- Add frontend component and service tests.
- Introduce deterministic test data seeding and teardown.

### Frontend Engineering Improvements
- Introduce clearer state management patterns to avoid scattered subscriptions.
- Improve error handling, empty states, and loading indicators.
- Add form validation for user inputs.
- Externalize environment configuration for API endpoints and feature flags.
- Improve accessibility and responsive layout behavior.

### Build, CI/CD, and Developer Experience
- Provide a one-command startup workflow for local development.
- Formalize EF Core migrations instead of manual SQL.
- Automate Angular builds as part of `dotnet publish`.
- Add CI pipelines for build, test, and linting.
- Enforce consistent formatting and linting via pre-commit hooks.

### Microservices and Event-Driven Next Steps
- Replace the console event publisher with a real messaging transport.
- Implement the outbox pattern for reliable event publishing.
- Define event schemas and versioning rules.
