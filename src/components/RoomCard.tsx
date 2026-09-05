import { Link } from "react-router-dom";
import type { Room } from "../types";

export default function RoomCard({ room, freeNow }: { room: Room; freeNow: boolean }) {
  return (
    <Link
      to={`/rooms/${room.id}`}
      className="group flex flex-col overflow-hidden rounded-2xl border border-white/10 bg-white/[0.03] transition-all hover:-translate-y-0.5 hover:border-indigo-400/40 hover:bg-white/[0.06]"
    >
      <div className="relative h-36 w-full overflow-hidden">
        <img
          src={room.image}
          alt={room.name}
          className="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
          loading="lazy"
        />
        <span
          className={`absolute right-3 top-3 rounded-full px-2.5 py-1 text-xs font-medium backdrop-blur ${
            freeNow
              ? "bg-emerald-500/20 text-emerald-300 ring-1 ring-emerald-400/30"
              : "bg-amber-500/20 text-amber-300 ring-1 ring-amber-400/30"
          }`}
        >
          {freeNow ? "Free now" : "In use"}
        </span>
      </div>

      <div className="flex flex-1 flex-col gap-3 p-4">
        <div className="flex items-start justify-between">
          <div>
            <h3 className="text-base font-semibold text-slate-100">{room.name}</h3>
            <p className="text-xs text-slate-500">{room.floor}</p>
          </div>
          <div className="flex items-center gap-1 text-xs text-slate-400">
            <UsersIcon />
            {room.capacity}
          </div>
        </div>

        <div className="flex flex-wrap gap-1.5">
          {room.amenities.map((a) => (
            <span
              key={a}
              className="rounded-md bg-white/5 px-2 py-1 text-[11px] text-slate-400 ring-1 ring-white/5"
            >
              {a}
            </span>
          ))}
        </div>
      </div>
    </Link>
  );
}

function UsersIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" className="h-3.5 w-3.5" stroke="currentColor" strokeWidth="2">
      <path
        d="M17 20v-1a4 4 0 0 0-4-4H7a4 4 0 0 0-4 4v1M13 7a4 4 0 1 1-8 0 4 4 0 0 1 8 0Zm7 13v-1a4 4 0 0 0-3-3.87M15 3.13a4 4 0 0 1 0 7.75"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
}
