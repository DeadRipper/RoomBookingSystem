import { Link, useLocation } from "react-router-dom";
import { rooms } from "../data/rooms";
import { useBookings } from "../store/BookingsContext";
import { formatDateLabel, formatMinutes } from "../lib/time";

interface NavState {
  notice?: string;
}

export default function MyBookingsPage() {
  const { bookings, cancelBooking, currentUser } = useBookings();
  const location = useLocation();
  const notice = (location.state as NavState | null)?.notice;

  const mine = bookings
    .filter((b) => b.bookedBy === currentUser)
    .sort((a, b) => (a.date + a.startMinutes).localeCompare(b.date + b.startMinutes));

  return (
    <div className="flex flex-col gap-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight text-slate-50">My Bookings</h1>
        <p className="mt-1 text-sm text-slate-400">Reservations made as {currentUser}.</p>
      </div>

      {notice && (
        <div className="rounded-xl border border-amber-400/20 bg-amber-500/10 px-4 py-3 text-sm text-amber-200">
          {notice}
        </div>
      )}

      {mine.length === 0 ? (
        <div className="rounded-2xl border border-dashed border-white/10 p-10 text-center">
          <p className="text-slate-400">You have no upcoming bookings.</p>
          <Link
            to="/"
            className="mt-3 inline-block rounded-lg bg-indigo-500 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-400"
          >
            Browse rooms
          </Link>
        </div>
      ) : (
        <div className="flex flex-col gap-3">
          {mine.map((b) => {
            const room = rooms.find((r) => r.id === b.roomId);
            return (
              <div
                key={b.id}
                className="flex items-center justify-between gap-4 rounded-2xl border border-white/10 bg-white/[0.03] p-4"
              >
                <div>
                  <p className="font-medium text-slate-100">{b.title}</p>
                  <p className="text-sm text-slate-400">
                    {room?.name ?? "Unknown room"} · {formatDateLabel(b.date)} ·{" "}
                    {formatMinutes(b.startMinutes)} – {formatMinutes(b.endMinutes)}
                  </p>
                </div>
                <button
                  onClick={() => cancelBooking(b.id)}
                  className="shrink-0 rounded-lg bg-rose-500/10 px-3 py-1.5 text-sm font-medium text-rose-300 ring-1 ring-rose-400/20 hover:bg-rose-500/20"
                >
                  Cancel
                </button>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
