import type { AuditLogEntry } from "../../../types/audit";
import ActivityItem from "./ActivityItem";

interface ActivityFeedProps {
  entries?: AuditLogEntry[];
  isLoading?: boolean;
}

function SkeletonRow() {
  return (
    <div className="px-5 py-3 flex items-start gap-3 animate-pulse">
      <div className="w-7 h-7 rounded-lg bg-slate-100 shrink-0" />
      <div className="flex-1 space-y-1.5">
        <div className="h-3 bg-slate-100 rounded w-2/3" />
        <div className="h-3 bg-slate-100 rounded w-1/3" />
      </div>
    </div>
  );
}

export default function ActivityFeed({ entries, isLoading }: ActivityFeedProps) {
  return (
    <div className="bg-white rounded-xl border border-slate-200">
      <div className="px-5 py-4 border-b border-slate-100">
        <h2 className="text-sm font-semibold text-slate-900">Recent Activity</h2>
      </div>
      <div className="divide-y divide-slate-100">
        {isLoading ? (
          Array.from({ length: 4 }).map((_, index) => (
            <SkeletonRow key={index} />
          ))
        ) : entries && entries.length > 0 ? (
          entries.map((entry) => <ActivityItem key={entry.id} entry={entry} />)
        ) : (
          <p className="px-5 py-6 text-sm text-slate-400 text-center">
            No recent activity.
          </p>
        )}
      </div>
    </div>
  );
}
