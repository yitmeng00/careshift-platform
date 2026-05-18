import { useDroppable } from "@dnd-kit/core";
import clsx from "clsx";
import { Plus } from "lucide-react";

import ShiftCard from "./ShiftCard";
import type { Shift } from "../../../types/shift";
import { toISODate } from "../../../utils/dateUtils";

interface DayColumnProps {
  date: Date;
  shifts: Shift[];
  isToday: boolean;
  canEdit: boolean;
  onLeaveStaffIds: Set<number>;
  onDelete: (id: number) => void;
  onCreateHere: (date: string) => void;
}

const DAY_NAMES = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

export default function DayColumn({
  date,
  shifts,
  isToday,
  canEdit,
  onLeaveStaffIds,
  onDelete,
  onCreateHere,
}: DayColumnProps) {
  const isoDate = toISODate(date);
  const { setNodeRef, isOver } = useDroppable({ id: isoDate });

  return (
    <div className="flex flex-col min-w-0">
      <div
        className={clsx(
          "text-center py-2 mb-2 rounded-lg",
          isToday ? "bg-accent text-white" : "text-slate-500",
        )}
      >
        <div className="text-xs font-medium uppercase tracking-wide">
          {DAY_NAMES[date.getDay()]}
        </div>
        <div
          className={clsx(
            "text-lg font-bold leading-tight",
            !isToday && "text-slate-800",
          )}
        >
          {date.getDate()}
        </div>
      </div>

      <div
        ref={setNodeRef}
        className={clsx(
          "flex-1 rounded-xl border-2 border-dashed p-1.5 flex flex-col gap-1.5 min-h-32 transition-colors",
          isOver ? "border-accent/40 bg-accent/5" : "border-slate-200",
        )}
      >
        {shifts.map((shift) => (
          <ShiftCard
            key={shift.id}
            shift={shift}
            canEdit={canEdit}
            isOnLeave={onLeaveStaffIds.has(shift.staffId)}
            onDelete={onDelete}
          />
        ))}
        {canEdit && (
          <button
            onClick={() => onCreateHere(isoDate)}
            className="mt-auto flex items-center justify-center py-1 rounded-lg text-slate-300 hover:text-accent hover:bg-accent/5 transition-colors cursor-pointer"
            aria-label="Add shift"
          >
            <Plus size={14} />
          </button>
        )}
      </div>
    </div>
  );
}
