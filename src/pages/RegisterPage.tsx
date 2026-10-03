import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { registerUser } from "../api/roomBookingApi";
import { useBookings } from "../store/BookingsContext";

export default function RegisterPage() {
  const navigate = useNavigate();
  const { setCurrentUser } = useBookings();
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setBusy(true);
    setError(null);
    try {
      const name = userName.trim();
      await registerUser({ userName: name, email: email.trim(), password });
      // The server returns no user id, so the username is all we can remember;
      // it becomes the "Booking as" name.
      setCurrentUser(name);
      navigate("/", { replace: true });
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unexpected error.");
    } finally {
      setBusy(false);
    }
  }

  const field =
    "rounded-lg border border-white/10 bg-white/5 px-3 py-2 text-sm text-slate-100 outline-none placeholder:text-slate-500 focus:border-indigo-400/60 focus:ring-1 focus:ring-indigo-400/40";

  return (
    <form onSubmit={onSubmit} className="mx-auto flex max-w-sm flex-col gap-4">
      <h1 className="text-2xl font-semibold tracking-tight text-slate-50">Create account</h1>
      <input
        value={userName}
        onChange={(e) => setUserName(e.target.value)}
        placeholder="Username"
        required
        className={field}
      />
      <input
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        type="email"
        placeholder="Email"
        required
        className={field}
      />
      <input
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        type="password"
        placeholder="Password"
        required
        className={field}
      />
      <button
        disabled={busy}
        className="rounded-lg bg-indigo-500 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-400 disabled:opacity-60"
      >
        {busy ? "Creating…" : "Register"}
      </button>
      {error && <p className="text-sm text-red-400">{error}</p>}
    </form>
  );
}
