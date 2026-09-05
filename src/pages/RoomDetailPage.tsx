import { useMemo, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { rooms } from "../data/rooms";
import { useBookings } from "../store/BookingsContext";
import { buildDaySlots, formatDateLabel, formatMinutes, isoDate } from "../lib/time";

export default function RoomDetailPage() {
  const { roomId } = useParams();
  const navigate = useNavigate();
  const { bookings, addBooking, isSlotFree, currentUser } = useBookings();

  const room = rooms.find((r) => r.id === Number(roomId));

  const [dayOffset, setDayOffset] = useState(0);
  const [selectedStart, setSelectedStart] = useState<number | null>(null);
  const [selectedEnd, setSelectedEnd] = useState<number | null>(null);
  const [title, setTitle] = useState("");
  const [error, setError] = useState<string | null>(null);

  const date = isoDate(dayOffset);
  const slots = useMemo(() => buildDaySlots(), []);

  if (!room) {
    return (
      <div className="text-center text-slate-400">
        <p>Room not found.</p>
        <Link to="/" className="mt-2 inline-block text-indigo-300 hover:underline">
          Back to rooms
        </Link>
      </div>
    );
  }

  const dayBookings = bookings
    .filter((b) => b.roomId === room.id && b.date === date)
    .sort((a, b) => a.startMinutes - b.startMinutes);

  const toggleSlot = (startMinutes: number, endMinutes: number) => {
    setError(null);
    if (!isSlotFree(room.id, date, startMinutes, endMinutes)) return;

    if (selectedStart === null || (selectedStart !== null && selectedEnd !== null)) {
      setSelectedStart(startMinutes);
      setSelectedEnd(endMinutes);
      return;
    }

    if (startMinutes === selectedStart) {
      setSelectedStart(null);
      setSelectedEnd(null);
      return;
    }

    const newStart = Math.min(selectedStart, startMinutes);
    const newEnd = Math.max(selectedStart, endMinutes);
    for (let m = newStart; m < newEnd; m += 30) {
      if (!isSlotFree(room.id, date, m, m + 30)) {
        setError("That range overlaps an existing booking.");
        return;
      }
    }
    setSelectedStart(newStart);
    setSelectedEnd(newEnd);
  };

  const handleBook = () => {
    if (selectedStart === null || selectedEnd === null) return;
    if (!title.trim()) {
      setError("Give the meeting a title.");
      return;
    }
    addBooking({
      roomId: room.id,
      date,
      startMinutes: selectedStart,
      endMinutes: selectedEnd,
      title: title.trim(),
      bookedBy: currentUser,
    });
    navigate("/bookings");
  };

  return (
    <div className="flex flex-col gap-6">
      <Link to="/" className="w-fit text-sm text-slate-400 hover:text-slate-200">
        ← Back to rooms
      </Link>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-[2fr_1fr]">
        <div className="overflow-hidden rounded-2xl border border-white/10 bg-white/[0.03]">
          <img src={room.image} alt={room.name} className="h-56 w-full object-cover" />
          <div className="flex flex-col gap-4 p-6">
            <div>
              <h1 className="text-2xl font-semibold text-slate-50">{room.name}</h1>
              <p className="text-sm text-slate-400">
                {room.floor} · Seats {room.capacity}
              </p>
            </div>
            <div className="flex flex-wrap gap-2">
              {room.amenities.map((a) => (
                <span
                  key={a}
                  className="rounded-md bg-white/5 px-2.5 py-1 text-xs text-slate-300 ring-1 ring-white/10"
                >
                  {a}
                </span>
              ))}
            </div>

            <div className="mt-2 flex items-center gap-2">
              {[0, 1, 2].map((offset) => (
                <button
                  key={offset}
                  onClick={() => {
                    setDayOffset(offset);
                    setSelectedStart(null);
                    setSelectedEnd(null);
                    setError(null);
                  }}
                  className={`rounded-lg px-3 py-1.5 text-sm transition-colors ${
                    dayOffset === offset
                      ? "bg-indigo-500 text-white"
                      : "bg-white/5 text-slate-400 hover:bg-white/10"
                  }`}
                >
                  {offset === 0 ? "Today" : formatDateLabel(isoDate(offset))}
                </button>
              ))}
            </div>

            <div>
              <p className="mb-2 text-xs uppercase tracking-wide text-slate-500">
                Select a time range
              </p>
              <div className="grid grid-cols-4 gap-1.5 sm:grid-cols-6">
                {slots.map((slot) => {
                  const free = isSlotFree(room.id, date, slot.startMinutes, slot.endMinutes);
                  const inSelection =
                    selectedStart !== null &&
                    selectedEnd !== null &&
                    slot.startMinutes >= selectedStart &&
                    slot.startMinutes < selectedEnd;
                  return (
                    <button
                      key={slot.startMinutes}
                      disabled={!free}
                      onClick={() => toggleSlot(slot.startMinutes, slot.endMinutes)}
                      className={`rounded-md px-1.5 py-1.5 text-[11px] font-medium transition-colors ${
                        !free
                          ? "cursor-not-allowed bg-white/[0.02] text-slate-700 line-through"
                          : inSelection
                            ? "bg-teal-400 text-slate-900"
                            : "bg-white/5 text-slate-300 hover:bg-indigo-500/30"
                      }`}
                    >
                      {formatMinutes(slot.startMinutes)}
                    </button>
                  );
                })}
              </div>
            </div>

            {selectedStart !== null && selectedEnd !== null && (
              <div className="flex flex-col gap-3 rounded-xl border border-indigo-400/20 bg-indigo-500/5 p-4">
                <p className="text-sm text-slate-300">
                  Booking{" "}
                  <span className="font-medium text-slate-100">
                    {formatMinutes(selectedStart)} – {formatMinutes(selectedEnd)}
                  </span>{" "}
                  on {formatDateLabel(date)}
                </p>
                <input
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                  placeholder="Meeting title"
                  className="rounded-lg border border-white/10 bg-white/5 px-3 py-2 text-sm text-slate-100 outline-none placeholder:text-slate-500 focus:border-indigo-400/60"
                />
                {error && <p className="text-xs text-rose-400">{error}</p>}
                <div className="flex gap-2">
                  <button
                    onClick={handleBook}
                    className="rounded-lg bg-indigo-500 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-400"
                  >
                    Confirm booking
                  </button>
                  <button
                    onClick={() => {
                      setSelectedStart(null);
                      setSelectedEnd(null);
                      setError(null);
                    }}
                    className="rounded-lg bg-white/5 px-4 py-2 text-sm text-slate-300 hover:bg-white/10"
                  >
                    Cancel
                  </button>
                </div>
              </div>
            )}
            {error && selectedStart === null && (
              <p className="text-xs text-rose-400">{error}</p>
            )}
          </div>
        </div>

        <div className="flex flex-col gap-3">
          <h2 className="text-sm font-medium uppercase tracking-wide text-slate-500">
            Schedule — {formatDateLabel(date)}
          </h2>
          {dayBookings.length === 0 && (
            <p className="rounded-xl border border-dashed border-white/10 p-4 text-sm text-slate-500">
              No bookings yet for this day.
            </p>
          )}
          {dayBookings.map((b) => (
            <div
              key={b.id}
              className="rounded-xl border border-white/10 bg-white/[0.03] p-3 text-sm"
            >
              <p className="font-medium text-slate-100">{b.title}</p>
              <p className="text-xs text-slate-400">
                {formatMinutes(b.startMinutes)} – {formatMinutes(b.endMinutes)} · {b.bookedBy}
              </p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
