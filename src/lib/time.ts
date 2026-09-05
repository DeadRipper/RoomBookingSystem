export function formatMinutes(minutes: number): string {
  const h = Math.floor(minutes / 60);
  const m = minutes % 60;
  const period = h >= 12 ? "PM" : "AM";
  const h12 = h % 12 === 0 ? 12 : h % 12;
  return `${h12}:${m.toString().padStart(2, "0")} ${period}`;
}

export function formatDateLabel(iso: string): string {
  const date = new Date(`${iso}T00:00:00`);
  return date.toLocaleDateString(undefined, {
    weekday: "short",
    month: "short",
    day: "numeric",
  });
}

export function isoDate(offsetDays = 0): string {
  const d = new Date();
  d.setDate(d.getDate() + offsetDays);
  return d.toISOString().slice(0, 10);
}

const DAY_START = 8 * 60; // 08:00
const DAY_END = 19 * 60; // 19:00
const SLOT_LENGTH = 30;

export function buildDaySlots(): { startMinutes: number; endMinutes: number }[] {
  const slots: { startMinutes: number; endMinutes: number }[] = [];
  for (let m = DAY_START; m < DAY_END; m += SLOT_LENGTH) {
    slots.push({ startMinutes: m, endMinutes: m + SLOT_LENGTH });
  }
  return slots;
}
