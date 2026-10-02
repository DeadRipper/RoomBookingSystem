import type { Room } from "../types";

// Thin client for the ASP.NET backend (RoomBookingApp.Controllers.RoomBookingController).
// BookRoomRequest now carries BookingDate and MeetingTitle alongside RoomId — see
// BookingsContext.addBooking for how the local booking model maps onto this request.
//
// UserId is hardcoded to 1 below: the backend added a required UsersId FK on
// Reservations, but there's no user lookup/auth in the app yet, so we can't
// resolve the "Booking as" name to a real DB user id. Revisit once a Users
// endpoint exists.

const BOOK_ROOM_URL = "/api/RoomBooking/bookRoom";
const UNBOOK_ROOM_URL = "/api/RoomBooking/unbookRoom";
const CHANGE_BOOKING_URL = "/api/RoomBooking/changeBookingSettings";

export class RoomBookingApiError extends Error {}

// Thrown specifically when the server explicitly rejected the booking/unbooking
// (BookState.Failed), as opposed to a network/transport failure. Callers use
// this to distinguish "the room couldn't be booked" from "we couldn't tell" —
// only the latter is safe to fall back to local-only storage for.
export class RoomBookingFailedError extends RoomBookingApiError {}

export interface BookRoomParams {
  roomId: number;
  bookingDate: string; // ISO 8601 datetime
  meetingTitle: string;
  userName: string;
}

// See the module-level comment above: there's no user lookup yet, so every
// booking is attributed to this fixed backend user id.
const HARDCODED_USER_ID = 1;

// Mirrors RBA.Models.States.BookState. System.Text.Json serializes enums as
// their numeric value by default (no JsonStringEnumConverter is registered).
const BookState = {
  Confirmed: 0,
  Pending: 1,
  Cancelled: 2,
  Failed: 3,
} as const;
type BookState = (typeof BookState)[keyof typeof BookState];

interface BookRoomResponse {
  RequestId?: string;
  RoomId?: number | null;
  BookState?: BookState | null;
  RoomState?: number | null;
  Error_msg?: string | null;
}

interface UnbookRoomResponse {
  RequestId: string;
  RoomId: number;
  BookState: BookState;
}

// Every endpoint now builds its body via JsonBuildHelper.BuildJsonResponse,
// which JsonSerializer.Serialize()s the response object into a string and
// then hands that STRING to Ok(...) — so the actual HTTP body is a JSON
// string literal containing the real (PascalCase) JSON payload, doubly
// encoded. Undo both layers here.
async function parseWrappedJson<T>(response: Response): Promise<T> {
  const outer = await response.text();
  const inner = JSON.parse(outer) as string;
  return JSON.parse(inner) as T;
}

export async function bookRoomOnServer({
  roomId,
  bookingDate,
  meetingTitle,
  userName,
}: BookRoomParams): Promise<void> {
  let response: Response;
  try {
    response = await fetch(BOOK_ROOM_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        roomId,
        bookingDate,
        meetingTitle,
        userId: HARDCODED_USER_ID,
        userName,
      }),
    });
  } catch (cause) {
    throw new RoomBookingApiError("Could not reach the booking server.", { cause });
  }

  if (!response.ok) {
    const errorText = await response.text().catch(() => "");
    throw new RoomBookingApiError(
      `Booking server rejected room ${roomId} (${response.status}): ${errorText}`,
    );
  }

  // The controller always replies 200 OK, even when the booking itself
  // failed (e.g. unknown room), so the outcome has to be read from the body.
  const result: BookRoomResponse = await parseWrappedJson<BookRoomResponse>(response).catch(
    () => ({}),
  );
  if (result.BookState === BookState.Failed) {
    throw new RoomBookingFailedError(
      result.Error_msg || `Booking server could not book room ${roomId}.`,
    );
  }
}

export interface UnbookRoomParams {
  roomId: number;
}

export async function unbookRoomOnServer({ roomId }: UnbookRoomParams): Promise<void> {
  let response: Response;
  try {
    response = await fetch(UNBOOK_ROOM_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ roomId }),
    });
  } catch (cause) {
    throw new RoomBookingApiError("Could not reach the booking server.", { cause });
  }

  if (!response.ok) {
    const errorText = await response.text().catch(() => "");
    throw new RoomBookingApiError(
      `Booking server rejected unbooking room ${roomId} (${response.status}): ${errorText}`,
    );
  }

  // Unlike before, the controller now reports an explicit BookState (e.g.
  // Failed when the room doesn't exist), so surface that the same way booking does.
  const result = await parseWrappedJson<UnbookRoomResponse>(response).catch(
    () => null,
  );
  if (result?.BookState === BookState.Failed) {
    throw new RoomBookingFailedError(`Booking server could not unbook room ${roomId}.`);
  }
}

// Mirrors RBA.Models.States.ChangesState (note the backend's "Penging" typo).
const ChangesState = {
  Accepted: 0,
  Pending: 1,
  Applied: 2,
  NotApplied: 3,
  Failed: 4,
} as const;

export interface ChangeBookingParams {
  roomId: number;
  bookingDate: string; // ISO 8601 datetime
  userName: string;
}

// Unlike the other endpoints, changeBookingSettings returns Ok(ChangesState)
// directly, so the body is a plain JSON number rather than a doubly-encoded
// string. The backend only queues the change (Accepted) - it isn't applied yet.
export async function changeBookingOnServer({
  roomId,
  bookingDate,
  userName,
}: ChangeBookingParams): Promise<void> {
  let response: Response;
  try {
    response = await fetch(CHANGE_BOOKING_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        roomId,
        date: bookingDate,
        users: { id: HARDCODED_USER_ID, userName },
      }),
    });
  } catch (cause) {
    throw new RoomBookingApiError("Could not reach the booking server.", { cause });
  }

  if (!response.ok) {
    const errorText = await response.text().catch(() => "");
    throw new RoomBookingApiError(
      `Booking server rejected changes to room ${roomId} (${response.status}): ${errorText}`,
    );
  }

  const state = await response.json().catch(() => null);
  if (state === ChangesState.Failed || state === ChangesState.NotApplied) {
    throw new RoomBookingFailedError(
      `Booking server could not change the booking for room ${roomId}.`,
    );
  }
}

// --- Room info (GetRoomInfoController.roomInfo) ---------------------------
// Returns Ok(string) where the string is System.Text.Json output of the
// RoomModel list (PascalCase, enums numeric, Floor is an int, Amenities is a
// single AmenityModel). A null result yields the literal text "no rooms".

const ROOM_INFO_URL = "/api/GetRoomInfo/roomInfo";

interface RoomModelDto {
  Id?: number;
  Name?: string;
  Floor?: number;
  Capacity?: number;
  Amenities?: { Id?: number; Name?: string } | null;
  Image?: string | null;
}

export async function fetchRoomsFromServer(): Promise<Room[]> {
  let response: Response;
  try {
    response = await fetch(ROOM_INFO_URL, { method: "POST" });
  } catch (cause) {
    throw new RoomBookingApiError("Could not reach the booking server.", { cause });
  }
  if (!response.ok) {
    throw new RoomBookingApiError(`Booking server could not list rooms (${response.status}).`);
  }

  const text = await response.text();
  let dtos: unknown;
  try {
    dtos = JSON.parse(text);
  } catch {
    return []; // "no rooms"
  }
  if (!Array.isArray(dtos)) return [];

  return (dtos as RoomModelDto[]).map((d) => ({
    id: d.Id ?? 0,
    name: d.Name ?? `Room ${d.Id}`,
    floor: `Floor ${d.Floor ?? "?"}`,
    capacity: d.Capacity ?? 0,
    amenities: d.Amenities?.Name ? [d.Amenities.Name] : [],
    image: d.Image ?? "",
  }));
}

// --- Admin login (AdminController.login) ----------------------------------
// 200 OK on success, 401 Unauthorized on a bad id/password.

const ADMIN_LOGIN_URL = "/api/Admin/login";

export async function adminLogin(id: number, password: string): Promise<boolean> {
  let response: Response;
  try {
    response = await fetch(ADMIN_LOGIN_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ id, password }),
    });
  } catch (cause) {
    throw new RoomBookingApiError("Could not reach the booking server.", { cause });
  }
  if (response.status === 401) return false;
  if (!response.ok) {
    throw new RoomBookingApiError(`Login failed (${response.status}).`);
  }
  return true;
}
