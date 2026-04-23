function getWeekRange(): string {
  const today = new Date();
  const day = today.getDay();
  const diff = today.getDate() - day + (day === 0 ? -6 : 1);
  const monday = new Date(today.setDate(diff));
  const sunday = new Date(monday);
  sunday.setDate(monday.getDate() + 6);

  const fmt = (d: Date) =>
    d.toLocaleDateString("en-US", { month: "short", day: "numeric" });
  const year = sunday.getFullYear();
  const weekNum = getWeekNumber(monday);

  return `Week ${weekNum} · ${fmt(monday)} – ${fmt(sunday)}, ${year}`;
}

function getWeekNumber(date: Date): number {
  const d = new Date(
    Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()),
  );
  const dayNum = d.getUTCDay() || 7;
  d.setUTCDate(d.getUTCDate() + 4 - dayNum);
  const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
  return Math.ceil(((d.getTime() - yearStart.getTime()) / 86400000 + 1) / 7);
}

export default function TopBar() {
  return (
    <div className="sticky top-0 flex items-center justify-between no-print bg-white z-90 border-b border-b-slate-200 h-13 px-9">
      <div className="text-sm text-slate-500 flex gap-4">
        <span className="text-slate-400">{getWeekRange().split(" · ")[0]}</span>
        <span>{getWeekRange().split(" · ")[1]}</span>
      </div>
      <div className="flex items-center gap-3 text-sm text-slate-500">
        <div className="flex items-center gap-1.5">
          <div className="w-2 h-2 rounded-full bg-emerald-500" />
          <span>System online</span>
        </div>
      </div>
    </div>
  );
}
