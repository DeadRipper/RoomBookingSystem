import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import type { Room } from "../types";
import { rooms as fallbackRooms } from "../data/rooms";
import { fetchRoomsFromServer } from "../api/roomBookingApi";

interface RoomsContextValue {
  rooms: Room[];
  loading: boolean;
  addRoomLocal: (room: Room) => void;
}

// StrictMode runs effects twice in development; share one in-flight request so
// the backend sees a single getAllrooms call.
let inflightRooms: Promise<Room[]> | null = null;
function loadRooms(): Promise<Room[]> {
  inflightRooms ??= fetchRoomsFromServer().finally(() => {
    inflightRooms = null;
  });
  return inflightRooms;
}

const RoomsContext = createContext<RoomsContextValue | null>(null);

// Loads rooms from the backend; falls back to the bundled list when the
// server is unreachable or has no rooms yet, so the UI stays usable offline.
export function RoomsProvider({ children }: { children: ReactNode }) {
  const [rooms, setRooms] = useState<Room[]>(fallbackRooms);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    loadRooms()
      .then((r) => {
        if (!cancelled && r.length > 0) setRooms(r);
      })
      .catch(() => {})
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  // Appends a room the server just created, so the lists update without a reload.
  const addRoomLocal = (room: Room) => setRooms((prev) => [...prev, room]);

  return (
    <RoomsContext.Provider value={{ rooms, loading, addRoomLocal }}>{children}</RoomsContext.Provider>
  );
}

export function useRooms(): RoomsContextValue {
  const ctx = useContext(RoomsContext);
  if (!ctx) throw new Error("useRooms must be used within RoomsProvider");
  return ctx;
}
