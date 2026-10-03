import { useEffect, useState, type FormEvent } from "react";
import { addRoomOnServer, fetchAmenitiesFromServer, type AmenityOption } from "../api/roomBookingApi";
import { useRooms } from "../store/RoomsContext";

const field =
  "w-full rounded-lg border border-white/10 bg-white/5 px-3 py-2 text-sm text-slate-100 outline-none placeholder:text-slate-500 focus:border-indigo-400/60 focus:ring-1 focus:ring-indigo-400/40";

export default function AddRoomForm() {
  const { addRoomLocal } = useRooms();
  const [name, setName] = useState("");
  const [floor, setFloor] = useState("1");
  const [capacity, setCapacity] = useState("4");
  const [amenities, setAmenities] = useState<AmenityOption[]>([]);
  const [amenityName, setAmenityName] = useState("");
  const [image, setImage] = useState("");
  const [busy, setBusy] = useState(false);
  const [message, setMessage] = useState<{ ok: boolean; text: string } | null>(null);

  useEffect(() => {
    let cancelled = false;
    fetchAmenitiesFromServer()
      .then((list) => {
        if (cancelled) return;
        setAmenities(list);
        setAmenityName((current) => current || list[0]?.name || "");
      })
      .catch(() => {});
    return () => {
      cancelled = true;
    };
  }, []);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setBusy(true);
    setMessage(null);
    try {
      const room = await addRoomOnServer({
        name: name.trim(),
        floor: Number(floor),
        capacity: Number(capacity),
        amenity: { id: amenities.find((a) => a.name === amenityName)?.id ?? 0, name: amenityName.trim() },
        image: image.trim(),
      });
      addRoomLocal(room);
      setMessage({ ok: true, text: `Room "${room.name}" added (id ${room.id}).` });
      setName("");
      setImage("");
    } catch (err) {
      setMessage({ ok: false, text: err instanceof Error ? err.message : "Unexpected error." });
    } finally {
      setBusy(false);
    }
  }

  return (
    <form
      onSubmit={onSubmit}
      className="flex flex-col gap-4 rounded-2xl border border-white/10 bg-white/[0.03] p-5"
    >
      <h2 className="text-lg font-semibold text-slate-50">Add room</h2>

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
        <label className="flex flex-col gap-1 text-sm text-slate-400 sm:col-span-3">
          Name
          <input value={name} onChange={(e) => setName(e.target.value)} required className={field} />
        </label>
        <label className="flex flex-col gap-1 text-sm text-slate-400">
          Floor
          <input type="number" min={0} value={floor} onChange={(e) => setFloor(e.target.value)} required className={field} />
        </label>
        <label className="flex flex-col gap-1 text-sm text-slate-400">
          Capacity
          <input type="number" min={1} value={capacity} onChange={(e) => setCapacity(e.target.value)} required className={field} />
        </label>
        <label className="flex flex-col gap-1 text-sm text-slate-400 sm:col-span-2">
          Amenity
          {amenities.length > 0 ? (
            <select value={amenityName} onChange={(e) => setAmenityName(e.target.value)} required className={field}>
              {amenities.map((a) => (
                <option key={a.name} value={a.name} className="bg-[#0b0e14]">
                  {a.name}
                </option>
              ))}
            </select>
          ) : (
            <input
              value={amenityName}
              onChange={(e) => setAmenityName(e.target.value)}
              placeholder="Could not load amenities - type a name"
              required
              className={field}
            />
          )}
        </label>
        <label className="flex flex-col gap-1 text-sm text-slate-400 sm:col-span-3">
          Image URL
          <input value={image} onChange={(e) => setImage(e.target.value)} required className={field} />
        </label>
      </div>

      <div className="flex items-center gap-3">
        <button
          disabled={busy}
          className="rounded-lg bg-indigo-500 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-400 disabled:opacity-60"
        >
          {busy ? "Adding…" : "Add room"}
        </button>
        {message && (
          <p className={`text-sm ${message.ok ? "text-teal-300" : "text-red-400"}`}>{message.text}</p>
        )}
      </div>
    </form>
  );
}
