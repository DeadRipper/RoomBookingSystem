import type { Room } from "../types";

// Wraps fetch with console logging of the request and the response (status and
// body), so 4xx/5xx errors - e.g. a 400 model-validation failure from ASP.NET,
// whose body lists the offending fields - are visible in the browser console.
async function loggedFetch(url: string, init: RequestInit = {}): Promise<Response> {
  const method = init.method ?? "GET";
  const label = `[RBA API] ${method} ${url}`;
  const requestBody = typeof init.body === "string" ? init.body : undefined;
  console.log(`${label} -> request`, requestBody ? safeParse(requestBody) : "(no body)");

  const started = performance.now();
  let response: Response;
  try {
    response = await fetch(url, init);
  } catch (err) {
    console.error(`${label} -> network error (no response)`, err);
    throw err;
  }

  const ms = Math.round(performance.now() - started);
  // Clone so callers can still read the body.
  const text = await response.clone().text().catch(() => "");
  const log = response.ok ? console.log : console.error;
  log(`${label} -> ${response.status} ${response.statusText} (${ms}ms)
${pretty(safeParse(text))}`);
  return response;
}

function safeParse(text: string): unknown {
  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
}

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

// The backend has returned these bodies in two shapes over time: a plain
// camelCase JSON object (Ok(response), current), and earlier a doubly-encoded
// PascalCase string (Ok(JsonSerializer.Serialize(...))). Accept both and
// normalise keys to PascalCase so callers can rely on the C# property names.
async function parseWrappedJson<T>(response: Response): Promise<T> {
  let value: unknown = JSON.parse(await response.text());
  if (typeof value === "string") value = JSON.parse(value);
  return capitaliseKeys(value) as T;
}

function capitaliseKeys(value: unknown): unknown {
  if (Array.isArray(value)) return value.map(capitaliseKeys);
  if (value && typeof value === "object") {
    return Object.fromEntries(
      Object.entries(value).map(([k, v]) => [k.charAt(0).toUpperCase() + k.slice(1), v]),
    );
  }
  return value;
}

export async function bookRoomOnServer({
  roomId,
  bookingDate,
  meetingTitle,
  userName,
}: BookRoomParams): Promise<void> {
  let response: Response;
  try {
    response = await loggedFetch(BOOK_ROOM_URL, {
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
    response = await loggedFetch(UNBOOK_ROOM_URL, {
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
    response = await loggedFetch(CHANGE_BOOKING_URL, {
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

  // Normally a bare ChangesState number. The controller currently returns the
  // un-awaited Task, which serializes as an object carrying the value in `result`.
  const body = await response.json().catch(() => null);
  const state =
    body && typeof body === "object" ? (body as { result?: number }).result : body;
  if (state === ChangesState.Failed || state === ChangesState.NotApplied) {
    throw new RoomBookingFailedError(
      `Booking server could not change the booking for room ${roomId}.`,
    );
  }
}

// --- Room info (RoomBookingController.getAllrooms) ---------------------------
// Returns Ok(string) where the string is System.Text.Json output of the
// RoomModel list (PascalCase, enums numeric, Floor is an int, Amenities is a
// single AmenityModel). A null result yields the literal text "no rooms".

const ROOM_INFO_URL = "/api/RoomBooking/getAllrooms";

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
    response = await loggedFetch(ROOM_INFO_URL, { method: "POST" });
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
// 200 OK on success, 401 Unauthorized on a bad username/password.

const ADMIN_LOGIN_URL = "/api/Admin/login";

export async function adminLogin(userName: string, password: string): Promise<boolean> {
  let response: Response;
  try {
    response = await loggedFetch(ADMIN_LOGIN_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ userName, password }),
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

// Fully expands objects so nested fields (e.g. ASP.NET's `errors`) aren't
// collapsed behind "{…}" in the console.
function pretty(value: unknown): string {
  return typeof value === "string" ? value : JSON.stringify(value, null, 2);
}

// --- Admin logout (AdminController.logout) --------------------------------
// 200 OK when the admin was marked as logged out, 400 Bad Request when the
// username is unknown to the server. Resolves to whether the server accepted it.

const ADMIN_LOGOUT_URL = "/api/Admin/logout";

export async function adminLogout(userName: string): Promise<boolean> {
  try {
    const response = await loggedFetch(ADMIN_LOGOUT_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ userName }),
    });
    return response.ok;
  } catch (cause) {
    throw new RoomBookingApiError("Could not reach the booking server.", { cause });
  }
}

// --- Add room (AdminController.addRoom) -----------------------------------
// Takes NewRoomRequest and replies Ok(RoomModel) for the created room, serialized
// with ASP.NET's default (camelCase) JSON - unlike getAllrooms, which is PascalCase.
// Amenities is a single AmenityModel; the backend maps it to a required FK.

const ADD_ROOM_URL = "/api/Admin/addRoom";

export interface NewRoomParams {
  name: string;
  floor: number;
  capacity: number;
  amenity: { id: number; name: string };
  image: string;
}

export async function addRoomOnServer({
  name,
  floor,
  capacity,
  amenity,
  image,
}: NewRoomParams): Promise<Room> {
  let response: Response;
  try {
    response = await loggedFetch(ADD_ROOM_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name, floor, capacity, amenities: amenity, image }),
    });
  } catch (cause) {
    throw new RoomBookingApiError("Could not reach the booking server.", { cause });
  }

  if (!response.ok) {
    const errorText = await response.text().catch(() => "");
    throw new RoomBookingApiError(`Booking server rejected the new room (${response.status}): ${errorText}`);
  }

  const created = (await response.json().catch(() => null)) as {
    id?: number;
    name?: string;
    floor?: number;
    capacity?: number;
    amenities?: { name?: string } | null;
    image?: string | null;
  } | null;

  return {
    id: created?.id ?? 0,
    name: created?.name ?? name,
    floor: `Floor ${created?.floor ?? floor}`,
    capacity: created?.capacity ?? capacity,
    amenities: (created?.amenities?.name ?? amenity.name) ? [created?.amenities?.name ?? amenity.name] : [],
    image: created?.image ?? image,
  };
}

// --- Room configs / amenities (AdminController.getRoomConfigs) ------------
// GET. Returns the existing amenities. Currently a list of names only
// (List<string>); if the backend later returns { id, name } objects those are
// accepted too. Without an id the amenity can only be identified by name.

const ROOM_CONFIGS_URL = "/api/Admin/getRoomConfigs";

export interface AmenityOption {
  id: number | null; // null when the backend only sent names
  name: string;
}

export async function fetchAmenitiesFromServer(): Promise<AmenityOption[]> {
  let response: Response;
  try {
    response = await loggedFetch(ROOM_CONFIGS_URL);
  } catch (cause) {
    throw new RoomBookingApiError("Could not reach the booking server.", { cause });
  }
  if (!response.ok) {
    throw new RoomBookingApiError(`Booking server could not list amenities (${response.status}).`);
  }

  const body: unknown = await response.json().catch(() => []);
  if (!Array.isArray(body)) return [];
  return body.flatMap((item): AmenityOption[] => {
    if (typeof item === "string") return [{ id: null, name: item }];
    const o = item as { id?: number; Id?: number; name?: string; Name?: string };
    const name = o?.name ?? o?.Name;
    return name ? [{ id: o.id ?? o.Id ?? null, name }] : [];
  });
}

// --- Dashboard counters (AdminController) ---------------------------------
// GET endpoints that each reply Ok(int): a bare JSON number.
//   totalBookings    - all reservations in the DB
//   getTodayBookings - reservations dated today (server's local date)
//   getAllRoomsCount - number of rooms

async function fetchCount(url: string, what: string): Promise<number> {
  let response: Response;
  try {
    response = await loggedFetch(url);
  } catch (cause) {
    throw new RoomBookingApiError("Could not reach the booking server.", { cause });
  }
  if (!response.ok) {
    throw new RoomBookingApiError(`Booking server could not count ${what} (${response.status}).`);
  }
  const count: unknown = await response.json().catch(() => null);
  if (typeof count !== "number") {
    throw new RoomBookingApiError(`Booking server returned an unexpected ${what} count.`);
  }
  return count;
}

export const fetchTotalBookings = () => fetchCount("/api/Admin/totalBookings", "bookings");
export const fetchTodayBookings = () => fetchCount("/api/Admin/getTodayBookings", "today's bookings");
export const fetchRoomsCount = () => fetchCount("/api/Admin/getAllRoomsCount", "rooms");

// --- All reservations (AdminController.getAllReservations) ----------------
// GET. Replies Ok(List<ReservationModel>) as camelCase JSON. The query has no
// Include(), so `users` and `room` are null and the user FK isn't serialized:
// only id, date and roomId are usable until the backend loads the relations.

const ALL_RESERVATIONS_URL = "/api/Admin/getAllReservations";

export interface ServerReservation {
  id: number;
  date: string; // ISO 8601 datetime
  roomId: number;
  userName: string | null; // null while the backend doesn't include Users
}

export async function fetchAllReservations(): Promise<ServerReservation[]> {
  let response: Response;
  try {
    response = await loggedFetch(ALL_RESERVATIONS_URL);
  } catch (cause) {
    throw new RoomBookingApiError("Could not reach the booking server.", { cause });
  }
  if (!response.ok) {
    throw new RoomBookingApiError(`Booking server could not list reservations (${response.status}).`);
  }

  const body: unknown = await response.json().catch(() => []);
  if (!Array.isArray(body)) return [];
  return (body as Array<{
    id?: number;
    date?: string;
    roomId?: number;
    users?: { userName?: string } | null;
  }>).map((r) => ({
    id: r.id ?? 0,
    date: r.date ?? "",
    roomId: r.roomId ?? 0,
    userName: r.users?.userName ?? null,
  }));
}
