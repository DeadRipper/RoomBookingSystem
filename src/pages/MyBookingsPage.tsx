import { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { useRooms } from "../store/RoomsContext";
import { useBookings } from "../store/BookingsContext";
import { buildDaySlots, formatDateLabel, formatMinutes, isoDate } from "../lib/time";
import type { Booking } from "../types";

interface NavState {
  notice?: string;
}

export default function MyBookingsPage() {
  const { bookings, cancelBooking, updateBooking, isSlotFree, currentUser } = useBookings();
  const { rooms } = useRooms();
  const location = useLocation();
  const notice = (location.state as NavState | null)?.notice;
  const [error, setError] = useState<string | null>(null);
  const [cancellingId, setCancellingId] = useState<string | null>(null);
  const [editing, setEditing] = useState<Booking | null>(null);
  const [editTitle, setEditTitle] = useState("");
  const [editDayOffset, setEditDayOffset] = useState(0);
  const [editStart, setEditStart] = useState(0);
  const [saving, setSaving] = useState(false);
  const [editError, setEditError] = useState<string | null>(null);
  const slots = buildDaySlots();

  const startEdit = (b: Booking) => {
    const offset = Math.round(
      (new Date(`${b.date}T00:00:00`).getTime() - new Date(`${isoDate()}T00:00:00`).getTime()) /
        86_400_000,
    );
    setEditing(b);
    setEditTitle(b.title);
    setEditDayOffset(Math.max(0, Math.min(2, offset)));
    setEditStart(b.startMinutes);
    setEditError(null);
  };

  const editDate = isoDate(editDayOffset);
  const editDuration = editing ? editing.endMinutes - editing.startMinutes : 0;

  const handleSave = async () => {
    if (!editing) return;
    if (!editTitle.trim()) {
      setEditError("Give the meeting a title.");
      return;
    }
    const editEnd = editStart + editDuration;
    for (let m = editStart; m < editEnd; m += 30) {
      if (!isSlotFree(editing.roomId, editDate, m, Math.min(m + 30, editEnd), editing.id)) {
        setEditError("That time overlaps an existing booking.");
        return;
      }
    }
    setSaving(true);
    const { serverError } = await updateBooking(editing.id, {
      title: editTitle.trim(),
      date: editDate,
      startMinutes: editStart,
      endMinutes: editEnd,
    });
    setSaving(false);
    if (serverError) {
      setEditError(serverError);
      return;
    }
    setEditing(null);
  };

  const handleCancel = async (id: string) => {
    setError(null);
    setCancellingId(id);
    const { serverError } = await cancelBooking(id);
    setCancellingId(null);
    if (serverError) setError(serverError);
  };

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

      {error && (
        <div className="rounded-xl border border-rose-400/20 bg-rose-500/10 px-4 py-3 text-sm text-rose-200">
          {error}
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
                <div className="flex shrink-0 gap-2">
                  <button
                    onClick={() => startEdit(b)}
                    className="rounded-lg bg-white/5 px-3 py-1.5 text-sm font-medium text-slate-200 ring-1 ring-white/10 hover:bg-white/10"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleCancel(b.id)}
                    disabled={cancellingId === b.id}
                    className="rounded-lg bg-rose-500/10 px-3 py-1.5 text-sm font-medium text-rose-300 ring-1 ring-rose-400/20 hover:bg-rose-500/20 disabled:cursor-not-allowed disabled:opacity-50"
                  >
                    {cancellingId === b.id ? "Cancelling…" : "Cancel"}
                  </button>
                </div>
              </div>
            );
          })}
        </div>
      )}

      {editing && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4">
          <div className="flex w-full max-w-md flex-col gap-4 rounded-2xl border border-white/10 bg-slate-900 p-6 shadow-xl">
            <h2 className="text-lg font-semibold text-slate-50">Edit booking</h2>
            <input
              value={editTitle}
              onChange={(e) => setEditTitle(e.target.value)}
              placeholder="Meeting title"
              className="rounded-lg border border-white/10 bg-white/5 px-3 py-2 text-sm text-slate-100 outline-none placeholder:text-slate-500 focus:border-indigo-400/60"
            />
            <div className="flex gap-2">
              {[0, 1, 2].map((offset) => (
                <button
                  key={offset}
                  onClick={() => setEditDayOffset(offset)}
                  className={`rounded-lg px-3 py-1.5 text-sm ${
                    editDayOffset === offset
                      ? "bg-indigo-500 text-white"
                      : "bg-white/5 text-slate-400 hover:bg-white/10"
                  }`}
                >
                  {offset === 0 ? "Today" : formatDateLabel(isoDate(offset))}
                </button>
              ))}
            </div>
            <div className="grid grid-cols-4 gap-1.5">
              {slots
                .filter((sl) => sl.startMinutes + editDuration <= 19 * 60)
                .map((sl) => (
                  <button
                    key={sl.startMinutes}
                    onClick={() => setEditStart(sl.startMinutes)}
                    className={`rounded-md px-1.5 py-1.5 text-[11px] font-medium ${
                      editStart === sl.startMinutes
                        ? "bg-teal-400 text-slate-900"
                        : "bg-white/5 text-slate-300 hover:bg-indigo-500/30"
                    }`}
                  >
                    {formatMinutes(sl.startMinutes)}
                  </button>
                ))}
            </div>
            <p className="text-xs text-slate-400">
              {formatMinutes(editStart)} – {formatMinutes(editStart + editDuration)} on{" "}
              {formatDateLabel(editDate)}
            </p>
            {editError && <p className="text-xs text-rose-400">{editError}</p>}
            <div className="flex gap-2">
              <button
                onClick={handleSave}
                disabled={saving}
                className="rounded-lg bg-indigo-500 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-400 disabled:opacity-60"
              >
                {saving ? "Saving…" : "Save changes"}
              </button>
              <button
                onClick={() => setEditing(null)}
                disabled={saving}
                className="rounded-lg bg-white/5 px-4 py-2 text-sm text-slate-300 hover:bg-white/10 disabled:opacity-60"
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
