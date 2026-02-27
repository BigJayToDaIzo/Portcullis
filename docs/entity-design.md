# Entity Design — Secret Vault Domain

> Decided: 2026-02-26
> Status: Approved — ready for TDD implementation

## Context

Portcullis is a portfolio piece demonstrating auth/authz competence. The scaffold is complete (.NET 10 webapi + xUnit test project, both green). These entities define the domain — the thing worth protecting before we build the fortifications around it.

All design decisions below were discussed and confirmed by the developer.

---

## Entities

### 1. User (local, synced from Keycloak)

| Field | Type | Notes |
|---|---|---|
| Id | string | Keycloak `sub` claim. Primary key. Not a GUID — OIDC subject claims are opaque strings. |
| DisplayName | string | From token claims. For admin dashboard display. |
| Email | string | From token claims. |
| CreatedAt | DateTimeOffset | First time this user was seen (first login). |
| UpdatedAt | DateTimeOffset | Last refresh from token claims. |

- Synced on login — when a token hits the API, create or update the local record from token claims.
- Foreign key target for Secret.UserId and AuditLogEntry.UserId / TargetUserId.

### 2. Secret

| Field | Type | Notes |
|---|---|---|
| Id | Guid | Primary key. |
| UserId | string | FK to User.Id. Required. Ownership. |
| Name | string | Label/display name. Required. Max length ~200. |
| Value | string? | Nullable — null means "reset by admin, user must set new value." Plaintext storage. |
| CreatedAt | DateTimeOffset | Set once at creation. |
| UpdatedAt | DateTimeOffset | Updated on every write (update, reset). Initially same as CreatedAt. |

- **Unique constraint**: composite index on (UserId, Name) — no duplicate names per user.
- **Index**: on UserId for ownership queries.
- **No soft delete** — delete is real, audit log records what happened.

### 3. AuditLogEntry

| Field | Type | Notes |
|---|---|---|
| Id | Guid | Primary key. |
| UserId | string | FK to User.Id. Who performed the action. Required. |
| TargetUserId | string? | FK to User.Id. Whose secret was affected. Nullable. Equals UserId for self-actions, different for admin actions. |
| SecretId | Guid? | Which secret. Nullable (secret may have been deleted). **No FK constraint** — audit log must survive secret deletion. |
| Action | AuditAction (enum) | Create, Read, Update, Delete, Reset. Stored as string in DB. |
| Timestamp | DateTimeOffset | When. Set once. |
| Description | string? | Optional human-readable context. Max length ~500. |

- **Indexes**: on UserId, TargetUserId, SecretId, Timestamp.
- **No FK to Secret** — intentionally decoupled. SecretId is a value, not a navigation property.
- **FK to User** on both UserId and TargetUserId — users persist, secrets may not.

### 4. AuditAction Enum

Five values: `Create`, `Read`, `Update`, `Delete`, `Reset`

- Reset is separate from Update — different authorization rules, different semantics.
- Stored as string via EF Core value converter for human-readable database.

---

## Design Rationale

### Why a local User table?
Keycloak owns identity, but Portcullis needs referential integrity at the database level and display data (name, email) for the admin dashboard without round-tripping to Keycloak. Synced on login from token claims — no background sync job needed.

### Why unique secret names per user?
Real vault systems (AWS Secrets Manager, Azure Key Vault, HashiCorp Vault) enforce unique names within a scope. A composite unique index on (UserId, Name) mirrors this. Also creates a meaningful error path for TDD.

### Why no FK from AuditLogEntry to Secret?
The audit log must survive secret deletion. If a secret is deleted, the audit record of its existence and what happened to it must persist. SecretId is stored as a value for querying, not as a navigation property.

### Why Reset is a separate AuditAction?
An Update is a user changing their own secret value. A Reset is an admin nulling it out. Different authorization rules, different actors, different intent. Collapsing them loses information.

### Why no entity history / delta tracking?
Over-engineering for this domain. The audit trail records *that* something happened. The admin dashboard provides filterable log views. No need for old-value/new-value tracking.

---

## Project Structure

### Test files
```
tests/Portcullis.Api.Tests/
  Domain/
    Entities/
      SecretTests.cs
      UserTests.cs
      AuditLogEntryTests.cs
    Enums/
      AuditActionTests.cs
  Data/
    PortcullisDbContextTests.cs
```

### Production files
```
src/Portcullis.Api/
  Domain/
    Entities/
      Secret.cs
      User.cs
      AuditLogEntry.cs
    Enums/
      AuditAction.cs
  Data/
    PortcullisDbContext.cs
    Configurations/
      SecretConfiguration.cs
      UserConfiguration.cs
      AuditLogEntryConfiguration.cs
```

---

## TDD Progression

### Phase 1: Entity Shape (Pure Unit Tests — no EF Core, no DB)

First failing tests. Verify each entity has the right properties with the right types.

1. **UserTests** — Id (string), DisplayName (string), Email (string), CreatedAt (DateTimeOffset), UpdatedAt (DateTimeOffset)
2. **SecretTests** — Id (Guid), UserId (string), Name (string), Value (string?, must be nullable), CreatedAt (DateTimeOffset), UpdatedAt (DateTimeOffset)
3. **AuditLogEntryTests** — Id (Guid), UserId (string), TargetUserId (string?, nullable), SecretId (Guid?, nullable), Action (AuditAction), Timestamp (DateTimeOffset), Description (string?, nullable)
4. **AuditActionTests** — enum has exactly 5 values: Create, Read, Update, Delete, Reset

Each test is red first. Write the minimal entity code to make it green. Refactor if needed. Move to the next.

### Phase 2: EF Core Configuration (Integration Tests — Testcontainers + PostgreSQL)

Once entity shapes are green, test the database configuration against a real Postgres instance.

1. DbContext builds successfully with all DbSets registered
2. Secret table has correct unique constraint on (UserId, Name)
3. Secret.Value is nullable in the schema
4. Secret.UserId is a FK to User.Id
5. AuditLogEntry.SecretId has NO FK constraint (intentionally decoupled)
6. AuditLogEntry.UserId is a FK to User.Id
7. AuditLogEntry.Action is stored as string
8. User.Id is the primary key (string, not auto-generated)

Requires adding packages:
- **API project**: `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`
- **Test project**: `Testcontainers.PostgreSql`, `Microsoft.EntityFrameworkCore`

### Phase 3 (future)
Service layer, authorization rules, audit logging — builds on these entities.
