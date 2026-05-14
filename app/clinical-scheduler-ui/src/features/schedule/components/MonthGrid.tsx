import clsx from "clsx";

import type { Shift, ShiftType } from "../../../types/shift";
import { toISODate } from "../../../utils/dateUtils";

interface MonthGridProps {
  year: number;
  month: number; // 0-based
  shifts: Shift[];
}

const DAY_HEADERS = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

const SHIFT_TYPE_ORDER: ShiftType[] = ["Morning", "Afternoon", "Night"];

const SHIFT_STYLE: Record<
  ShiftType,
  { dot: string; text: string; bg: string }
> = {
  Morning: {
    dot: "bg-blue-400",
    text: "text-blue-700",
    bg: "bg-blue-50 border-blue-100",
  },
  Afternoon: {
    dot: "bg-amber-400",
    text: "text-amber-700",
    bg: "bg-amber-50 border-amber-100",
  },
  Night: {
    dot: "bg-indigo-400",
    text: "text-indigo-700",
    bg: "bg-indigo-50 border-indigo-100",
  },
};

export default function MonthGrid({ year, month, shifts }: MonthGridProps) {
  const today = toISODate(new Date());
  const monthStr = String(month + 1).padStart(2, "0");

  // First day of month: offset so week starts Monday (0=Mon…6=Sun)
  const firstDay = new Date(year, month, 1);
  const startOffset = (firstDay.getDay() + 6) % 7;
  const daysInMonth = new Date(year, month + 1, 0).getDate();

  const cells: (number | null)[] = [
    ...Array.from<null>({ length: startOffset }).fill(null),
    ...Array.from({ length: daysInMonth }, (_, i) => i + 1),
  ];
  while (cells.length % 7 !== 0) cells.push(null);

  // Group shifts by local ISO date → shift type counts
  const countsByDate = new Map<string, Record<ShiftType, number>>();
  for (const shift of shifts) {
    const d = toISODate(new Date(shift.startTime));
    if (!countsByDate.has(d)) {
      countsByDate.set(d, { Morning: 0, Afternoon: 0, Night: 0 });
    }
    const type = shift.shiftType as ShiftType;
    if (type in countsByDate.get(d)!) {
      countsByDate.get(d)![type]++;
    }
  }

  return (
    <div className="bg-white rounded-xl border border-slate-200 overflow-hidden">
      {/* Day-of-week headers */}
      <div className="grid grid-cols-7 border-b border-slate-100">
        {DAY_HEADERS.map((d) => (
          <div
            key={d}
            className="py-2 text-center text-xs font-medium text-slate-400 uppercase tracking-wide"
          >
            {d}
          </div>
        ))}
      </div>

      {/* Calendar cells */}
      <div className="grid grid-cols-7">
        {cells.map((day, idx) => {
          if (day === null) {
            return (
              <div
                key={`pad-${idx}`}
                className="min-h-28 border-b border-r border-slate-100 bg-slate-50/40"
              />
            );
          }

          const isoDate = `${year}-${monthStr}-${String(day).padStart(2, "0")}`;
          const counts = countsByDate.get(isoDate);
          const isToday = isoDate === today;
          const hasShifts =
            counts !== undefined && SHIFT_TYPE_ORDER.some((t) => counts[t] > 0);

          return (
            <div
              key={isoDate}
              className={clsx(
                "min-h-28 border-b border-r border-slate-100 p-1.5 flex flex-col gap-1 last:border-r-0",
                isToday && "bg-accent/5",
              )}
            >
              {/* Date number */}
              <div className="flex justify-end mb-0.5">
                <span
                  className={clsx(
                    "w-6 h-6 rounded-full flex items-center justify-center text-xs font-semibold",
                    isToday ? "bg-accent text-white" : "text-slate-500",
                  )}
                >
                  {day}
                </span>
              </div>

              {/* Shift type count rows */}
              {hasShifts ? (
                <div className="flex flex-col gap-1">
                  {SHIFT_TYPE_ORDER.map((type) => {
                    const count = counts?.[type] ?? 0;
                    if (count === 0) return null;
                    const s = SHIFT_STYLE[type];
                    return (
                      <div
                        key={type}
                        className={clsx(
                          "flex items-center gap-1 px-1.5 py-0.5 rounded border text-xs font-medium",
                          s.bg,
                          s.text,
                        )}
                      >
                        <span
                          className={clsx(
                            "w-1.5 h-1.5 rounded-full shrink-0",
                            s.dot,
                          )}
                        />
                        <span className="truncate">{type}</span>
                        <span className="ml-auto font-bold shrink-0">
                          ×{count}
                        </span>
                      </div>
                    );
                  })}
                </div>
              ) : null}
            </div>
          );
        })}
      </div>
    </div>
  );
}
