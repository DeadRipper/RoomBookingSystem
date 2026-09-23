// Thin client for the ASP.NET backend (RoomBookingApp.Controllers.RoomBookingController).
// BookRoomRequest now carries BookingDate and MeetingTitle alongside RoomId — see
// BookingsContext.addBooking for how the local booking model maps onto this request.

const BOOK_ROOM_URL = "/api/RoomBooking/bookRoom";

export class RoomBookingApiError extends Error {}

export interface BookRoomParams {
  roomId: number;
  bookingDate: string; // ISO 8601 datetime
  meetingTitle: string;
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
    const body = await response.text().catch(() => "");
    throw new RoomBookingApiError(
      `Booking server rejected room ${roomId} (${response.status}): ${body}`,
    );
  }
}
