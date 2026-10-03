import { useEffect, useState } from "react";
import { fetchAllReservations, type ServerReservation } from "../api/roomBookingApi";

// Admin overview of every reservation on the server, newest first.
export default function AdminReservations() {
  const [reservations, setReservations] = useState<ServerReservation[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    fetchAllReservations()
      .then((list) => {
        if (!cancelled) setReservations([...list].sort((a, b) => b.date.localeCompare(a.date)));
      })
      .catch((err) => {
        if (!cancelled) setError(err instanceof Error ? err.message : "Unexpected error.");
      });
    return () => {
      cancelled = true;
    };
  }, []);

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
                <th className="py-2 pr-4 font-medium">Room</th>
                <th className="py-2 pr-4 font-medium">When</th>
                <th className="py-2 pr-4 font-medium">Meeting</th>
                <th className="py-2 font-medium">Booked by</th>
              </tr>
            </thead>
            <tbody className="text-slate-200">
              {reservations.map((r, i) => (
                <tr key={`${r.date}-${r.roomName}-${r.userName}-${i}`} className="border-t border-white/5">
                  <td className="py-2 pr-4">{r.roomName}</td>
                  <td className="py-2 pr-4">{new Date(r.date).toLocaleString()}</td>
                  <td className="py-2 pr-4">{r.meetingTitle || "—"}</td>
                  <td className="py-2 text-slate-400">{r.userName || "—"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
