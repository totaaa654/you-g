# You G? — Product Requirements Document

Status: **Signed off — 2026-07-13**
Owner: Jozza Galang
Last updated: 2026-07-13

## 1. Problem Statement

Coordinating "when are we free?" across a friend group today happens through screenshots of calendars, endless group-chat back-and-forth ("I'm free Tues", "not me, how about Wed?"), or generic scheduling tools built for *business* meetings between two people, not *social* meetups among many. There is no consumer-friendly tool that:

- Lets a group passively share rolling availability instead of re-negotiating every time.
- Automatically surfaces the best overlapping windows instead of requiring manual comparison.
- Turns that overlap into a concrete event with voting on time/place, not just a "poll."

**You G?** solves this by making availability a persistent, low-effort profile (like a calendar you set once and update occasionally) rather than a one-off scheduling negotiation.

## 2. Target User & Primary Use Case

Per product decision: MVP optimizes for **casual friend groups of 5–15 people** trying to find a time to hang out — not recurring work teams, not large public events. Design/UX priority order:

1. Minimize effort to declare availability (must be faster than replying in a group chat).
2. Make overlap obvious at a glance (heatmap, not manual comparison).
3. Make going from "we're free Thursday evening" → "confirmed event" a 2-tap flow.

## 3. Personas

- **Maya, 22, group organizer.** Always the one trying to herd 8 friends into picking a time. Wants to stop being the group's unpaid scheduling assistant.
- **Dev, 24, passive participant.** Will update his availability if it takes <10 seconds, will not fill out a form. Wants to just get notified when something's decided and vote yes/no.

## 4. Epics & User Stories

Story format: *As a [persona], I want to [action], so that [outcome].* Each tagged with MVP or V2.

### 4.1 Authentication & Onboarding (MVP)
- As a new user, I want to sign up with email/password or Google, so I can start using the app without friction.
- As a user, I want to reset a forgotten password via email, so I'm not locked out permanently.
- As a new user, I want to set a username and profile picture during onboarding, so friends can identify me.
- As a user, I want a unique friend code, so people can add me without knowing my email.

### 4.2 Profile (MVP)
- As a user, I want to set my timezone, so my availability displays correctly to friends in other timezones.
- As a user, I want to view my friends list and mutual friends with someone, so I understand my social graph.
- As a user, I want a personal availability calendar, so my free/busy status is a standing profile, not re-entered per event.

### 4.3 Friends (MVP)
- As a user, I want to search for other users by username, so I can send friend requests.
- As a user, I want to accept/decline incoming friend requests, so I control my connections.
- As a user, I want to remove or block a friend, so I can manage unwanted contacts.
- As a user, I want to mark a friend as favorite, so they're prioritized in group-creation flows.

### 4.4 Groups (MVP)
- As a user, I want to create a group and invite friends, so we have a persistent space for our recurring hangouts.
- As a user, I want to join a group via invite link, so I don't need to already be friends with everyone in it.
- As a group admin, I want to remove members or assign roles, so I can manage the group.
- As a user, I want to leave a group, so I'm not stuck in stale groups.

### 4.5 Availability & Smart Time Finder (MVP — core differentiator)
- As a user, I want to mark myself Available/Busy/Maybe per daypart (morning/afternoon/evening/night) or whole day, so my group can see when I'm free without needing hour-level precision.
- As a user, I want to set recurring availability (e.g. "free every Friday evening"), so I don't re-enter it weekly.
- As a group member, I want the app to automatically compute the best overlapping time across the group, so no one has to manually cross-reference schedules.
- As a group member, I want to filter suggestions by weekends-only or "after work/school," so results match my real constraints.
- As a group member, I want to see a visual heatmap of group availability, so I can eyeball good times myself, not just trust the algorithm.

### 4.6 Events & Voting (MVP)
- As a group member, I want to create an event with a date, time, description, and location, so we have something concrete to plan around.
- As an invitee, I want to vote Going/Maybe/Can't Go, so the organizer knows attendance.
- As an invitee, I want to vote on proposed times/locations when the organizer hasn't finalized one, so the group converges democratically.
- As an organizer, I want to cap max attendees, so the event doesn't overflow the venue.

### 4.7 Maps (MVP)
- As a user, I want to see the event location on a map, so I know where to go.

### 4.8 Notifications & Reminders (MVP)
- As a user, I want a push notification for friend requests, group invites, and event reminders, so I don't have to keep checking the app.
- As a user, I want a reminder to update my availability if it's stale, so group results stay accurate.

### 4.9 Search & Settings (MVP)
- As a user, I want to search friends, groups, and events in one place, so I can navigate quickly.
- As a user, I want dark mode, privacy controls, and account deletion, so the app respects baseline user expectations.

### 4.10 V2 (explicitly out of MVP scope)
- Google Calendar sync, AI-suggested times, travel-time optimization, QR join, statistics/streaks/achievements.

## 5. Functional Requirements

| ID | Requirement |
|---|---|
| FR-1 | System shall authenticate users via email/password and Google OAuth, issuing JWT access + refresh tokens. |
| FR-2 | System shall allow a user to define recurring and one-off availability blocks with status: Available, Busy, Maybe, Unknown. |
| FR-3 | System shall compute overlapping availability across all members of a group for a given date range, ranked by overlap duration. |
| FR-4 | System shall support filters on the overlap computation: weekend-only, preferred time-of-day window. |
| FR-5 | System shall render group availability as a heatmap (day × hour grid, intensity = number of available members). |
| FR-6 | System shall allow creating an event linked to a group, with date/time, location (lat/lng + address), description, and optional max attendee cap. |
| FR-7 | System shall support voting on event attendance (Going/Maybe/Can't Go) and, pre-finalization, on candidate times/locations. |
| FR-8 | System shall send push notifications (FCM) for: friend request received, group invite received, event created/updated, event reminder (T-24h configurable). |
| FR-9 | System shall enforce friend-relationship visibility rules: only accepted friends see each other's availability, unless in a shared group. |
| FR-10 | System shall support blocking a user, which hides all mutual visibility bidirectionally. |
| FR-11 | System shall allow account deletion, cascading removal/anonymization of the user's personal data per privacy requirements. |

## 6. Non-Functional Requirements

| ID | Category | Requirement |
|---|---|---|
| NFR-1 | Performance | Overlap computation for a group of 15 members over a 7-day window shall return in <300ms server-side at p95. |
| NFR-2 | Scalability | Backend shall be stateless (JWT, no server session) so it can scale horizontally behind a load balancer. |
| NFR-3 | Availability | API uptime target 99.5% (portfolio-appropriate; not claiming enterprise SLA). |
| NFR-4 | Security | Passwords hashed with a modern adaptive hash (e.g. bcrypt/argon2); all traffic over TLS; JWT short-lived with refresh rotation. |
| NFR-5 | Security | Authorization enforced server-side on every endpoint (never trust client-supplied user/group IDs without ownership checks). |
| NFR-6 | Privacy | Availability data visible only per FR-9 visibility rules — enforced at the query layer, not just UI. |
| NFR-7 | Maintainability | Backend follows Clean Architecture with enforced dependency direction (Domain has zero external dependencies); Flutter follows feature-first structure. |
| NFR-8 | Testability | Core business logic (overlap algorithm, voting resolution) must have unit test coverage; enforced in CI. |
| NFR-9 | Observability | Structured logging on the backend from day one (even before Serilog/Grafana are added in Phase 9), so production issues are diagnosable. |
| NFR-10 | Internationalization | All timestamps stored in UTC; converted to user timezone client-side, so cross-timezone groups display correctly. |
| NFR-11 | Accessibility | Flutter UI meets basic Material 3 accessibility defaults (contrast, tap target size, screen-reader labels on interactive elements). |

## 7. MVP Definition

**In scope for MVP (v1.0):**
Auth (email/password + Google), profile setup, friends (add/accept/remove/block), groups (create/invite/join/leave/roles), availability (manual + recurring, whole-day + hourly), Smart Time Finder with weekend/time-of-day filters, heatmap, events (create/edit), voting (attendance + time/location), maps for event location, push notifications for the four core triggers, search, dark mode, privacy settings, account deletion.

**Explicitly out of MVP (v2+):** Google Calendar sync, AI suggestions, travel-time optimization, QR join, invite-link *analytics*, statistics/achievements/streaks.

**MVP success criteria** (portfolio-appropriate, not growth-metric-driven):
- A group of friends can go from "no plan" to "confirmed event with a voted time and location" entirely inside the app, in under 5 minutes, for a group where members have pre-set availability.
- Clean Architecture boundaries hold up under test — domain layer has zero framework dependencies, verified by an architecture/dependency test in CI.
- CI pipeline runs backend + Flutter tests and blocks merge on failure.

## 8. Resolved Design Decisions (2026-07-13)

These were open questions during drafting; resolved before Phase 2 to unblock architecture and DB design:

1. **Availability granularity**: daypart buckets (Morning / Afternoon / Evening / Night) + whole-day, not hourly. Matches how friend groups actually think ("free Thursday evening") and avoids building a calendar-grade precision engine for MVP.
2. **Recurrence storage**: materialized instances, not runtime RRULE expansion. A background job expands each user's recurrence rule into concrete `(date, daypart, status)` rows for a rolling ~8-12 week horizon. Overlap queries run as plain SQL over real rows — no rule-parsing in the hot path. Editing a recurrence rule regenerates future (not past) instances.
3. **Group visibility model**: group membership alone grants availability visibility within that group's context — mutual friendship is *not* required. This matches real usage (you can be in a shared group with people you haven't 1:1-friended in-app) and simplifies the authorization model: visibility is scoped to "shared group" as the single rule, not two overlapping rules (friend OR group).
4. **Invite links**: expiring (default 7 days) and multi-use during that window, then auto-invalidated. Balances share-once-in-a-group-chat convenience against a leaked link staying exploitable forever.

Implication flagged for Phase 3: this means we need a scheduled/background job (Hangfire, listed under Future infra) to run the recurrence-materialization sweep — worth deciding in Phase 2 whether this ships from day one or is stubbed as a manual admin trigger for MVP and promoted to Hangfire in Phase 9.

---
**Sign-off needed:** Please review Sections 4–8. Flag anything mis-scoped before we move to Phase 2 (System Architecture).
