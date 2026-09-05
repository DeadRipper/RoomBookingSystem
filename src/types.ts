export type Amenity =
  | "TV Screen"
  | "Video Conferencing"
  | "Whiteboard"
  | "Projector"
  | "Phone"
  | "Wheelchair Access";

export interface Room {
  id: number;
  name: string;
  floor: string;
  capacity: number;
  amenities: Amenity[];
  image: string;
}

export interface Booking {
  id: string;
  roomId: number;
  title: string;
  bookedBy: string;
  date: string; // YYYY-MM-DD
  startMinutes: number; // minutes from midnight
  endMinutes: number;
}

export interface TimeSlot {
  startMinutes: number;
  endMinutes: number;
  label: string;
  available: boolean;
}
