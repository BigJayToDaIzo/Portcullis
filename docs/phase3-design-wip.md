# Phase 3 Design — Work In Progress

> Status: IN PROGRESS — brainstorming, not yet approved

## Scope

Phase 3: Service layer + authorization rules for Secrets.

Phases broken out as:
- **Phase 2.5** — Keycloak + OIDC auth pipeline (not yet designed)
- **Phase 3** — Service layer + authorization rules (this doc)
- **Phase 4** — Audit logging (not yet designed)

User sync from token claims — undecided, likely Phase 2.5 but deferred until that phase is designed.

---

## Service Layer — SecretService

Single service class, inline role checks. No split by role — domain is small enough.

| Method | Who can call | What it does |
|---|---|---|
| `GetSecrets(userId, queryParams)` | Owner | List secrets for the calling user (paginated, filterable, sortable) |
| `GetSecret(userId, secretId)` | Owner | Get a single secret, scoped to the calling user |
| `CreateSecret(userId, name, value)` | Owner | Create a secret, enforces unique name per user |
| `UpdateSecret(userId, secretId, name?, value?)` | Owner | Update own secret |
| `DeleteSecret(userId, secretId)` | Owner | Delete own secret |
| `GetAllSecrets(queryParams)` | Admin | Read all secrets across all users (paginated, filterable, sortable) |
| `ResetSecret(secretId)` | Admin | Null the value — user must set a new one |
| `AdminDeleteSecret(secretId)` | Admin | Delete any secret |

Each method checks the caller's role before proceeding. Owner methods verify the secret belongs to the calling user. Admin methods verify the caller has the admin role.

---

## Exceptions

Three custom exception types. Exceptions bubble up — no try-catch in service or controllers. Single `IExceptionHandler` at the API boundary maps to ProblemDetails (RFC 7807).

Each exception carries **structured data** as public read-only properties — the handler has direct access to typed values for logging and ProblemDetails construction without parsing `ex.Message`. The exception constructs its own message from the structured data via `base($"...")` — callers pass typed values, not hand-rolled strings.

### AuditAction Enum

Lives in `Domain/Enums/`. Defines the finite set of operations in the system — used by `NotAuthorizedException` to constrain what operation the caller attempted.

Values: `Create`, `Read`, `Update`, `Delete`, `Reset`

### Exception Definitions

| Exception | HTTP Status | When | Carries |
|---|---|---|---|
| `SecretNotFoundException` | 404 | Secret ID doesn't exist, or user tries to access a secret that isn't theirs | `Guid SecretId` |
| `NotAuthorizedException` | 403 | Caller's role doesn't permit the operation | `Guid UserId`, `AuditAction Operation` |
| `DuplicateSecretNameException` | 409 | Create/update with a name that already exists for that user | `string Name` |

### Handler Behavior per Exception

- **`SecretNotFoundException`** — Response: 404, detail includes the secret ID (client already sent it). Log: warning level.
- **`DuplicateSecretNameException`** — Response: 409, detail includes the name (client sent it). Log: warning level.
- **`NotAuthorizedException`** — Response: 403, generic "You do not have permission to perform this action." — no internals leaked. Log: warning level, full message with userId and operation.
- **Unexpected exceptions** — Response: generic 500 with no internals leaked. Log: error level.

**404 vs 403 for owner methods**: If a regular user requests a secret that exists but belongs to someone else, return **404 not 403** — don't leak the existence of other users' secrets.

---

## Controller Layer — SecretsController

Thin controller. No business logic, no authorization checks. It:
1. Extracts user ID from token claims (wired in Phase 2.5)
2. Calls SecretService
3. Returns the result — exception handler deals with errors

### Endpoints

| Verb | Route | Maps to |
|---|---|---|
| `GET` | `/api/secrets` | `GetSecrets(userId)` |
| `GET` | `/api/secrets/{id}` | `GetSecret(userId, id)` |
| `POST` | `/api/secrets` | `CreateSecret(userId, name, value)` |
| `PUT` | `/api/secrets/{id}` | `UpdateSecret(userId, id, name?, value?)` |
| `DELETE` | `/api/secrets/{id}` | `DeleteSecret(userId, id)` |
| `GET` | `/api/admin/secrets` | `GetAllSecrets()` |
| `POST` | `/api/admin/secrets/{id}/reset` | `ResetSecret(id)` |
| `DELETE` | `/api/admin/secrets/{id}` | `AdminDeleteSecret(id)` |

Admin routes under `/api/admin/` — blanket `[Authorize(Roles = "admin")]` at the route group level.

---

## Request/Response DTOs

DTOs sit between the client and the service layer. Controllers accept request DTOs and return response DTOs. Entities never cross the API boundary.

### Request DTOs

| DTO | Fields | Notes |
|---|---|---|
| `CreateSecretRequest` | `Name` (required), `Value` (required) | Used by `POST /api/secrets` |
| `UpdateSecretRequest` | `Name` (optional), `Value` (optional) | Used by `PUT /api/secrets/{id}` — at least one field must be present |

Admin operations (`reset`, `delete`) take no request body — IDs come from the route.

### Response DTOs

**`SecretResponse`** — returned by owner endpoints

| Field | Type |
|---|---|
| `Id` | Guid |
| `Name` | string |
| `Value` | string |
| `CreatedAt` | DateTime |
| `UpdatedAt` | DateTime |

**`AdminSecretResponse`** — returned by admin endpoints

| Field | Type |
|---|---|
| `Id` | Guid |
| `Name` | string |
| `Value` | string |
| `CreatedAt` | DateTime |
| `UpdatedAt` | DateTime |
| `UserId` | Guid |
| `UserName` | string |

Two separate response DTOs rather than one shared shape with nullable fields. Each DTO represents a specific use case. Admin response includes `UserId` and `UserName` so the admin view is self-contained — `UserName` resolved from the User entity via `UserId`.

---

## Interface — ISecretService

`SecretService` implements `ISecretService`. Controller depends on the interface, not the concrete class.

**Why**: Provides a testability seam — controller unit tests mock the interface without hitting the real service or database. Service unit tests mock the DbContext without hitting a real database.

`ISecretService` mirrors the method table from the Service Layer section above.

---

## DI Registration — Extension Method

Services registered via `builder.Services.AddApplicationServices()` extension method rather than inline in `Program.cs`.

**Note**: For a domain this small (one service, one DbContext), inline registration would be fine. The extension method is a scalability pattern — it becomes necessary in projects with many services, middleware registrations, and database configurations. Used here to demonstrate the pattern.

---

## Pagination, Filtering & Sorting

Both list endpoints support pagination, filtering, and sorting. Offset-based pagination — simple, works well with any UI table/grid component.

### Pagination

All list endpoints return a paginated wrapper rather than raw arrays.

**Query Parameters** (shared by both endpoints):

| Param | Type | Default | Notes |
|---|---|---|---|
| `page` | int | 1 | 1-indexed |
| `pageSize` | int | 20 | Max cap TBD (see Open Gap 8) |
| `sortBy` | string | `CreatedAt` | Field name to sort on (whitelist enforced — see Open Gap 9) |
| `sortDirection` | string | `desc` | `asc` or `desc` (whitelist enforced — see Open Gap 9) |

**Paginated Response Wrapper** — `PaginatedResponse<T>`:

| Field | Type |
|---|---|
| `Items` | `List<T>` |
| `Page` | int |
| `PageSize` | int |
| `TotalCount` | int |
| `TotalPages` | int |

Default sort: `CreatedAt` descending (newest first).

### Filtering & Sorting by Endpoint

**Owner** (`GET /api/secrets`):
- **Filter by**: `Name` (partial match / contains)
- **Sort by**: `Name`, `CreatedAt`, `UpdatedAt`

**Admin** (`GET /api/admin/secrets`):
- **Filter by**: `Name` (partial match / contains), `UserName`
- **Sort by**: `Name`, `UserName`, `CreatedAt`, `UpdatedAt`

Filter parameters passed as query strings (e.g. `?name=api&userName=joe`). No filtering/sorting on IDs — they aren't human-readable.

---

## Open Gaps — Must Resolve Before Implementation

### Gap 1: TDD Progression
**Decision**: Implementation order defined below.

**Status**: DECIDED

### Gap 2: Secret → User Navigation Property
**Decision**: Add `public User User { get; init; }` to Secret. Update SecretConfiguration to `builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId)`. Admin queries use `.Include()` or projection via `s.User.DisplayName`.

**Status**: DECIDED

### Gap 3: Who Sets UpdatedAt on Save?
**Decision**: SaveChanges override in DbContext. Detect entities in `Modified` state that have `UpdatedAt`, stamp `DateTimeOffset.UtcNow` before save. Automatic, centralized, fire-and-forget.

**Status**: DECIDED

### Gap 4: Query Parameter DTOs
**Decision**: `SecretQueryParameters` base class with page, pageSize, sortBy, sortDirection, name filter. `AdminSecretQueryParameters : SecretQueryParameters` adds userName filter. Inheritance — admin params are a genuine superset of owner params.

**Status**: DECIDED

### Gap 5: Request Validation Location
**Decision**: Validate in the service layer (keeps controller thin, all business rules in one place). Throw `ArgumentException` (BCL) for bad input — no custom exception class. `IExceptionHandler` maps `ArgumentException` → 400 ProblemDetails.

**Status**: DECIDED

### Gap 6: Auth Stubbing for Phase 3 (No Keycloak Yet)
**Decision**: Service layer takes userId as a parameter — no auth dependency in service tests. Controller tests mock `HttpContext.User` with fake `ClaimsPrincipal` (standard ASP.NET Core pattern). No real auth middleware in Program.cs until Phase 2.5. Controller code writes the claims extraction logic now — it'll work once Keycloak is plugged in.

**Status**: DECIDED

### Gap 7: User Existence in Tests
**Decision**: Service tests mock DbContext — no FK constraints, arrange whatever state needed. Integration tests (Testcontainers) seed User records via direct DB inserts before creating Secrets. Same pattern as Phase 2.

**Status**: SUPERSEDED — actual implementation uses SQLite in-memory (real EF + constraints) rather than mocked DbContext. User records are seeded per-test in the same arrange block as Secrets. The original "mock DbContext" decision proved less useful than real LINQ translation against a real DB.

### Gap 8: Pagination Max Page Size Cap
**Decision**: TBD — caller can currently request `pageSize=10000` with no upper bound. Need to either silently clamp (e.g., max 100) or reject with `ArgumentException`. Silent clamp is friendlier for clients; reject is louder about API contract.

**Status**: OPEN — must resolve before implementing `GetSecretsAsync` / `GetAllSecretsAsync`.

### Gap 9: Sort Field & Direction Whitelisting
**Decision**: TBD. `SortBy` and `SortDirection` are free-form strings on `SecretQueryParameters`. Without a whitelist, callers can pass arbitrary column names (or worse). Need to validate against:
- SortBy: `Name`, `CreatedAt`, `UpdatedAt` (owner) plus `UserName` (admin)
- SortDirection: `asc`, `desc` only

Where to enforce — same answer as Gap 5 (service layer, throw `ArgumentException`).

**Status**: OPEN — must resolve before implementing list methods.

### Gap 10: Page Out of Range Behavior
**Decision**: TBD. If client requests `page=99` and only 2 pages exist, options are: (a) return empty `Items` with correct `TotalCount`/`TotalPages` (RESTful, lets client paginate forward); (b) throw `ArgumentException` (strict). Most APIs choose (a).

**Status**: OPEN.

### Gap 11: Name Filter Match Semantics
**Decision**: Design doc §Pagination says "partial match / contains" — confirm this means case-insensitive substring (`ILIKE %name%` on Postgres). EF translates `EF.Functions.ILike(s.Name, $"%{name}%")` to the right SQL.

**Status**: OPEN — confirm semantics before TDD.

### Gap 12: AdminSecretResponse DTO Bugs
**Decision**: Two type mismatches between entity and DTO must be fixed:
- `AdminSecretResponse.Value` is `string`; should be `string?` (matches `Secret.Value` which can be null after admin reset).
- `AdminSecretResponse.UserId` is `Guid`; should be `string` (matches `Secret.UserId` / Keycloak subject id, which is string-typed).

These will cause compile failures or null-coalescing surprises during entity-to-DTO mapping in `GetAllSecretsAsync`.

**Status**: OPEN — fix before TDD'ing `GetAllSecretsAsync`.

---

## Phase 3 TDD Progression

### Foundation (steps 1–6)

1. ~~**Custom exceptions** — `SecretNotFoundException`, `NotAuthorizedException`, `DuplicateSecretNameException`~~ ✅ DONE
2. ~~**Request DTOs** — `CreateSecretRequest`, `UpdateSecretRequest`~~ ✅ DONE *(UpdateSecretRequest later deleted; replaced by `RenameSecretRequest` and `RotateSecretRequest` — see Step 8)*
3. ~~**Entity/DbContext updates** — Add `User` navigation property to `Secret`, update `SecretConfiguration`, add `SaveChangesAsync` override for `UpdatedAt` stamping~~ ✅ DONE
4. ~~**Response DTOs** — `SecretResponse`, `AdminSecretResponse`~~ ✅ DONE *(AdminSecretResponse has 2 type bugs — see Open Gap 12)*
5. ~~**Query parameter DTOs** — `SecretQueryParameters` (base), `AdminSecretQueryParameters` (inherits, adds userName)~~ ✅ DONE
6. ~~**PaginatedResponse\<T>** — generic wrapper~~ ✅ DONE

### Core logic (sequential — steps 7–11)

7. ~~**ISecretService** — interface with method signatures~~ ✅ DONE *(UpdateSecretAsync split into RenameSecretAsync + RotateSecretAsync mid-implementation)*
8. **SecretService** — IN PROGRESS (90 tests green, 6 of 8 methods done)
   - ✅ `CreateSecretAsync`, `GetSecretAsync`, `DeleteSecretAsync`, `RenameSecretAsync`, `RotateSecretAsync`, `ResetSecretAsync`, `AdminDeleteSecretAsync`
   - ⏳ `GetSecretsAsync` (paginated, owner-scoped) — blocked on Open Gaps 8-11
   - ⏳ `GetAllSecretsAsync` (paginated, admin) — blocked on Open Gaps 8-12
   - **Authz note:** Service layer takes `userId` for owner methods, no role checks. Admin methods take only `secretId`. Service trusts the API layer for role authz — see authz-strategy memory.
9. **IExceptionHandler** — maps custom exceptions + `ArgumentException` to ProblemDetails (RFC 7807)
10. **SecretsController** — thin controller, tested with `WebApplicationFactory<TProgram>` + fake JWT/ClaimsPrincipal. **First test for each admin endpoint must be "non-admin → 403."**
11. **DI extension method** — `AddApplicationServices()` registration wiring
