# Copilot instructions for this repo

Purpose: help AI coding agents be productive immediately in this Angular SPA.

- **Quick start (dev)**: run `npm install` then `npm start` (runs `ng serve`). See `package.json` scripts.
- **Build (prod)**: `npm run build` — output is written to `../wwwroot` (see `angular.json` -> `outputPath`).
- **Tests**: `npm test` runs Vitest through Angular CLI.

Architecture overview
- This is a small Angular standalone-component SPA (see `src/app/app.ts` and `src/app/app.config.ts`).
- Routing is configured via `src/app/app.routes.ts` (currently empty). `appConfig` calls `provideRouter(routes)`.
- Build outputs are intended for an ASP.NET host: Angular's build writes into `../wwwroot`.

Key integration points (backend API)
- All backend calls use relative paths under `/api/v1`. Important services:
  - `src/app/places.service.ts` — uses `/api/v1/places` and expects paged results in the response body with paging metadata in headers (`X-Total-Count`, `Link`). Follow the same header/payload pattern when adding endpoints.
  - `src/app/location.service.ts` — calls `/api/v1/location/me` to resolve client location.

Project-specific conventions
- Services are provided in root via `@Injectable({ providedIn: 'root' })` — prefer that pattern for new services.
- Paging: backend returns the array in the body and paging metadata in headers. See `PlacesService.getAll()` for an example of parsing `X-Total-Count` and `Link`.
- Standalone components: files use the `imports` array in `@Component` (example: `src/app/app.ts`) — follow that style rather than NgModule-based registration.
- Static assets live in `public/` and are referenced in `angular.json` as an asset input.

Developer workflows & useful notes
- Use `npm ci` or `npm install` before running scripts. `package.json` lists `start`, `build`, `watch`, and `test` scripts.
- Local dev server: `npm start` -> `http://localhost:4200/` (hot reload enabled).
- When adding API clients, prefer `HttpClient` and keep full typed DTO interfaces near the service (see `PlaceDto`, `NearbyPlaceDto`).
- When adding new builds, remember `angular.json` default configuration is `production` (the dev server defaults to `development`).

Files to inspect when making changes
- App bootstrap and router: `src/app/app.config.ts` and `src/app/app.routes.ts`.
- Root component: `src/app/app.ts`, `src/app/app.html`, `src/app/app.css`.
- Backend clients: `src/app/places.service.ts`, `src/app/location.service.ts`.
- Project scripts and dependency versions: `package.json`.

When in doubt
- Mirror existing patterns: DTO interfaces in service files, header-based paging, standalone components and `provideRouter` usage.
- Ask for clarification if a backend route does not follow the `/api/v1/*` pattern or if paging metadata is different.

If you want me to expand this with code-mod examples (e.g., add a new API call or new route), say which file to modify.
