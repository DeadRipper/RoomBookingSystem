// Amenity names come from the backend (AmenityModel.Name), so they are free-form.
export type Amenity = string;

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
