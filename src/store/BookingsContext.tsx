import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import type { Booking } from "../types";
import { bookRoomOnServer, RoomBookingApiError } from "../api/roomBookingApi";

const STORAGE_KEY = "rba_bookings_v1";
const USER_KEY = "rba_user_v1";

function todayISO(offsetDays = 0): string {
  const d = new Date();
  d.setDate(d.getDate() + offsetDays);
  return d.toISOString().slice(0, 10);
}

function seedBookings(): Booking[] {
  return [
    {
      id: "seed-1",
      roomId: 2,
      title: "Sprint Planning",
      bookedBy: "Dmytro",
      date: todayISO(),
      startMinutes: 10 * 60,
      endMinutes: 11 * 60,
    },
    {
      id: "seed-2",
      roomId: 5,
      title: "Quarterly Review",
      bookedBy: "Olena",
      date: todayISO(),
      startMinutes: 13 * 60,
      endMinutes: 14 * 60 + 30,
    },
    {
      id: "seed-3",
      roomId: 3,
      title: "Client Demo",
      bookedBy: "Dmytro",
      date: todayISO(1),
      startMinutes: 9 * 60,
      endMinutes: 10 * 60,
    },
  ];
}

function loadBookings(): Booking[] {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return seedBookings();
    const parsed = JSON.parse(raw) as Booking[];
    return Array.isArray(parsed) ? parsed : seedBookings();
  } catch {
    return seedBookings();
  }
}

function loadUser(): string {
  try {
    return localStorage.getItem(USER_KEY) ?? "Dmytro";
  } catch {
    return "Dmytro";
  }
}

interface BookingsContextValue {
  bookings: Booking[];
  currentUser: string;
  setCurrentUser: (name: string) => void;
  addBooking: (booking: Omit<Booking, "id">) => Promise<{ booking: Booking; serverError: string | null }>;
  cancelBooking: (id: string) => void;
  isSlotFree: (roomId: number, date: string, startMinutes: number, endMinutes: number) => boolean;
}

const BookingsContext = createContext<BookingsContextValue | null>(null);

export function BookingsProvider({ children }: { children: ReactNode }) {
  const [bookings, setBookings] = useState<Booking[]>(loadBookings);
  const [currentUser, setCurrentUserState] = useState<string>(loadUser);

  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(bookings));
  }, [bookings]);

  useEffect(() => {
    localStorage.setItem(USER_KEY, currentUser);
  }, [currentUser]);

  const value = useMemo<BookingsContextValue>(
    () => ({
      bookings,
      currentUser,
      setCurrentUser: setCurrentUserState,
      addBooking: async (booking) => {
        const created: Booking = { ...booking, id: crypto.randomUUID() };

        let serverError: string | null = null;
        try {
          await bookRoomOnServer(booking.roomId);
        } catch (err) {
          serverError =
            err instanceof RoomBookingApiError
              ? err.message
              : "Unexpected error talking to the booking server.";
        }

        // The backend doesn't yet persist date/time/title, so the booking is
        // always recorded locally too — this keeps the UI usable while the
        // server-side room manager is still a stub.
        setBookings((prev) => [...prev, created]);
        return { booking: created, serverError };
      },
      cancelBooking: (id) => {
        setBookings((prev) => prev.filter((b) => b.id !== id));
      },
      isSlotFree: (roomId, date, startMinutes, endMinutes) =>
        !bookings.some(
          (b) =>
            b.roomId === roomId &&
            b.date === date &&
            startMinutes < b.endMinutes &&
            endMinutes > b.startMinutes,
        ),
    }),
    [bookings, currentUser],
  );

  return <BookingsContext.Provider value={value}>{children}</BookingsContext.Provider>;
}

export function useBookings(): BookingsContextValue {
  const ctx = useContext(BookingsContext);
  if (!ctx) throw new Error("useBookings must be used within BookingsProvider");
  return ctx;
}
