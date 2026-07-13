# You G? — Project Roadmap

Tracks phase completion. Updated as we go. This is the source of truth for "where we left off."

| Phase | Deliverable | Status |
|---|---|---|
| 1 | PRD, user stories, requirements, MVP definition | ✅ Signed off |
| 2 | System architecture, folder structure, tech stack justification | ✅ Signed off |
| 3 | Database design, ER diagram, indexing | ✅ Signed off |
| 4 | REST API design, DTOs, versioning | ✅ Signed off |
| 5 | Backend development (.NET 9) | 🟡 In progress — scaffolding + persistence layer merged |
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
| 2026-07-13 | Monorepo layout: `/backend` (.NET solution) + `/mobile` (Flutter app) at repo root | Single repo simplifies CI/CD and versioning for a solo/small-team portfolio project |
| 2026-07-13 | CI/CD groundwork (GitHub Actions skeleton + branch protection) moved up from Phase 8 to before Phase 5 coding starts | Branches + CI need to exist together to be meaningful; branches without gated PRs are just naming conventions |
| 2026-07-13 | Branching strategy: `chore/*` and `feature/*` branches off `master`, PR required, CI (`backend`/`mobile` checks) must pass, 0 required approvals (solo dev) | Standard PR-gated workflow, demonstrates practice without adding self-review friction for a one-person team |
| 2026-07-13 | Google Sign-In setup fully deferred until Flutter app exists (Phase 6) | Mobile client IDs need a real app package name + signing key; half-configuring the Google Cloud project now would sit unfinished |

## Repo & Branches
Repo: https://github.com/totaaa654/you-g (public). `master` is protected — PRs required, `backend`/`mobile` CI checks must pass, no force-push/delete.

Branches created 2026-07-13 (all off `master`):
- `chore/backend-scaffolding` — merged 2026-07-13 (PR #2)
- `feature/auth` — merged 2026-07-13 (PR #4)
- `feature/groups` — merged 2026-07-13 (PR #6)
- `feature/availability-smart-time-finder` — merged 2026-07-13 (PR #8)
- `feature/events-voting` — merged 2026-07-13 (PR #10)
- `chore/flutter-scaffolding` — empty, not started
- `feature/profile`, `feature/friends`, `feature/maps`, `feature/notifications`, `feature/search-settings` — empty, not started

## Backend scaffolding (merged 2026-07-13, PR #2)
`backend/` — .NET 9 solution, 8 projects (Domain, Application, Infrastructure, API + 4 test projects):
- 16 Domain entities + 8 enums matching `docs/03-DATABASE.md` exactly, all built on a shared `Entity` base using `Guid.CreateVersion7()`
- `YouGDbContext` + per-entity EF Core configurations (citext, smallint enums, jsonb, the critical `(UserId, Date)` overlap index, explicit FK/cascade matrix)
- MediatR CQRS-lite wired via `AddApplication()`, with `ValidationBehavior` (FluentValidation) and `LoggingBehavior` pipeline behaviors
- `GlobalExceptionHandler` (RFC 7807 ProblemDetails), Swagger, API versioning wired in `Program.cs`
- `NetArchTest`-based architecture tests enforcing the dependency rule mechanically (3 passing)
- `docker-compose.yml` (Postgres only, for local dev) + initial EF migration, verified against a live database
- **Deliberately deferred to `feature/auth`**: JWT bearer authentication wiring — this branch only set up feature-agnostic scaffolding

## Auth feature (merged 2026-07-13, PR #4)
Email/password register, login, refresh, logout — the foundation every other feature branch depends on ([Authorize] now actually works):
- Repository + Unit of Work pattern (`IUserRepository`, `IRefreshTokenRepository`) backing the four MediatR commands
- Password hashing via ASP.NET Core Identity's `PasswordHasher<T>` (PBKDF2) — no third-party dependency
- JWT access tokens (15min) + rotating, hashed, single-use refresh tokens (30-day); replaying a rotated token is rejected
- `ConflictException`/`AuthenticationFailedException` → 409/401 via `GlobalExceptionHandler`
- 13 unit tests against in-memory fake repositories (zero DB dependency) + full manual end-to-end verification via curl against live Postgres
- **Deliberately out of scope, tracked as follow-up**: Google Sign-In and forgot/reset-password — both need external dependencies (Google token verification, an email sender) not yet in the codebase. Decision made 2026-07-13: defer fully until the Flutter app exists (Phase 6), since Google Sign-In's mobile client IDs can't be finished without a real app package name/signing key anyway.

## Groups feature (merged 2026-07-13, PR #6)
Create/update group, membership, roles, invite links — unblocks Availability/Events/Voting, which all depend on group membership existing:
- Repository pattern (`IGroupRepository`, `IGroupMemberRepository`, `IGroupInviteLinkRepository`) + new `ICurrentUserService` (resolves caller from JWT claims, reusable by every future authenticated feature)
- New shared exceptions `NotFoundException` (404) / `ForbiddenException` (403), matching the Phase 4 authorization matrix exactly (non-member → 404, member-but-not-admin → 403)
- Business rules: can't leave/demote a group's sole admin while other members exist; re-joining an already-valid invite link is idempotent
- **Real bug found via manual testing, not caught by unit tests**: `JwtBearerHandler` remaps the `sub` claim to a legacy `ClaimTypes.NameIdentifier` URI by default, silently breaking `ICurrentUserService`. Fixed with `options.MapInboundClaims = false`. Reinforces why the manual end-to-end curl pass is a required step, not optional polish, for every feature branch.
- 16 new unit tests (29 total) + full manual verification against live Postgres with two real users (create, 404-not-member, invite link, join, 403-not-admin, 409-sole-admin, removal)

## Availability & Smart Time Finder (merged 2026-07-13, PR #8)
The product's core differentiator per the PRD — recurrence rules, materialization, group overlap ranking, and heatmap:
- `RecurrenceMaterializationJob`: expands `AvailabilityRules` into materialized `AvailabilityInstance` rows for a rolling 8-week horizon. Triggered immediately after a rule is created (instant feedback) and swept every 24h by `RecurrenceMaterializationBackgroundService`. Never overwrites a manually-edited instance or one owned by a different rule.
- **Design correction made during development**: the job started as an Infrastructure class coupled directly to `DbContext`, inconsistent with the Repository pattern used everywhere else — refactored into a pure Application-layer class (`YouG.Application/Features/Availability/Jobs/`) depending only on repository interfaces. Fully unit-testable with fakes as a result.
- `GET /groups/{id}/overlap`: ranks windows by available-member count descending, with weekend-only and preferred-daypart filters (dayparts, not clock times, per the daypart-granularity decision)
- `GET /groups/{id}/heatmap`: flat cell list, Available-only counts
- **Two real bugs found via manual testing**: (1) enums weren't serialized as strings by default — `"Monday"` couldn't bind to `DayOfWeek` — fixed with a global `JsonStringEnumConverter`, now matching every example in `docs/04-API-DESIGN.md`; (2) an off-by-one in the test author's own test expectations (56-day horizon is inclusive → 9 weekly occurrences, not 8), caught while writing tests rather than manually.
- 18 new unit tests (42 total) covering override-preservation, EffectiveFrom/EffectiveUntil boundaries, overlap ranking, heatmap counting + full manual verification against live Postgres

## Events & Voting (merged 2026-07-13, PR #10)
Turns an overlap window into a concrete plan — the last piece of the core product loop:
- Create event two ways: fixed time+location (auto-Confirmed, no voting) or open-ended (Proposed, then propose/vote on `EventTimeOptions`/`EventLocationOptions` before the organizer calls `/confirm`) — both paths resolve through the same option tables per the Phase 3 design note
- Voting is idempotent (PUT again is a no-op) and retractable (DELETE)
- Attendance enforces `MaxAttendees` as a hard cap on `Going` only — `Maybe`/`CantGo` never blocked, and a user already `Going` can re-affirm even when the event is externally full
- New `EventAuthorization` helper reuses the 404-non-member/403-non-organizer pattern from Groups
- **Real bug found via manual testing, not caught by unit tests**: creating an event with both a fixed time AND location threw "circular dependency detected" from EF Core — `Event.ConfirmedTimeOptionId` points at `EventTimeOption` while `EventTimeOption.EventId` points back at `Event`, and inserting both new rows in one `SaveChanges` call is a genuine cycle EF can't topologically sort regardless of client-generated IDs. Fixed by splitting into two `SaveChanges` calls (insert first, then a plain UPDATE for the Confirmed*Id pointers). This is the third feature in a row where manual curl testing caught something unit tests structurally couldn't.
- 15 new unit tests (57 total) + full manual verification against live Postgres with three real users

## Next up
Phase 5 in progress. The full core product loop is done end-to-end: register → create group → set availability → find overlap → create event → vote → confirm → RSVP. Remaining backend feature branches: Profile, Friends, Maps, Notifications, Search & Settings — all independent, none blocking. Also worth considering: pivoting to `chore/flutter-scaffolding` to start the mobile app now that there's a substantial API surface to build against.
