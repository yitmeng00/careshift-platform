import {
  DndContext,
  PointerSensor,
  useSensor,
  useSensors,
} from "@dnd-kit/core";
import type { DragEndEvent } from "@dnd-kit/core";

import DayColumn from "./DayColumn";
import type { ApprovedLeave } from "../../../types/leave";
import type { Shift } from "../../../types/shift";
import { toISODate } from "../../../utils/dateUtils";

interface WeeklyGridProps {
  weekDays: Date[];
  shifts: Shift[];
  canEdit: boolean;
  approvedLeaves: ApprovedLeave[];
  onDeleteShift: (id: number) => void;
  onCreateShift: (date: string) => void;
  onMoveShift: (shiftId: number, newDate: string) => void;
}

export default function WeeklyGrid({
  weekDays,
  shifts,
  canEdit,
  approvedLeaves,
  onDeleteShift,
  onCreateShift,
  onMoveShift,
}: WeeklyGridProps) {
  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 8 } }),
  );
  const today = toISODate(new Date());

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;
    if (!over) return;

    const shiftId = active.id as number;
    const newDate = over.id as string;

    const currentShift = shifts.find((s) => s.id === shiftId);
    if (!currentShift) return;

    const currentDate = toISODate(new Date(currentShift.startTime));
    if (currentDate === newDate) return;

    onMoveShift(shiftId, newDate);
  };

  const onLeaveForDate = (isoDate: string): Set<number> =>
    new Set(
      approvedLeaves
        .filter((l) => l.startDate <= isoDate && l.endDate >= isoDate)
        .map((l) => l.staffId),
    );

  return (
    <DndContext sensors={sensors} onDragEnd={handleDragEnd}>
      <div className="grid grid-cols-7 gap-3">
        {weekDays.map((day) => {
          const isoDate = toISODate(day);
          const dayShifts = shifts.filter(
            (shift) => toISODate(new Date(shift.startTime)) === isoDate,
          );

          return (
            <DayColumn
              key={isoDate}
              date={day}
              shifts={dayShifts}
              isToday={isoDate === today}
              canEdit={canEdit}
              onLeaveStaffIds={onLeaveForDate(isoDate)}
              onDelete={onDeleteShift}
              onCreateHere={onCreateShift}
            />
          );
        })}
      </div>
    </DndContext>
  );
}
