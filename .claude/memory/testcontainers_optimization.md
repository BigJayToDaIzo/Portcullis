---
name: testcontainers-optimization
description: Investigate sharing a single PostgreSQL Testcontainer across integration tests instead of one per test
type: project
---

Each integration test currently spins up its own PostgreSqlContainer via IAsyncLifetime. As the test suite grows this will get slow and resource-heavy.

**Why:** Observed multiple containers spinning up in parallel during test runs — will compound as Phase 2 adds more tests.

**How to apply:** Before Phase 2 is complete, investigate xUnit v3 patterns for sharing a single container (e.g., collection fixtures or assembly-level fixtures). Balance isolation vs speed.
