import clsx from "clsx";
import { Plus } from "lucide-react";
import { useState } from "react";

import { useAppSelector } from "../../app/hooks";
import type { LeaveRequest, LeaveStatus } from "../../types/leave";
import { LEAVE_TYPE_LABELS } from "../../types/leave";
import ReviewLeaveModal from "./components/ReviewLeaveModal";
import SubmitLeaveModal from "./components/SubmitLeaveModal";
import { filterByStatus, selectPendingCount, useGetLeaveRequestsQuery } from "./leavesApi";

type TabStatus = "All" | LeaveStatus;

const STATUS_STYLE: Record<string, string> = {
  Pending: "bg-yellow-100 text-yellow-700",
  Approved: "bg-emerald-100 text-emerald-700",
  Rejected: "bg-red-100 text-red-700",
};

const TABS: TabStatus[] = ["All", "Pending", "Approved", "Rejected"];

function formatDate(iso: string) {
  const d = new Date(iso + "T00:00:00");
  return d.toLocaleDateString("en-US", { month: "short", day: "numeric" });
}

function formatDateRange(start: string, end: string) {
  if (start === end) return formatDate(start);
  return `${formatDate(start)} – ${formatDate(end)}`;
}

function formatSubmitted(iso: string) {
  return new Date(iso).toLocaleDateString("en-US", { month: "short", day: "numeric" });
}

export default function LeavesPage() {
  const user = useAppSelector((s) => s.auth.user);
  const [activeTab, setActiveTab] = useState<TabStatus>("All");
  const [submitOpen, setSubmitOpen] = useState(false);
  const [reviewing, setReviewing] = useState<LeaveRequest | null>(null);

  const { data: leaves = [], isLoading } = useGetLeaveRequestsQuery();

  const canReview =
    user?.role === "Admin" ||
    user?.role === "DepartmentLead" ||
    user?.role === "ChargeNurse";

  const canSubmit = user?.role !== "Admin";

  const pendingCount = selectPendingCount(leaves);
  const filtered = filterByStatus(leaves, activeTab);

  const countFor = (tab: TabStatus) =>
    tab === "All" ? leaves.length : leaves.filter((l) => l.status === tab).length;

  return (
    <div className="p-6 lg:p-8 max-w-6xl mx-auto">
      {/* Page header */}
      <div className="flex items-start justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Leave Requests</h1>
          {pendingCount > 0 ? (
            <p className="text-sm text-slate-500 mt-0.5">
              {pendingCount} pending approval
            </p>
          ) : (
            <p className="text-sm text-slate-500 mt-0.5">No pending approvals</p>
          )}
        </div>
        {canSubmit && (
          <button
            onClick={() => setSubmitOpen(true)}
            className="flex items-center gap-2 px-4 py-2.5 rounded-xl bg-accent text-white text-sm font-semibold hover:bg-accent/90 transition-colors cursor-pointer shrink-0"
          >
            <Plus size={16} />
            Request Leave
          </button>
        )}
      </div>

      {/* Status tabs */}
      <div className="flex gap-2 mb-5 flex-wrap">
        {TABS.map((tab) => {
          const count = countFor(tab);
          return (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={clsx(
                "flex items-center gap-1.5 px-4 py-1.5 rounded-full text-sm font-medium transition-colors cursor-pointer",
                activeTab === tab
                  ? "bg-accent text-white"
                  : "bg-white border border-slate-200 text-slate-600 hover:border-slate-300",
              )}
            >
              {tab}
              {tab !== "All" && count > 0 && (
                <span
                  className={clsx(
                    "text-xs font-semibold",
                    activeTab === tab ? "text-white/80" : "text-slate-500",
                  )}
                >
                  {count}
                </span>
              )}
            </button>
          );
        })}
      </div>

      {/* Table */}
      <div className="bg-white rounded-xl border border-slate-200 overflow-hidden">
        {isLoading ? (
          <div className="py-16 text-center text-sm text-slate-400">Loading…</div>
        ) : filtered.length === 0 ? (
          <div className="py-16 text-center text-sm text-slate-400">
            No {activeTab !== "All" ? activeTab.toLowerCase() : ""} leave requests.
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-slate-100">
                <th className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wide px-5 py-3">
                  Staff
                </th>
                <th className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wide px-3 py-3">
                  Type
                </th>
                <th className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wide px-3 py-3">
                  Dates
                </th>
                <th className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wide px-3 py-3 hidden md:table-cell">
                  Reason
                </th>
                <th className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wide px-3 py-3 hidden lg:table-cell">
                  Submitted
                </th>
                <th className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wide px-3 py-3">
                  Status
                </th>
                <th className="px-3 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-50">
              {filtered.map((leave) => (
                <tr
                  key={leave.id}
                  className="hover:bg-slate-50/60 transition-colors"
                >
                  {/* Staff */}
                  <td className="px-5 py-3.5">
                    <div className="flex items-center gap-2.5">
                      <div className="w-8 h-8 rounded-lg bg-accent/10 text-accent text-xs font-bold flex items-center justify-center shrink-0">
                        {leave.staffInitials}
                      </div>
                      <div>
                        <p className="font-medium text-slate-800">{leave.staffFullName}</p>
                        <p className="text-xs text-slate-400">{leave.staffDepartment}</p>
                      </div>
                    </div>
                  </td>

                  {/* Type */}
                  <td className="px-3 py-3.5">
                    <span className="px-2.5 py-1 rounded-full bg-slate-100 text-slate-600 text-xs font-medium">
                      {LEAVE_TYPE_LABELS[leave.leaveType]}
                    </span>
                  </td>

                  {/* Dates */}
                  <td className="px-3 py-3.5 text-slate-700 whitespace-nowrap">
                    {formatDateRange(leave.startDate, leave.endDate)}
                  </td>

                  {/* Reason */}
                  <td className="px-3 py-3.5 text-slate-500 max-w-48 truncate hidden md:table-cell">
                    {leave.reason}
                  </td>

                  {/* Submitted */}
                  <td className="px-3 py-3.5 text-slate-400 hidden lg:table-cell">
                    {formatSubmitted(leave.submittedAt)}
                  </td>

                  {/* Status badge */}
                  <td className="px-3 py-3.5">
                    <span
                      className={clsx(
                        "px-2.5 py-1 rounded-full text-xs font-semibold",
                        STATUS_STYLE[leave.status],
                      )}
                    >
                      {leave.status}
                    </span>
                  </td>

                  {/* Action */}
                  <td className="px-3 py-3.5">
                    <button
                      onClick={() => setReviewing(leave)}
                      className="px-3 py-1.5 rounded-lg border border-slate-200 text-xs font-medium text-slate-600 hover:bg-slate-50 hover:border-slate-300 transition-colors cursor-pointer"
                    >
                      {canReview && leave.status === "Pending" ? "Review" : "View"}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {submitOpen && <SubmitLeaveModal onClose={() => setSubmitOpen(false)} />}
      {reviewing && (
        <ReviewLeaveModal
          leave={reviewing}
          canReview={canReview}
          onClose={() => setReviewing(null)}
        />
      )}
    </div>
  );
}
