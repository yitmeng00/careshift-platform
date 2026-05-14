const LEGEND_ITEMS = [
  { label: "Morning", time: "07:00 - 15:00", dot: "bg-blue-400" },
  { label: "Afternoon", time: "15:00 - 23:00", dot: "bg-amber-400" },
  { label: "Night", time: "23:00 - 07:00", dot: "bg-indigo-400" },
] as const;

export default function ShiftLegend() {
  return (
    <div className="flex items-center gap-4 flex-wrap">
      {LEGEND_ITEMS.map(({ label, time, dot }) => (
        <div key={label} className="flex items-center gap-1.5">
          <span className={`w-2.5 h-2.5 rounded-full shrink-0 ${dot}`} />
          <span className="text-xs text-slate-600 font-medium">{label}</span>
          <span className="text-xs text-slate-400">{time}</span>
        </div>
      ))}
    </div>
  );
}
