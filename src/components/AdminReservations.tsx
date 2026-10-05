import { useEffect, useState } from "react";
import { cancelBookingOnServer, fetchAllReservations, fetchUserNameById, type ServerReservation } from "../api/roomBookingApi";
import { useRooms } from "../store/RoomsContext";

// Admin overview of every reservation on the server, newest first.
export default function AdminReservations() {
  const { rooms } = useRooms();
  const [reservations, setReservations] = useState<ServerReservation[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  // userId -> username, filled in after the reservations arrive.
  const [userNames, setUserNames] = useState<Record<number, string>>({});

  useEffect(() => {
    let cancelled = false;
    fetchAllReservations()
      .then((list) => {
        if (!cancelled) setReservations([...list].sort((a, b) => b.date.localeCompare(a.date)));
        // One lookup per distinct user; a failed lookup just leaves "User #id".
        for (const id of new Set(list.map((r) => r.userId))) {
          fetchUserNameById(id)
            .then((name) => {
              if (!cancelled && name) setUserNames((prev) => ({ ...prev, [id]: name }));
            })
            .catch(() => {});
        }
      })
      .catch((err) => {
        if (!cancelled) setError(err instanceof Error ? err.message : "Unexpected error.");
      });
    return () => {
      cancelled = true;
    };
  }, []);

  const cancel = async (id: number) => {
    if (!window.confirm("Cancel this reservation?")) return;
    try {
      await cancelBookingOnServer(id);
      setReservations((prev) => prev && prev.filter((r) => r.id !== id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unexpected error.");
    }
  };

  const roomName = (id: number) => rooms.find((r) => r.id === id)?.name ?? `Room #${id}`;

  return (
    <div className="rounded-2xl border border-white/10 bg-white/[0.03] p-5">
      <h2 className="text-lg font-semibold text-slate-50">All reservations</h2>

      {error && <p className="mt-3 text-sm text-red-400">{error}</p>}
      {!error && reservations === null && <p className="mt-3 text-sm text-slate-500">Loading…</p>}
      {reservations?.length === 0 && <p className="mt-3 text-sm text-slate-500">No reservations yet.</p>}

      {reservations && reservations.length > 0 && (
        <div className="mt-3 max-h-96 overflow-auto">
          <table className="w-full text-left text-sm">
            <thead className="sticky top-0 bg-[#0b0e14] text-slate-400">
              <tr>
                <th className="py-2 pr-4 font-medium">#</th>
                <th className="py-2 pr-4 font-medium">Room</th>
                <th className="py-2 pr-4 font-medium">When</th>
                <th className="py-2 pr-4 font-medium">Meeting</th>
                <th className="py-2 pr-4 font-medium">Booked by</th>
                <th className="py-2 font-medium"></th>
              </tr>
            </thead>
            <tbody className="text-slate-200">
              {reservations.map((r) => (
                <tr key={r.id} className="border-t border-white/5">
                  <td className="py-2 pr-4 text-slate-500">{r.id}</td>
                  <td className="py-2 pr-4">{roomName(r.roomId)}</td>
                  <td className="py-2 pr-4">{new Date(r.date).toLocaleString()}</td>
                  <td className="py-2 pr-4">{r.meetingTitle || "—"}</td>
                  <td className="py-2 pr-4 text-slate-400">{userNames[r.userId] ?? `User #${r.userId}`}</td>
                  <td className="py-2 text-right">
                    <button type="button" onClick={() => cancel(r.id)} className="text-red-400 hover:text-red-300">
                      Cancel
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
