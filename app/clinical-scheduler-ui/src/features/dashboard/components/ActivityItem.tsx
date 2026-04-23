import type { AuditLogEntry } from "../../../types/audit";
import { formatRelativeTime } from "../../../utils/dateUtils";

interface ActivityItemProps {
  entry: AuditLogEntry;
}

export default function ActivityItem({ entry }: ActivityItemProps) {
  return (
    <div className="px-5 py-3 flex items-start gap-3">
      <div className="w-7 h-7 rounded-lg bg-slate-100 flex items-center justify-center text-sm shrink-0 text-slate-500">
        {entry.icon ?? "·"}
      </div>
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-slate-800">{entry.action}</p>
        {entry.detail && (
          <p className="text-xs text-slate-400 mt-0.5">{entry.detail}</p>
        )}
        <p className="text-xs text-slate-400 mt-0.5">by {entry.performedBy}</p>
      </div>
      <span className="text-xs text-slate-400 shrink-0 pt-0.5">
        {formatRelativeTime(entry.timestamp)}
      </span>
    </div>
  );
}
