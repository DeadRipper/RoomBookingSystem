import { createContext, useContext, useState, type ReactNode } from "react";
import { adminLogout } from "../api/roomBookingApi";

const ADMIN_KEY = "rba_admin_v1";

interface AdminContextValue {
  admin: string | null; // username of the logged-in admin
  signIn: (userName: string) => void;
  signOut: () => void;
}

const AdminContext = createContext<AdminContextValue | null>(null);

function loadAdmin(): string | null {
  try {
    return localStorage.getItem(ADMIN_KEY);
  } catch {
    return null;
  }
}

// The backend login endpoint returns no token, so the session is just the
// remembered username. This gates the UI only - it is not real authorization.
export function AdminProvider({ children }: { children: ReactNode }) {
  const [admin, setAdmin] = useState<string | null>(loadAdmin);

  const signIn = (userName: string) => {
    setAdmin(userName);
    try {
      localStorage.setItem(ADMIN_KEY, userName);
    } catch {
      /* storage unavailable - session lasts until reload */
    }
  };

  const signOut = () => {
    // Best-effort: tell the server, but always end the local session.
    if (admin) {
      adminLogout(admin)
        .then((ok) => {
          if (!ok) console.warn(`[RBA] Server did not recognise admin "${admin}" on logout; session cleared locally.`);
        })
        .catch(() => {});
    }
    setAdmin(null);
    try {
      localStorage.removeItem(ADMIN_KEY);
    } catch {
      /* ignore */
    }
  };

  return <AdminContext.Provider value={{ admin, signIn, signOut }}>{children}</AdminContext.Provider>;
}

export function useAdmin(): AdminContextValue {
  const ctx = useContext(AdminContext);
  if (!ctx) throw new Error("useAdmin must be used within AdminProvider");
  return ctx;
}
