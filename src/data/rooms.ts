import type { Room } from "../types";

export const rooms: Room[] = [
  {
    id: 1,
    name: "Aurora",
    floor: "Floor 2",
    capacity: 4,
    amenities: ["TV Screen", "Whiteboard"],
    image:
      "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800&auto=format&fit=crop&q=60",
  },
  {
    id: 2,
    name: "Nimbus",
    floor: "Floor 2",
    capacity: 8,
    amenities: ["Video Conferencing", "TV Screen", "Phone"],
    image:
      "https://images.unsplash.com/photo-1517502884422-41eaead166d4?w=800&auto=format&fit=crop&q=60",
  },
  {
    id: 3,
    name: "Zenith",
    floor: "Floor 3",
    capacity: 12,
    amenities: ["Projector", "Video Conferencing", "Whiteboard", "Wheelchair Access"],
    image:
      "https://images.unsplash.com/photo-1431540015161-0bf868a2d407?w=800&auto=format&fit=crop&q=60",
  },
  {
    id: 4,
    name: "Solace",
    floor: "Floor 1",
    capacity: 2,
    amenities: ["Phone", "Wheelchair Access"],
    image:
      "https://images.unsplash.com/photo-1497215728101-856f4ea42174?w=800&auto=format&fit=crop&q=60",
  },
  {
    id: 5,
    name: "Horizon",
    floor: "Floor 3",
    capacity: 20,
    amenities: ["Projector", "Video Conferencing", "TV Screen", "Phone"],
    image:
      "https://images.unsplash.com/photo-1524758631624-e2822e304c36?w=800&auto=format&fit=crop&q=60",
  },
  {
    id: 6,
    name: "Ember",
    floor: "Floor 1",
    capacity: 6,
    amenities: ["Whiteboard", "TV Screen"],
    image:
      "https://images.unsplash.com/photo-1552664730-d307ca884978?w=800&auto=format&fit=crop&q=60",
  },
];
