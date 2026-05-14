interface OvertimeAlertsPanelProps {
  count: number;
  isLoading?: boolean;
}

export default function OvertimeAlertsPanel({
  count,
  isLoading,
}: OvertimeAlertsPanelProps) {
  return (
    <div className="bg-white rounded-xl border border-slate-200">
      <div className="px-5 py-4 border-b border-slate-100">
        <h2 className="text-sm font-semibold text-slate-900">
          Overtime Alerts
        </h2>
      </div>
      <div className="px-5 py-6 flex flex-col items-center justify-center gap-2">
        {isLoading ? (
          <div className="w-12 h-10 bg-slate-100 rounded animate-pulse" />
        ) : (
          <>
            <span
              className={`text-3xl font-bold ${count > 0 ? "text-red-500" : "text-slate-300"}`}
            >
              {count}
            </span>
            <p className="text-xs text-slate-400 text-center">
              {count === 0
                ? "No staff over 40 hrs this week"
                : `staff member${count !== 1 ? "s" : ""} over 40 hrs this week`}
            </p>
          </>
        )}
      </div>
    </div>
  );
}
