import { useEffect, useState } from "react";
import { useBookings } from "../store/BookingsContext";
import { fetchRoomsCount, fetchTodayBookings, fetchTotalBookings } from "../api/roomBookingApi";
import { useRooms } from "../store/RoomsContext";
import { useAdmin } from "../store/AdminContext";
import { isoDate } from "../lib/time";
import AddRoomForm from "../components/AddRoomForm";
import AdminReservations from "../components/AdminReservations";

const upcoming = [
  "Cancel any booking",
  "Edit / delete rooms & maintenance",
];

// Loads a counter from the server. `value` is null while loading and when the
// request failed (callers then fall back to a local figure); `loading` tells
// the two apart. Re-runs when `refreshKey` changes.
function useServerCount(fetchCount: () => Promise<number>, refreshKey?: unknown) {
  const [state, setState] = useState<{ value: number | null; loading: boolean }>({
    value: null,
    loading: true,
  });
  useEffect(() => {
    let cancelled = false;
    fetchCount()
      .then((value) => !cancelled && setState({ value, loading: false }))
      .catch(() => !cancelled && setState({ value: null, loading: false }));
    return () => {
      cancelled = true;
    };
  }, [fetchCount, refreshKey]);
  return state;
}

export default function AdminDashboardPage() {
  const { admin } = useAdmin();
  const { rooms } = useRooms();
  const { bookings } = useBookings();
  const today = isoDate();

  // Rooms refetch when the list changes, so adding a room updates the tile.
  const roomsCount = useServerCount(fetchRoomsCount, rooms.length);
  const todayCount = useServerCount(fetchTodayBookings);
  const totalCount = useServerCount(fetchTotalBookings);

  // "…" while loading; the local figure only if the server request failed.
  const show = (c: { value: number | null; loading: boolean }, local: number) =>
    c.loading ? "…" : (c.value ?? local);

  const stats = [
    { label: "Rooms", value: show(roomsCount, rooms.length) },
    { label: "Bookings today", value: show(todayCount, bookings.filter((b) => b.date === today).length) },
    { label: "Total bookings", value: show(totalCount, bookings.length) },
  ];

  return (
    <div className="flex flex-col gap-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight text-slate-50">Admin dashboard</h1>
        <p className="mt-1 text-sm text-slate-400">Signed in as {admin}.</p>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        {stats.map((s) => (
          <div key={s.label} className="rounded-2xl border border-white/10 bg-white/[0.03] p-5">
            <p className="text-sm text-slate-400">{s.label}</p>
            <p className="mt-1 text-3xl font-semibold text-slate-50">{s.value}</p>
          </div>
        ))}
      </div>

      <AdminReservations />

      <AddRoomForm />

      <div className="rounded-2xl border border-dashed border-white/10 p-5">
        <p className="text-sm font-medium text-slate-300">Coming next</p>
        <ul className="mt-2 list-disc pl-5 text-sm text-slate-500">
          {upcoming.map((u) => (
            <li key={u}>{u}</li>
          ))}
        </ul>
      </div>
    </div>
  );
}
