// Thin client for the ASP.NET backend (RoomBookingApp.Controllers.RoomBookingController).
// BookRoomRequest now carries BookingDate and MeetingTitle alongside RoomId — see
// BookingsContext.addBooking for how the local booking model maps onto this request.

const BOOK_ROOM_URL = "/api/RoomBooking/bookRoom";

export class RoomBookingApiError extends Error {}

// Thrown specifically when the server explicitly rejected the booking
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
  roomId?: number | null;
  bookState?: BookState | null;
  roomState?: number | null;
  error_msg?: string | null;
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
  const result: BookRoomResponse = await response.json().catch(() => ({}));
  if (result.bookState === BookState.Failed) {
    throw new RoomBookingFailedError(
      result.error_msg || `Booking server could not book room ${roomId}.`,
    );
  }
}
