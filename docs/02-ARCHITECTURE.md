# You G? — System Architecture

Status: **Signed off — 2026-07-13**
Last updated: 2026-07-13

## 1. Architectural Style: Modular Monolith (not microservices)

**Why this matters first, before anything else:** the biggest architectural mistake at this scale is over-engineering for scale you don't have yet.

| Option | Verdict |
|---|---|
| Microservices | ❌ Rejected. One team, one deploy cadence, ~100K target users is not a scale that needs independent service deployment. Microservices would add network-call overhead, distributed-transaction complexity (e.g. "create event + notify members" spans services), and operational cost (service discovery, multiple CI pipelines) with zero benefit at this stage — and would hurt the portfolio story, since it reads as cargo-culting rather than judgment. |
| Single unstructured API project | ❌ Rejected. Fast to start, but without enforced layer boundaries, business logic leaks into controllers and the codebase becomes untestable/unmaintainable as features grow — fails NFR-7/NFR-8. |
| **Modular monolith with Clean Architecture** | ✅ **Chosen.** One deployable API, internally organized into strict layers with enforced dependency direction. Gets you the maintainability and testability benefits people reach for microservices for, without the distributed-systems tax. If a piece (e.g. notifications) ever needs to scale independently, the layer boundaries make it extractable later — you're not locked in. |

This directly serves NFR-2 (stateless, horizontally scalable) and NFR-7 (maintainability) from the PRD.

## 2. High-Level Component Diagram

```
┌─────────────────────┐         ┌──────────────────────┐
│   Flutter Mobile App │◄───────►│   ASP.NET Core API    │
│  (iOS / Android)     │  HTTPS  │  (.NET 9, modular      │
│  Riverpod + GoRouter │  REST   │   monolith)            │
└──────────┬───────────┘         └───────────┬───────────┘
           │                                  │
           │ FCM push                         │ EF Core
           ▼                                  ▼
┌─────────────────────┐         ┌──────────────────────┐
│ Firebase Cloud       │         │   PostgreSQL           │
│ Messaging             │         │   (primary datastore) │
└─────────────────────┘         └──────────────────────┘
                                             │
                                             ▼
                                  ┌──────────────────────┐
                                  │ Background job runner  │
                                  │ (recurrence            │
                                  │  materialization, etc.)│
                                  └──────────────────────┘
```

External integrations: Google OAuth (auth), Google Maps (location picker + rendering), FCM (push).

## 3. Backend: Clean Architecture Layers

The core rule: **dependencies point inward.** Domain knows nothing about Application; Application knows nothing about Infrastructure or API. This is what makes the domain logic (the overlap algorithm, voting resolution) unit-testable without a database, and swappable (e.g. Postgres → something else) without touching business logic.

```
┌────────────────────────────────────────────────────────┐
│  YouG.API            (Controllers, Middleware, Program.cs)│
│  — depends on Application + Infrastructure (composition)  │
└───────────────────────┬──────────────────────────────────┘
                         │
┌────────────────────────▼─────────────────────────────────┐
│  YouG.Infrastructure  (EF Core, repo implementations,      │
│  FCM client, Google OAuth verification, background jobs)   │
│  — depends on Application (implements its interfaces)      │
└───────────────────────┬──────────────────────────────────┘
                         │
┌────────────────────────▼─────────────────────────────────┐
│  YouG.Application     (Use cases, DTOs, validators,         │
│  repository INTERFACES, business orchestration)             │
│  — depends only on Domain                                   │
└───────────────────────┬──────────────────────────────────┘
                         │
┌────────────────────────▼─────────────────────────────────┐
│  YouG.Domain          (Entities, value objects, domain      │
│  events, enums — ZERO external dependencies, not even EF)   │
└──────────────────────────────────────────────────────────┘
```

**Why Repository Pattern specifically**: `Application` defines `IAvailabilityRepository`, `IGroupRepository`, etc. as interfaces. `Infrastructure` implements them with EF Core. This means the overlap-computation use case can be unit-tested against an in-memory fake repository, with no database in the test — directly serving NFR-8.

**Enforcement, not just convention**: a dependency rule you don't enforce will get violated the first time someone's in a hurry. Phase 7 will include an *architecture test* (using `NetArchTest` or `ArchUnitNET`) that fails the build if, say, `Domain` ever references `Microsoft.EntityFrameworkCore`. This is a small but high-signal addition for a portfolio project — it's the kind of thing that shows you're not just following Clean Architecture by convention, you're enforcing it mechanically.

## 4. Application Layer Pattern: CQRS-lite via MediatR (proposed)

Your original stack didn't specify how the Application layer organizes use cases. Two real options:

| Option | Trade-off |
|---|---|
| Plain service classes (`IAvailabilityService.ComputeOverlap(...)`) | Simple, no new library. But services tend to accumulate unrelated methods over time and it's harder to apply cross-cutting concerns (logging, validation) uniformly. |
| **CQRS-lite with MediatR** (one class per use case: `ComputeGroupOverlapQuery` → `ComputeGroupOverlapHandler`) | Each use case is isolated, independently testable, and you get free cross-cutting behavior via MediatR pipeline behaviors (e.g. a `ValidationBehavior` that runs FluentValidation before every handler, a `LoggingBehavior` that logs every use case invocation). This is the idiomatic pattern in modern .NET Clean Architecture reference implementations, which strengthens the portfolio signal — it shows familiarity with patterns hiring managers recognize. |

**I recommend MediatR**, specifically because it lets us wire up FluentValidation and structured logging as pipeline behaviors from day one (serving NFR-8 and NFR-9) with almost no boilerplate, rather than remembering to call a validator manually in every handler. This is a judgment call, not a requirement — flagging for your sign-off below since it affects the shape of every backend feature we write from Phase 5 onward.

## 5. Cross-Cutting Concerns

### 5.1 Authentication & Authorization flow
- Access token: JWT, short-lived (15 min), signed, contains `userId` + roles claim.
- Refresh token: opaque random string, stored hashed in the database with an expiry and rotation — on refresh, the old token is invalidated and a new one issued (prevents replay of a stolen refresh token going undetected).
- Flutter stores the refresh token in `flutter_secure_storage` (Keychain/Keystore-backed), never in plain shared prefs.
- Every endpoint enforces authorization server-side via a resource-ownership check (e.g. "is this user actually a member of this group?") — never trust a client-supplied ID alone. This directly implements NFR-5.
- Google Sign-In: client obtains a Google ID token, backend verifies it server-side against Google's public keys, then issues our own JWT — the app never trusts Google's token directly for authorization.

### 5.2 Background jobs — the recurrence materialization decision
Flagged in the PRD (Section 8): recurring availability requires a sweep job that expands recurrence rules into concrete instances on a rolling 8-12 week horizon. Two options:

| Option | Trade-off |
|---|---|
| **Hosted background service now** (`IHostedService` / `BackgroundService`, timer-driven) | Zero extra infrastructure — ships inside the existing API process. No dashboard/retry UI, but the job is simple (idempotent upsert) and doesn't need one at MVP scale. |
| Hangfire from day one | Gives you persistent job storage, retry policies, and a dashboard — but it's another moving piece (needs its own DB tables, dashboard auth) to stand up before you have any real load justifying it. |

**Recommendation**: hosted background service now, behind an `IRecurrenceMaterializationJob` interface, so swapping the *trigger mechanism* to Hangfire in Phase 9 (already on your roadmap as a "Future" item) is a small, isolated change — not a rewrite. This avoids standing up infrastructure before it earns its keep, while keeping the door open.

### 5.3 Validation
FluentValidation (already in your stack) runs as a MediatR pipeline behavior — every command/query is validated before its handler executes, so handlers never need defensive null/range checks for input shape.

### 5.4 Error handling
Centralized exception-handling middleware maps domain/application exceptions to consistent HTTP problem-details responses (RFC 7807), so the Flutter client can rely on one error shape everywhere instead of parsing ad hoc error formats per endpoint.

### 5.5 Real-time updates — explicitly deferred
Group availability/voting could theoretically use SignalR for live updates. **Deferring this**: for a friend-group scheduling app, near-real-time (pull-to-refresh, or refresh-on-navigation, plus push notifications for the events that matter) is sufficient — the actual usage pattern is "check the app when you get a notification," not "watch a live dashboard." Adding SignalR now would be complexity without a validated need. Flagging as a V2 candidate if usage patterns show it's wanted.

## 6. Frontend Architecture: Flutter Feature-First Clean Architecture

Mirrors the backend's dependency-direction discipline, scoped per feature instead of globally:

```
features/<feature>/
  data/         # DTOs (Freezed/JSON), remote data sources (Dio calls), repository implementations
  domain/       # Entities, repository interfaces, use cases (plain Dart, zero Flutter/Dio imports)
  presentation/ # Screens, widgets, Riverpod providers/controllers (state)
```

`domain/` has zero dependency on Flutter or Dio — same testability argument as the backend Domain layer, just scoped to the feature.

### 6.1 State management: Riverpod (already in your stack) — why it's the right call
| Option | Trade-off |
|---|---|
| Provider | Predecessor to Riverpod; requires `BuildContext` for reads, weaker compile-time safety. |
| Bloc | Excellent discipline and testability, but noticeably more boilerplate per feature (events + states + bloc classes) for a solo/small-team portfolio pace. |
| GetX | Fast to write, but its service-locator style and lack of compile-time provider safety leads to runtime errors that Riverpod catches at compile time — poor fit for a project meant to demonstrate rigor. |
| **Riverpod** | Compile-safe (no `BuildContext` needed to read providers), doubles as a DI mechanism (no separate `get_it` needed), excellent testability via `ProviderContainer` overrides. |

### 6.2 Networking: Dio + interceptor-based auth
A single `Dio` instance with interceptors: attach JWT to every request, catch 401 → attempt refresh → retry original request once → if refresh fails, force logout. This centralizes token-refresh logic in one place instead of scattering it across every API call site.

### 6.3 Routing: GoRouter — why
Official Flutter-team package (not third-party like `auto_route`), declarative route definitions, and critically: **native deep-link support**, which the Groups invite-link feature (FR from PRD) directly depends on — an invite link needs to deep-link straight into a "join group" screen, including when the app is cold-started.

## 7. Tech Stack Justification Summary

| Choice | Primary alternative(s) | Why this one |
|---|---|---|
| ASP.NET Core / .NET 9 | Node.js+NestJS, Spring Boot | Strong static typing catches errors at compile time; mature EF Core ORM; built-in DI container (no extra library); first-class OpenAPI/Swagger generation; strong performance characteristics for a REST API workload. |
| PostgreSQL | MongoDB, MySQL | Data is inherently relational (users↔friends↔groups↔events↔votes, all with integrity constraints) — a document store would force denormalization and make the overlap-query logic harder to express correctly. Postgres also gives room to grow (PostGIS for geo-search on meeting locations later, JSONB for flexible fields where genuinely needed). |
| Riverpod | Bloc, Provider, GetX | See 6.1 — compile safety + doubles as DI + low boilerplate. |
| GoRouter | auto_route, raw Navigator 2.0 | See 6.3 — official package + deep-link support required by invite-link feature. |
| Dio | `http` package | Interceptor support is not optional here — token refresh and centralized error mapping need it. |
| Docker + Compose | Bare local installs | New contributor (or future-you six months from now) runs `docker compose up` and has Postgres + API running identically to CI — eliminates "works on my machine." |
| Railway (primary deploy target) | Azure | Railway: minutes to a live URL, generous free/hobby tier, trivial Postgres provisioning — right fit for a portfolio demo that needs to *actually be live* for people to click. Azure is documented as the enterprise-equivalent path in Phase 8 (App Service + Azure Database for PostgreSQL) to show you can target either, but Railway is the default so the project has a real, low-friction deployed URL. |

## 8. Backend Folder Structure

```
YouG.sln
/src
  YouG.Domain/
    Entities/           # User, Group, Availability, Event, Vote, FriendRequest, ...
    Enums/               # AvailabilityStatus, EventVoteType, GroupRole, ...
    ValueObjects/        # TimeZoneId, GeoCoordinate, ...
    Exceptions/          # Domain-specific exceptions (e.g. NotGroupMemberException)
  YouG.Application/
    Common/
      Interfaces/        # IAvailabilityRepository, IGroupRepository, ICurrentUserService, INotificationSender, ...
      Behaviors/         # ValidationBehavior, LoggingBehavior (MediatR pipeline)
    Features/
      Availability/
        Commands/        # SetAvailabilityCommand + Handler
        Queries/         # ComputeGroupOverlapQuery + Handler
        Validators/       # FluentValidation validators
        Dtos/
      Groups/ Friends/ Events/ Voting/ Auth/ Notifications/  # same shape per feature
  YouG.Infrastructure/
    Persistence/
      YouGDbContext.cs
      Configurations/    # EF Core IEntityTypeConfiguration per entity
      Repositories/       # Concrete repo implementations
      Migrations/
    Auth/                # JWT generation, Google token verification
    Notifications/        # FCM client implementation
    BackgroundJobs/        # RecurrenceMaterializationJob (IHostedService)
  YouG.API/
    Controllers/
    Middleware/           # ExceptionHandlingMiddleware
    Program.cs             # composition root — DI registration
/tests
  YouG.Domain.Tests/
  YouG.Application.Tests/       # use case tests against fake repositories
  YouG.API.IntegrationTests/    # WebApplicationFactory + Testcontainers Postgres
  YouG.ArchitectureTests/        # NetArchTest dependency-rule enforcement
```

## 9. Flutter Folder Structure

```
lib/
  main.dart
  core/
    network/           # Dio client + interceptors
    router/             # GoRouter config, route guards
    theme/               # Material 3 theme, dark mode
    error/                # Failure types, error mapping
    widgets/               # Shared/reusable widgets
    utils/                   # Timezone conversion helpers, etc.
  features/
    auth/
      data/       # AuthRemoteDataSource, AuthRepositoryImpl, DTOs
      domain/     # User entity, AuthRepository interface, LoginUseCase
      presentation/  # LoginScreen, SignupScreen, authControllerProvider
    profile/
    friends/
    groups/
    availability/
      # includes the heatmap widget + smart time finder UI
    events/
    voting/
    notifications/
    settings/
/test
  # mirrors lib/ structure: unit tests per domain use case,
  # widget tests per presentation component
```

## 10. Deployment Topology (preview — full detail in Phase 8)

```
GitHub Actions CI ──► build + test ──► Docker image ──► Railway (API + Postgres)
                                                      ──► Flutter build ──► (manual) TestFlight / Play internal track
```

Local dev: `docker compose up` runs API + Postgres together; Flutter runs against `localhost` API via `--dart-define` environment config (dev/staging/prod flavors).

## 11. Confirmed Decisions (2026-07-13)

1. **Application layer**: MediatR CQRS-lite, confirmed. Every use case is a Command/Query + Handler; FluentValidation and logging wired in as MediatR pipeline behaviors.
2. **Background jobs**: hosted `BackgroundService` now, behind an `IRecurrenceMaterializationJob` interface, confirmed. Promotable to Hangfire in Phase 9 without touching call sites.
3. **Deploy target**: Railway primary, confirmed. Azure path documented in Phase 8 as the enterprise-equivalent alternative, not built in parallel.

---
**Phase 2 signed off.** Proceeding to Phase 3 (Database Design).
