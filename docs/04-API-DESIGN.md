# You G? — REST API Design

Status: **Signed off — 2026-07-13**
Last updated: 2026-07-13

## 1. Conventions (why before what)

### 1.1 Versioning: URL segment, `/api/v1/...`
| Option | Trade-off |
|---|---|
| **URL segment** (`/api/v1/groups`) | Explicit, visible in every log line and Swagger doc, trivial for the Flutter client to point at a fixed base URL per environment. Slightly "impure" REST-wise but overwhelmingly the pragmatic default. |
| Header-based (`Api-Version: 1.0`) | "Purer" but invisible in logs/browser testing, and adds a header the Flutter Dio client must remember to set on every request — an easy thing to forget mid-project. |

**Chosen: URL segment**, via the `Asp.Versioning.Mvc` package so version routing is declarative on controllers, not hand-rolled.

### 1.2 Resource naming
- Plural nouns for collections: `/groups`, `/events`, not `/group`, `/getEvents`.
- Nesting reflects ownership, capped at one level: `/groups/{groupId}/events` (an event belongs to a group), but `/events/{eventId}` for direct access once you have the ID (avoids `/groups/{groupId}/events/{eventId}/time-options/{optionId}/votes` sprawl).
- Actions that aren't pure CRUD are modeled as sub-resources, not verbs in the path: `POST /friends/requests/{id}/accept` is avoided in favor of `PUT /friends/requests/{id}` with a `status` body — kept as a resource state transition, not an RPC-style verb endpoint. Exception: a small number of genuine actions with no natural resource (e.g. `POST /groups/join/{code}`) are fine as verbs — forcing everything into pure REST when it doesn't fit is its own anti-pattern.

### 1.3 JSON casing
`camelCase` on the wire (ASP.NET Core's default JSON serializer setting), matching Dart/Flutter conventions naturally — no translation layer needed client-side.

### 1.4 Pagination
**Offset-based** (`?page=1&pageSize=20`) for MVP, not cursor-based. Cursor pagination solves a problem (consistent pagination under high write-concurrency) that doesn't exist yet at this scale — friend lists, group lists, and notification feeds for a single user are not high-volume enough to need it. Flagged as a Phase 9 revisit if the Notifications feed specifically grows large per user.

### 1.5 Error format: RFC 7807 Problem Details
Every non-2xx response (already decided in Phase 2, Section 5.4) returns:
```json
{
  "type": "https://youg.app/errors/not-group-member",
  "title": "You are not a member of this group",
  "status": 403,
  "traceId": "00-4bf9...-01"
}
```
`type` is a stable machine-readable slug the Flutter client can switch on for custom error UI; `title` is human-readable fallback text.

### 1.6 Status code conventions
| Code | Used for |
|---|---|
| 200 | Successful GET/PUT/PATCH with a response body |
| 201 | Successful POST that creates a resource — `Location` header points to the new resource |
| 204 | Successful DELETE, or POST/PUT with no meaningful response body |
| 400 | Validation failure (FluentValidation pipeline behavior output) |
| 401 | Missing/invalid/expired JWT |
| 403 | Authenticated but not authorized for this resource (e.g. not a group member) |
| 404 | Resource doesn't exist — or exists but the requester shouldn't know that (see 1.7) |
| 409 | Conflict (e.g. duplicate friend request, duplicate vote) |
| 429 | Rate limited (Phase 9) |

### 1.7 Authorization leakage
`404` is deliberately returned instead of `403` where distinguishing "doesn't exist" from "exists but you can't see it" would leak information — e.g. requesting a group you're not a member of returns `404`, not `403`, so you can't enumerate valid group IDs you don't have access to. `403` is reserved for cases where the resource's existence is already known/expected (e.g. you're a group member but not an admin, trying to remove another member).

## 2. Endpoint Inventory

### 2.1 Auth (`/api/v1/auth`)
| Method | Path | Auth | Notes |
|---|---|---|---|
| POST | `/auth/register` | none | email/password signup |
| POST | `/auth/login` | none | email/password |
| POST | `/auth/google` | none | exchanges Google ID token for our JWT |
| POST | `/auth/refresh` | refresh token | rotates refresh token, issues new access token |
| POST | `/auth/logout` | JWT | revokes the current refresh token |
| POST | `/auth/forgot-password` | none | sends reset email; always 202 regardless of whether email exists (prevents account enumeration) |
| POST | `/auth/reset-password` | reset token | |

### 2.2 Users / Profile (`/api/v1/users`)
| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/users/me` | JWT | full profile |
| PATCH | `/users/me` | JWT | bio, displayName, timezone, profile picture |
| DELETE | `/users/me` | JWT | soft-delete per Phase 3 decision |
| GET | `/users/{id}` | JWT | public profile subset; visibility per friend/group rules |
| GET | `/users/search?q=` | JWT | by username |
| GET | `/users/me/friend-code` | JWT | |
| PATCH | `/users/me/settings` | JWT | dark mode, privacy, notification prefs |

### 2.3 Friends (`/api/v1/friends`)
| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/friends` | JWT | accepted friends list |
| POST | `/friends/requests` | JWT | body: `{ addresseeId }` or `{ friendCode }` |
| GET | `/friends/requests?direction=incoming\|outgoing` | JWT | |
| PUT | `/friends/requests/{id}` | JWT | body: `{ status: "Accepted" \| "Declined" }` |
| DELETE | `/friends/{userId}` | JWT | unfriend |
| PATCH | `/friends/{userId}` | JWT | body: `{ isFavorite: true }` |
| POST | `/blocks` | JWT | body: `{ blockedUserId }` |
| DELETE | `/blocks/{userId}` | JWT | |

### 2.4 Groups (`/api/v1/groups`)
| Method | Path | Auth | Notes |
|---|---|---|---|
| POST | `/groups` | JWT | |
| GET | `/groups` | JWT | groups the caller is a member of |
| GET | `/groups/{id}` | JWT + member | 404 if not a member (1.7) |
| PATCH | `/groups/{id}` | JWT + admin | |
| DELETE | `/groups/{id}/members/me` | JWT + member | leave group |
| GET | `/groups/{id}/members` | JWT + member | |
| DELETE | `/groups/{id}/members/{userId}` | JWT + admin | |
| PATCH | `/groups/{id}/members/{userId}` | JWT + admin | role change |
| POST | `/groups/{id}/invite-links` | JWT + admin | returns `{ code, expiresAt }` |
| POST | `/groups/join/{code}` | JWT | resolves an invite code, joins caller |

### 2.5 Availability (`/api/v1/availability`)
| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/availability/me/rules` | JWT | recurrence rules |
| POST | `/availability/me/rules` | JWT | |
| DELETE | `/availability/me/rules/{id}` | JWT | |
| GET | `/availability/me/instances?from=&to=` | JWT | materialized view, editable calendar |
| PATCH | `/availability/me/instances` | JWT | body: array of `{ date, daypart, status }` upserts — one-off overrides |

### 2.6 Smart Time Finder & Heatmap (`/api/v1/groups/{id}`)
| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/groups/{id}/overlap?from=&to=&weekendOnly=&preferredStart=&preferredEnd=` | JWT + member | ranked overlap windows |
| GET | `/groups/{id}/heatmap?from=&to=` | JWT + member | day×daypart grid |

### 2.7 Events & Voting (`/api/v1/events`, nested under groups for creation)
| Method | Path | Auth | Notes |
|---|---|---|---|
| POST | `/groups/{id}/events` | JWT + member | |
| GET | `/groups/{id}/events` | JWT + member | |
| GET | `/events/{id}` | JWT + attendee-eligible | |
| PATCH | `/events/{id}` | JWT + organizer | |
| DELETE | `/events/{id}` | JWT + organizer | cancels |
| POST | `/events/{id}/time-options` | JWT + member | propose a candidate time |
| PUT | `/events/{id}/time-options/{optionId}/vote` | JWT + member | idempotent: voting again just confirms |
| DELETE | `/events/{id}/time-options/{optionId}/vote` | JWT + member | retract vote |
| POST | `/events/{id}/location-options` | JWT + member | mirrors time-options |
| PUT/DELETE | `/events/{id}/location-options/{optionId}/vote` | JWT + member | |
| POST | `/events/{id}/confirm` | JWT + organizer | body: `{ timeOptionId, locationOptionId }` → sets `Events.Status = Confirmed` |
| PUT | `/events/{id}/attendance` | JWT + invitee | body: `{ status: "Going" \| "Maybe" \| "CantGo" }` |

### 2.8 Notifications (`/api/v1/notifications`)
| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/notifications?page=&pageSize=` | JWT | |
| PATCH | `/notifications/{id}` | JWT | body: `{ isRead: true }` |
| POST | `/notifications/read-all` | JWT | |
| POST | `/device-tokens` | JWT | register FCM token |
| DELETE | `/device-tokens/{token}` | JWT | on logout |

### 2.9 Search (`/api/v1/search`)
| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/search?q=&type=friends\|groups\|events` | JWT | unified search endpoint, `type` narrows scope |

## 3. Representative DTOs

Showing the non-trivial ones — pure CRUD DTOs (e.g. `UpdateProfileRequest`) are self-evident from the table schema in Phase 3 and don't need spelling out here.

### 3.1 `POST /auth/register`
Request:
```json
{ "email": "maya@example.com", "password": "correct-horse-battery", "username": "maya22", "displayName": "Maya", "timeZoneId": "Australia/Sydney" }
```
Response `201`:
```json
{
  "accessToken": "eyJhbGciOi...",
  "refreshToken": "8f4e2c1a-...",
  "user": { "id": "018f...", "username": "maya22", "friendCode": "YG-7F3K2A" }
}
```

### 3.2 `GET /groups/{id}/overlap`
Response `200`:
```json
{
  "groupId": "018f...",
  "windows": [
    {
      "date": "2026-07-17",
      "daypart": "Evening",
      "availableUserIds": ["018f...", "019a...", "01a2..."],
      "availableCount": 3,
      "totalMembers": 4,
      "maybeUserIds": ["01b3..."]
    }
  ]
}
```
Ranked by `availableCount` descending server-side — the client renders in the order returned rather than re-sorting, keeping ranking logic in one place (the domain, not duplicated in Flutter).

### 3.3 `GET /groups/{id}/heatmap`
Response `200`:
```json
{
  "groupId": "018f...",
  "from": "2026-07-14",
  "to": "2026-07-20",
  "cells": [
    { "date": "2026-07-14", "daypart": "Morning", "availableCount": 1 },
    { "date": "2026-07-14", "daypart": "Evening", "availableCount": 4 }
  ],
  "totalMembers": 4
}
```
Deliberately a flat cell list rather than a nested `date → daypart → count` object — flat lists serialize smaller and map directly onto a Flutter `GridView`/heatmap widget without a client-side reshape step.

### 3.4 `PUT /events/{id}/attendance`
Request:
```json
{ "status": "Going" }
```
Response `200`:
```json
{ "eventId": "018f...", "userId": "019a...", "status": "Going", "respondedAt": "2026-07-13T10:15:00Z" }
```

### 3.5 Validation error example (`400`)
```json
{
  "type": "https://youg.app/errors/validation-failed",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": { "password": ["Password must be at least 8 characters."] }
}
```

## 4. Authorization Matrix (summary)

| Resource | Rule |
|---|---|
| User profile (full) | self only |
| User profile (public subset) | any authenticated user (needed for search/friend-code add flow) |
| Availability instances | self (write); group co-members (read, per Phase 1 group-visibility decision) |
| Group | members only (404 otherwise, per 1.7) |
| Group admin actions | `GroupMembers.Role = Admin` |
| Event | group members of the event's group |
| Event edit/cancel/confirm | event's `CreatedByUserId` (organizer) — not just any group member |
| Votes/attendance | any group member, but only mutate their own vote/attendance row |

Every one of these is enforced server-side in the MediatR handler (or a shared `IAuthorizationBehavior` pipeline step for the common "is member of group X" check) — never trusted from a client-supplied flag, per NFR-5.

## 5. Confirmed Decision (2026-07-13)

**Pagination**: offset-based (`page`/`pageSize`), confirmed, per Section 1.4. Revisit for Notifications specifically in Phase 9 if that feed grows large enough to need cursor semantics.

---
**Phase 4 signed off.** Proceeding to Phase 5 (Backend Development), where these endpoints get implemented as MediatR commands/queries + controllers.
