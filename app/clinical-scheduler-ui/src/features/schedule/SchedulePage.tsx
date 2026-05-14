import { ChevronLeft, ChevronRight, Plus } from "lucide-react";
import { useMemo, useState } from "react";
import type { z } from "zod";

import CreateShiftModal from "./components/CreateShiftModal";
import MonthGrid from "./components/MonthGrid";
import ShiftLegend from "./components/ShiftLegend";
import WeeklyGrid from "./components/WeeklyGrid";
import {
  useCreateShiftMutation,
  useDeleteShiftMutation,
  useGetShiftsByMonthQuery,
  useGetShiftsByWeekQuery,
  useUpdateShiftMutation,
} from "./shiftsApi";
import { useAppSelector } from "../../app/hooks";
import { useWeekNavigation } from "../../hooks/useWeekNavigation";
import type { createShiftSchema } from "../../schemas/shift";
import { useGetStaffListQuery } from "../../services/staffApi";
import { toISODate } from "../../utils/dateUtils";

type CreateShiftFormValues = z.infer<typeof createShiftSchema>;
type ViewMode = "week" | "month";

const getMonthLabel = (year: number, month: number): string => {
  return new Date(year, month, 1).toLocaleDateString("en-US", {
    month: "long",
    year: "numeric",
  });
};

export default function SchedulePage() {
  const { weekDays, weekLabel, weekStartIso, nextWeek, prevWeek, goToToday } =
    useWeekNavigation();
  const [view, setView] = useState<ViewMode>("week");
  const [modalOpen, setModalOpen] = useState(false);
  const [defaultDate, setDefaultDate] = useState<string | undefined>();
  const [deptFilter, setDeptFilter] = useState<number | undefined>();

  // Month navigation (independent of week nav)
  const today = new Date();
  const [monthYear, setMonthYear] = useState(today.getFullYear());
  const [monthMonth, setMonthMonth] = useState(today.getMonth()); // 0-based

  const monthStart = `${monthYear}-${String(monthMonth + 1).padStart(2, "0")}-01`;

  const user = useAppSelector((state) => state.auth.user);
  const canEdit = user?.role === "Admin" || user?.role === "DepartmentLead";

  // Build department options from staff list
  const { data: staffList = [] } = useGetStaffListQuery();
  const departments = useMemo(() => {
    const seen = new Map<number, string>();
    for (const s of staffList) {
      if (!seen.has(s.departmentId)) seen.set(s.departmentId, s.departmentName);
    }
    return [...seen.entries()]
      .map(([id, name]) => ({ id, name }))
      .sort((a, b) => a.name.localeCompare(b.name));
  }, [staffList]);

  const { data: weekShifts = [], isLoading: weekLoading } =
    useGetShiftsByWeekQuery(
      { weekStart: weekStartIso, departmentId: deptFilter },
      { skip: view !== "week" },
    );

  const { data: monthShifts = [], isLoading: monthLoading } =
    useGetShiftsByMonthQuery(
      { monthStart, departmentId: deptFilter },
      { skip: view !== "month" },
    );

  const [createShift, { isLoading: isCreating }] = useCreateShiftMutation();
  const [deleteShift] = useDeleteShiftMutation();
  const [updateShift] = useUpdateShiftMutation();

  const openCreate = (date?: string) => {
    setDefaultDate(date);
    setModalOpen(true);
  };

  const handleCreate = async (values: CreateShiftFormValues) => {
    await createShift({
      staffId: Number(values.staffId),
      departmentId: Number(values.departmentId),
      date: values.date,
      shiftType: values.shiftType,
      notes: values.notes,
    }).unwrap();
    setModalOpen(false);
  };

  const handleDelete = async (id: number) => {
    await deleteShift(id).unwrap();
  };

  const handleMove = async (shiftId: number, newDate: string) => {
    await updateShift({ id: shiftId, newDate }).unwrap();
  };

  const prevMonth = () => {
    if (monthMonth === 0) {
      setMonthYear((y) => y - 1);
      setMonthMonth(11);
    } else setMonthMonth((m) => m - 1);
  };
  const nextMonth = () => {
    if (monthMonth === 11) {
      setMonthYear((y) => y + 1);
      setMonthMonth(0);
    } else setMonthMonth((m) => m + 1);
  };
  const goToThisMonth = () => {
    setMonthYear(today.getFullYear());
    setMonthMonth(today.getMonth());
  };

  const isLoading = view === "week" ? weekLoading : monthLoading;

  return (
    <div className="p-4 lg:p-9">
      {/* Header row */}
      <div className="flex items-start justify-between mb-5 gap-4 flex-wrap">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 tracking-tight">
            Schedule
          </h1>
          <p className="text-sm text-slate-500 mt-0.5">
            {view === "week" ? weekLabel : getMonthLabel(monthYear, monthMonth)}
          </p>
        </div>

        <div className="flex items-center gap-2 flex-wrap">
          {/* View toggle */}
          <div className="flex rounded-lg border border-slate-200 overflow-hidden text-sm">
            <button
              onClick={() => setView("week")}
              className={`px-3 py-1.5 transition-colors cursor-pointer ${view === "week" ? "bg-accent text-white font-medium" : "text-slate-600 hover:bg-slate-50"}`}
            >
              Week
            </button>
            <button
              onClick={() => setView("month")}
              className={`px-3 py-1.5 transition-colors cursor-pointer border-l border-slate-200 ${view === "month" ? "bg-accent text-white font-medium" : "text-slate-600 hover:bg-slate-50"}`}
            >
              Month
            </button>
          </div>

          {/* Department filter */}
          <select
            value={deptFilter ?? ""}
            onChange={(e) =>
              setDeptFilter(e.target.value ? Number(e.target.value) : undefined)
            }
            className="form-input rounded-lg border border-slate-200 text-sm px-3 py-1.5 text-slate-700 cursor-pointer focus:outline-none"
          >
            <option value="">All departments</option>
            {departments.map((d) => (
              <option key={d.id} value={d.id}>
                {d.name}
              </option>
            ))}
          </select>

          {/* Navigation */}
          <button
            onClick={view === "week" ? prevWeek : prevMonth}
            className="w-8 h-8 flex items-center justify-center rounded-lg border border-slate-200 text-slate-600 hover:text-slate-900 hover:bg-slate-50 transition-colors cursor-pointer"
            aria-label="Previous"
          >
            <ChevronLeft size={16} />
          </button>
          <button
            onClick={view === "week" ? goToToday : goToThisMonth}
            className="px-3 py-1.5 text-sm rounded-lg border border-slate-200 text-slate-700 hover:bg-slate-50 transition-colors cursor-pointer"
          >
            Today
          </button>
          <button
            onClick={view === "week" ? nextWeek : nextMonth}
            className="w-8 h-8 flex items-center justify-center rounded-lg border border-slate-200 text-slate-600 hover:text-slate-900 hover:bg-slate-50 transition-colors cursor-pointer"
            aria-label="Next"
          >
            <ChevronRight size={16} />
          </button>

          {canEdit && (
            <button
              onClick={() =>
                openCreate(view === "week" ? undefined : toISODate(new Date()))
              }
              className="flex items-center gap-1.5 px-3 py-1.5 bg-accent text-white text-sm font-medium rounded-lg hover:bg-accent/90 transition-colors cursor-pointer"
            >
              <Plus size={15} />
              New Shift
            </button>
          )}
        </div>
      </div>

      {/* Legend */}
      <div className="mb-4">
        <ShiftLegend />
      </div>

      {/* Grid */}
      {isLoading ? (
        view === "week" ? (
          <div className="grid grid-cols-7 gap-3">
            {Array.from({ length: 7 }).map((_, i) => (
              <div
                key={i}
                className="h-64 rounded-xl bg-slate-100 animate-pulse"
              />
            ))}
          </div>
        ) : (
          <div className="h-96 rounded-xl bg-slate-100 animate-pulse" />
        )
      ) : view === "week" ? (
        <div className="overflow-x-auto">
          <div className="min-w-175">
            <WeeklyGrid
              weekDays={weekDays}
              shifts={weekShifts}
              canEdit={canEdit}
              onDeleteShift={handleDelete}
              onCreateShift={openCreate}
              onMoveShift={handleMove}
            />
          </div>
        </div>
      ) : (
        <MonthGrid year={monthYear} month={monthMonth} shifts={monthShifts} />
      )}

      {modalOpen && (
        <CreateShiftModal
          defaultDate={defaultDate}
          onSubmit={handleCreate}
          onClose={() => setModalOpen(false)}
          isSubmitting={isCreating}
        />
      )}
    </div>
  );
}
