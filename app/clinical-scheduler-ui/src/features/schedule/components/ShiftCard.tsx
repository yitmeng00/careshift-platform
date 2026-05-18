import { useDraggable } from "@dnd-kit/core";
import { CSS } from "@dnd-kit/utilities";
import clsx from "clsx";
import { Trash2 } from "lucide-react";

import type { Shift, ShiftType } from "../../../types/shift";

interface ShiftCardProps {
  shift: Shift;
  canEdit: boolean;
  isOnLeave?: boolean;
  onDelete: (id: number) => void;
}

const SHIFT_STYLES: Record<ShiftType, { badge: string; border: string }> = {
  Morning: { badge: "bg-blue-100 text-blue-700", border: "border-blue-200" },
  Afternoon: {
    badge: "bg-amber-100 text-amber-700",
    border: "border-amber-200",
  },
  Night: {
    badge: "bg-indigo-100 text-indigo-700",
    border: "border-indigo-200",
  },
};

export default function ShiftCard({
  shift,
  canEdit,
  isOnLeave = false,
  onDelete,
}: ShiftCardProps) {
  const { attributes, listeners, setNodeRef, transform, isDragging } =
    useDraggable({
      id: shift.id,
      disabled: !canEdit,
    });

  const transformStyle = transform
    ? { transform: CSS.Translate.toString(transform) }
    : undefined;
  const styles = SHIFT_STYLES[shift.shiftType];

  return (
    <div
      ref={setNodeRef}
      style={transformStyle}
      {...(canEdit ? { ...listeners, ...attributes } : {})}
      className={clsx(
        "group relative rounded-lg border p-2.5 bg-white select-none text-sm transition-shadow",
        styles.border,
        canEdit && "cursor-grab active:cursor-grabbing",
        isDragging && "opacity-50 shadow-lg z-50",
      )}
    >
      <div className="flex items-center gap-2 mb-1.5">
        <div className="w-6 h-6 rounded-full bg-accent/10 text-accent text-xs font-bold flex items-center justify-center shrink-0">
          {shift.staffInitials}
        </div>
        <span className="font-medium text-slate-800 truncate text-xs">
          {shift.staffName}
        </span>
      </div>
      <div className="flex items-center gap-1 flex-wrap">
        <span
          className={clsx(
            "text-xs px-1.5 py-0.5 rounded font-medium",
            styles.badge,
          )}
        >
          {shift.shiftType}
        </span>
        {isOnLeave && (
          <span className="text-xs px-1.5 py-0.5 rounded font-medium bg-orange-100 text-orange-600">
            On Leave
          </span>
        )}
      </div>
      {canEdit && (
        <button
          onPointerDown={(e) => e.stopPropagation()}
          onClick={() => onDelete(shift.id)}
          className="absolute top-1.5 right-1.5 hidden group-hover:flex w-5 h-5 items-center justify-center rounded text-slate-400 hover:text-red-500 hover:bg-red-50 transition-colors cursor-pointer"
          aria-label="Delete shift"
        >
          <Trash2 size={12} />
        </button>
      )}
    </div>
  );
}
