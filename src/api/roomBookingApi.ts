// Thin client for the ASP.NET backend (RoomBookingApp.Controllers.RoomBookingController).
// The backend currently only exposes booking-by-room-id with no date/time/title yet,
// so this is called best-effort alongside the richer local booking model — see
// BookingsContext.addBooking for how the two are combined.

const BOOK_ROOM_URL = "/api/RoomBooking/BookRoom";

export class RoomBookingApiError extends Error {}

export async function bookRoomOnServer(roomId: number): Promise<void> {
  let response: Response;
  try {
    response = await fetch(BOOK_ROOM_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ roomId }),
    });
  } catch (cause) {
    throw new RoomBookingApiError("Could not reach the booking server.", { cause });
  }

  if (!response.ok) {
    const body = await response.text().catch(() => "");
    throw new RoomBookingApiError(
      `Booking server rejected room ${roomId} (${response.status}): ${body}`,
    );
  }
}
