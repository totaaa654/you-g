# You G? — Project Roadmap

Tracks phase completion. Updated as we go. This is the source of truth for "where we left off."

| Phase | Deliverable | Status |
|---|---|---|
| 1 | PRD, user stories, requirements, MVP definition | ✅ Signed off |
| 2 | System architecture, folder structure, tech stack justification | ✅ Signed off |
| 3 | Database design, ER diagram, indexing | ✅ Signed off |
| 4 | REST API design, DTOs, versioning | ✅ Signed off |
| 5 | Backend development (.NET 9) | ⬜ Not started |
| 6 | Flutter development | ⬜ Not started |
| 7 | Testing (unit/integration/widget) | ⬜ Not started |
| 8 | DevOps (Docker, CI/CD, deployment) | ⬜ Not started |
| 9 | Production hardening | ⬜ Not started |

## Key decisions log

| Date | Decision | Rationale |
|---|---|---|
| 2026-07-13 | MVP target user = casual friend groups (5-15 people) | Core "You G?" pitch; optimizes UX for fast, low-friction scheduling over recurring-team or large-event use cases |
| 2026-07-13 | Smart Time Finder + Heatmap included in MVP (not deferred) | Considered the product's core differentiator — without it, MVP is just another shared calendar |
| 2026-07-13 | Primary goal = portfolio piece | Favor architectural rigor, tests, and CI/CD over raw shipping speed when the two trade off |
| 2026-07-13 | Availability granularity = daypart buckets (not hourly) | Matches how friend groups think ("free Thurs evening"); avoids building a calendar-grade precision engine for MVP |
| 2026-07-13 | Recurring availability = materialized instances (~8-12wk rolling horizon), not runtime RRULE expansion | Keeps overlap queries as plain SQL over real rows; simpler to test and reason about than rule-expansion-at-query-time |
| 2026-07-13 | Group visibility = membership alone grants availability visibility (no mutual-friend requirement) | Matches real usage; single authorization rule ("shared group") instead of two overlapping rules |
| 2026-07-13 | Invite links = expiring (7d default), multi-use | Balances share-once convenience against leaked-link risk |
| 2026-07-13 | Modular monolith (not microservices), Clean Architecture with enforced dependency rule | One team, ~100K user target doesn't justify distributed-systems tax; layer boundaries keep it extractable later if ever needed |
| 2026-07-13 | Application layer = MediatR CQRS-lite | Free cross-cutting behavior (validation, logging) via pipeline behaviors; idiomatic modern .NET Clean Architecture pattern |
| 2026-07-13 | Recurrence job = hosted BackgroundService now, Hangfire-promotable later | Zero extra infra at MVP scale; swappable behind `IRecurrenceMaterializationJob` |
| 2026-07-13 | Deploy target = Railway primary, Azure documented as alternative | Fast path to a real live demo URL; Azure path shown in Phase 8 without slowing MVP |
| 2026-07-13 | Real-time (SignalR) explicitly deferred to V2 | Push notifications + refresh-on-navigation is sufficient for actual usage pattern; avoids unvalidated complexity |
| 2026-07-13 | PKs = UUIDv7 (time-ordered), not bigint identity | Non-enumerable public-facing resource IDs while retaining index locality on insert, unlike random UUIDv4 |
| 2026-07-13 | Account deletion = soft-delete tombstone (PII scrubbed, row persists) | Avoids per-table special-casing of FK integrity for every table referencing a deleted user (votes, attendance, memberships) |
| 2026-07-13 | FriendRequests table doubles as Friendships (Status=Accepted) | One table, no sync step; extra "clarity" of a separate table isn't worth the moving part at this scale |
| 2026-07-13 | API versioning = URL segment (`/api/v1/...`) | Visible in logs/Swagger, no header the Flutter client must remember to set |
| 2026-07-13 | Pagination = offset-based (page/pageSize), not cursor-based | Simpler, sufficient at MVP list-size scale; revisit for Notifications feed in Phase 9 if needed |
| 2026-07-13 | 404 (not 403) returned when a non-member requests a group/event they can't access | Prevents enumerating valid resource IDs the requester has no access to |

## Next up
Phase 4 (`docs/04-API-DESIGN.md`) signed off. Proceeding to Phase 5: Backend Development (ASP.NET Core Clean Architecture implementation).
