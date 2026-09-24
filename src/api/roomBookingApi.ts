// Thin client for the ASP.NET backend (RoomBookingApp.Controllers.RoomBookingController).
// BookRoomRequest now carries BookingDate and MeetingTitle alongside RoomId — see
// BookingsContext.addBooking for how the local booking model maps onto this request.

const BOOK_ROOM_URL = "/api/RoomBooking/bookRoom";
const UNBOOK_ROOM_URL = "/api/RoomBooking/unbookRoom";

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
}

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
}: BookRoomParams): Promise<void> {
  let response: Response;
  try {
    response = await fetch(BOOK_ROOM_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ roomId, bookingDate, meetingTitle }),
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
