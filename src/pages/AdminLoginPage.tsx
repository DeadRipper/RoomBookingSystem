import { useState, type FormEvent } from "react";
import { adminLogin } from "../api/roomBookingApi";

export default function AdminLoginPage() {
  const [id, setId] = useState("");
  const [password, setPassword] = useState("");
  const [status, setStatus] = useState<"idle" | "busy" | "ok" | "denied">("idle");
  const [error, setError] = useState<string | null>(null);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setStatus("busy");
    setError(null);
    try {
      setStatus((await adminLogin(Number(id), password)) ? "ok" : "denied");
    } catch (err) {
      setStatus("idle");
      setError(err instanceof Error ? err.message : "Unexpected error.");
    }
  }

  const field =
    "rounded-lg border border-white/10 bg-white/5 px-3 py-2 text-sm text-slate-100 outline-none focus:border-indigo-400/60 focus:ring-1 focus:ring-indigo-400/40";

  return (
    <form onSubmit={onSubmit} className="mx-auto flex max-w-sm flex-col gap-4">
      <h1 className="text-2xl font-semibold tracking-tight text-slate-50">Admin login</h1>
      <input
        value={id}
        onChange={(e) => setId(e.target.value)}
        inputMode="numeric"
        placeholder="Admin ID"
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
        disabled={status === "busy"}
        className="rounded-lg bg-indigo-500 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-400 disabled:opacity-60"
      >
        {status === "busy" ? "Signing in…" : "Sign in"}
      </button>
      {status === "ok" && <p className="text-sm text-teal-300">Signed in.</p>}
      {status === "denied" && <p className="text-sm text-red-400">Invalid admin ID or password.</p>}
      {error && <p className="text-sm text-red-400">{error}</p>}
    </form>
  );
}
