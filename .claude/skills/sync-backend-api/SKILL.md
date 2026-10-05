---
name: sync-backend-api
description: Fetch the latest commits on the backend's current git branch, find new or changed API endpoints and models, and implement matching support in the React frontend (src/api/roomBookingApi.ts and the UI that uses it). Use when the user says the backend was updated, a new controller/endpoint was added, or asks to "sync the front with the backend".
---

# Sync frontend with backend API

The ASP.NET backend lives in the **superproject** that contains this frontend repo
(`git rev-parse --show-superproject-working-tree`, normally `C:\repos\RoomBooking`).
The frontend is `RoomBooking-frontend/` inside it. The backend branch it has checked out
(e.g. `cabinet`) is the source of truth. Never edit backend files — only report problems in them.

## 1. Locate backend and pull in the latest

```bash
BACK=$(git rev-parse --show-superproject-working-tree)   # run from the frontend dir
git -C "$BACK" branch --show-current
git -C "$BACK" fetch --quiet
git -C "$BACK" status -sb | head -1                      # ahead/behind upstream
```

- If the branch is behind upstream and the backend working tree is clean (ignoring the
  `RoomBooking-frontend` submodule entry), `git -C "$BACK" pull --ff-only`.
- If the tree has uncommitted backend edits, do **not** pull or stash. Read the backend as it
  is on disk (the user's uncommitted work is usually the newest) and say so.
- Tell the user which branch and commit you are reading.

## 2. Find what changed since the last sync

The last synced backend commit is stored in `.claude/backend-sync.json` (`{"commit": "<sha>"}`),
in the frontend repo. If it is missing, diff against the commit where the frontend last touched
the API (`git log -1 --format=%H -- src/api/roomBookingApi.ts` date, then pick the backend commit
at that date) or ask the user for a starting point.

```bash
git -C "$BACK" log --oneline <last>..HEAD
git -C "$BACK" diff <last>..HEAD --stat -- '*.cs' ':!*Migrations*' ':!*Designer.cs'
git -C "$BACK" diff <last>..HEAD -- RoomBookingApp/RoomBookingApp/Controllers RoomBookingApp/RBA.Models
```

Also include uncommitted changes (`git -C "$BACK" diff HEAD -- <same paths>`).
Ignore migrations, `.sql` seed scripts and middleware unless they change the API contract.

## 3. Work out the contract of each new/changed endpoint

For every controller action in the diff, read the full controller plus the request/response
models and note:

- **Route and verb**: `[Route("api/[controller]")]` + `[HttpPost("name")]`/`[HttpGet]` →
  `/api/<Controller without "Controller">/<name>`. The Vite dev proxy already forwards `/api`.
- **Request body**: class properties → JSON keys (camelCase is accepted on input).
  Non-nullable reference types are *required*; a missing/`null`/`NaN` value gives a 400.
- **Response shape** — check how the controller builds it, this has changed before:
  - `Ok(obj)` → plain **camelCase** JSON.
  - `Ok(string)` containing `JsonSerializer.Serialize(...)` → raw text that is **PascalCase** JSON
    (`getAllrooms`), or a doubly-encoded string if it went through `JsonBuildHelper`.
  - `Ok()` / `Unauthorized()` / `BadRequest()` → status code only; the status *is* the result.
  - Un-awaited `Task` passed to `Ok(...)` serializes as an object with `result`; flag it.
  - Enums are serialized as **numbers** (no string converter) — mirror them as `as const` objects.
- **Foreign keys / required relations** the UI must supply (e.g. amenity id, user id).

## 4. Implement on the frontend

All HTTP goes through `src/api/roomBookingApi.ts`:

- Add a typed function per endpoint using `loggedFetch` (never bare `fetch`, so requests and
  responses are logged under `[RBA API]` in the console).
- Wrap transport failures in `RoomBookingApiError`; throw `RoomBookingFailedError` when the
  server explicitly refused (e.g. `BookState.Failed`). Map server DTOs to the frontend types in
  `src/types.ts` inside the API module — components never see raw DTOs.
- Reuse `parseWrappedJson` for booking-style responses (it accepts plain and double-encoded,
  and normalises keys to PascalCase).
- Wire the call into the UI: shared server data goes in a context under `src/store/` (see
  `RoomsContext`, which also de-duplicates the StrictMode double effect); admin screens live
  under `src/pages/Admin*` / `src/components/`, routes in `src/App.tsx`, gated by `RequireAdmin`.
- Show a loading placeholder (`…`) while fetching rather than a hard-coded fallback value;
  only fall back after the request fails.
- Match the surrounding code style and comment density. Comments say *why* and document the
  backend contract (route, shape, quirks), as the existing ones do.

## 5. Verify

```bash
npx tsc -b            # must be clean
npx oxlint src        # only the existing react-refresh warnings are acceptable
```

Check `git status` for stray files. You cannot run the backend, so say clearly that nothing was
exercised against it.

## 6. Record the sync and report

- Write the backend commit you synced to into `.claude/backend-sync.json`.
- Do **not** commit unless the user asks.
- Report in a few lines: backend branch/commit, endpoints added or changed, files touched on the
  front, and **backend problems you noticed** (missing `await`, endpoints returning names without
  ids, inconsistent JSON casing, update-vs-insert mistakes, unvalidated nullable fields, etc.)
  so the user can fix them on the backend.

## Known endpoints (as of the last sync, for orientation only — re-read the code)

| Route | Front function |
|---|---|
| `POST /api/RoomBooking/getAllrooms` | `fetchRoomsFromServer` |
| `POST /api/RoomBooking/bookRoom` / `unbookRoom` / `changeBookingSettings` | `bookRoomOnServer` / `unbookRoomOnServer` / `changeBookingOnServer` |
| `POST /api/Admin/login` / `logout` | `adminLogin` / `adminLogout` |
| `POST /api/User/registrate` / `getUserId` / `getUserById`, `GET /api/User/getAllUsersId` | `registerUser` / `fetchUserId` (used by booking) / `fetchUserNameById` (admin table), `fetchAllUserIds` |
| `POST /api/Admin/addRoom` | `addRoomOnServer` |
| `GET /api/Admin/getRoomConfigs` | `fetchAmenitiesFromServer` |
| `GET /api/Admin/getAllReservations` (ids only: userId, roomId) | `fetchAllReservations` |
| `GET /api/Admin/totalBookings` / `getTodayBookings` / `getAllRoomsCount` | `fetchTotalBookings` / `fetchTodayBookings` / `fetchRoomsCount` |
| `POST /api/RoomBooking/checkAvailable` | not used by the front yet |
