import { NavLink, Outlet } from "react-router-dom";
import { useBookings } from "../store/BookingsContext";

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  `rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
    isActive
      ? "bg-indigo-500/15 text-indigo-300"
      : "text-slate-400 hover:bg-white/5 hover:text-slate-200"
  }`;

export default function Layout() {
  const { currentUser, setCurrentUser } = useBookings();

  return (
    <div className="min-h-screen bg-[#0b0e14] text-slate-100">
      <div className="pointer-events-none fixed inset-0 -z-10 bg-[radial-gradient(circle_at_20%_-10%,rgba(99,102,241,0.18),transparent_45%),radial-gradient(circle_at_85%_10%,rgba(45,212,191,0.12),transparent_40%)]" />

      <header className="sticky top-0 z-20 border-b border-white/5 bg-[#0b0e14]/80 backdrop-blur">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4">
          <div className="flex items-center gap-2">
            <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-gradient-to-br from-indigo-500 to-teal-400 text-sm font-bold text-white">
              M
            </div>
            <span className="text-lg font-semibold tracking-tight">MeetSpace</span>
          </div>

          <nav className="flex items-center gap-1">
            <NavLink to="/" end className={navLinkClass}>
              Rooms
            </NavLink>
            <NavLink to="/bookings" className={navLinkClass}>
              My Bookings
            </NavLink>
          </nav>

          <div className="flex items-center gap-2">
            <span className="hidden text-sm text-slate-500 sm:inline">Booking as</span>
            <input
              value={currentUser}
              onChange={(e) => setCurrentUser(e.target.value || "Guest")}
              className="w-28 rounded-lg border border-white/10 bg-white/5 px-2.5 py-1.5 text-sm text-slate-100 outline-none focus:border-indigo-400/60 focus:ring-1 focus:ring-indigo-400/40"
            />
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-6 py-8">
        <Outlet />
      </main>
    </div>
  );
}
