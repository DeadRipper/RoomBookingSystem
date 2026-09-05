import { useMemo, useState } from "react";
import RoomCard from "../components/RoomCard";
import { rooms } from "../data/rooms";
import { useBookings } from "../store/BookingsContext";
import { isoDate } from "../lib/time";

export default function RoomsPage() {
  const [query, setQuery] = useState("");
  const [minCapacity, setMinCapacity] = useState(0);
  const { bookings } = useBookings();

  const nowMinutes = new Date().getHours() * 60 + new Date().getMinutes();
  const today = isoDate();

  const filteredRooms = useMemo(
    () =>
      rooms.filter(
        (r) =>
          r.name.toLowerCase().includes(query.toLowerCase()) && r.capacity >= minCapacity,
      ),
    [query, minCapacity],
  );

  const isFreeNow = (roomId: number) =>
    !bookings.some(
      (b) =>
        b.roomId === roomId &&
        b.date === today &&
        nowMinutes >= b.startMinutes &&
        nowMinutes < b.endMinutes,
    );

  return (
    <div className="flex flex-col gap-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight text-slate-50">Conference Rooms</h1>
        <p className="mt-1 text-sm text-slate-400">
          Find a space, check availability, and book in a couple of clicks.
        </p>
      </div>

      <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
        <input
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search rooms…"
          className="w-full max-w-xs rounded-lg border border-white/10 bg-white/5 px-3 py-2 text-sm text-slate-100 outline-none placeholder:text-slate-500 focus:border-indigo-400/60 focus:ring-1 focus:ring-indigo-400/40"
        />
        <div className="flex items-center gap-2 text-sm text-slate-400">
          <span>Min capacity</span>
          <select
            value={minCapacity}
            onChange={(e) => setMinCapacity(Number(e.target.value))}
            className="rounded-lg border border-white/10 bg-white/5 px-2 py-2 text-slate-100 outline-none focus:border-indigo-400/60"
          >
            {[0, 2, 4, 6, 8, 12, 20].map((n) => (
              <option key={n} value={n} className="bg-[#0b0e14]">
                {n === 0 ? "Any" : `${n}+`}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {filteredRooms.map((room) => (
          <RoomCard key={room.id} room={room} freeNow={isFreeNow(room.id)} />
        ))}
        {filteredRooms.length === 0 && (
          <p className="col-span-full text-sm text-slate-500">No rooms match your filters.</p>
        )}
      </div>
    </div>
  );
}
