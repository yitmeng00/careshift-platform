import clsx from "clsx";
import { Plus } from "lucide-react";
import { useState } from "react";

import RequestSwapModal from "./components/RequestSwapModal";
import ViewSwapModal from "./components/ViewSwapModal";
import { useGetSwapRequestsQuery } from "./swapsApi";
import { useAppSelector } from "../../app/hooks";
import type { SwapRequest, SwapStatus } from "../../types/swap";
import { SWAP_STATUS_LABELS } from "../../types/swap";
import { formatShortDate } from "../../utils/dateUtils";

type TabFilter = "All" | "Pending" | "Approved" | "Rejected";

const TABS: TabFilter[] = ["All", "Pending", "Approved", "Rejected"];

const STATUS_STYLE: Record<SwapStatus, string> = {
  PendingRequestee: "bg-yellow-100 text-yellow-700",
  PendingAdmin: "bg-blue-100 text-blue-700",
  Approved: "bg-emerald-100 text-emerald-700",
  Rejected: "bg-red-100 text-red-700",
  Cancelled: "bg-slate-100 text-slate-500",
};

const filterSwaps = (swaps: SwapRequest[], tab: TabFilter) => {
  if (tab === "All") return swaps;
  if (tab === "Pending")
    return swaps.filter(
      (s) => s.status === "PendingRequestee" || s.status === "PendingAdmin",
    );
  if (tab === "Approved") return swaps.filter((s) => s.status === "Approved");
  return swaps.filter(
    (s) => s.status === "Rejected" || s.status === "Cancelled",
  );
};

const countFor = (swaps: SwapRequest[], tab: TabFilter) =>
  filterSwaps(swaps, tab).length;

export default function SwapsPage() {
  const user = useAppSelector((s) => s.auth.user);
  const [activeTab, setActiveTab] = useState<TabFilter>("All");
  const [requestOpen, setRequestOpen] = useState(false);
  const [viewing, setViewing] = useState<SwapRequest | null>(null);

  const { data: swaps = [], isLoading } = useGetSwapRequestsQuery();

  const canReview =
    user?.role === "Admin" ||
    user?.role === "DepartmentLead" ||
    user?.role === "ChargeNurse";

  const canSubmit = user?.role !== "Admin";

  const filtered = filterSwaps(swaps, activeTab);

  const getActionLabel = (swap: SwapRequest) => {
    if (swap.status === "PendingRequestee" && swap.requesteeId === user?.id)
      return "Respond";
    if (canReview && swap.status === "PendingAdmin") return "Review";
    return "View";
  };

  return (
    <div className="p-6 lg:p-8 max-w-6xl mx-auto">
      {/* Page header */}
      <div className="flex items-start justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Shift Swaps</h1>
          <p className="text-sm text-slate-500 mt-0.5">
            {swaps.filter(
              (s) =>
                (s.status === "PendingRequestee" &&
                  s.requesteeId === user?.id) ||
                (canReview && s.status === "PendingAdmin"),
            ).length > 0
              ? "You have requests awaiting action"
              : "No pending actions"}
          </p>
        </div>
        {canSubmit && (
          <button
            onClick={() => setRequestOpen(true)}
            className="flex items-center gap-2 px-4 py-2.5 rounded-xl bg-accent text-white text-sm font-semibold hover:bg-accent/90 transition-colors cursor-pointer shrink-0"
          >
            <Plus size={16} />
            Request Swap
          </button>
        )}
      </div>

      {/* Tabs */}
      <div className="flex gap-2 mb-5 flex-wrap">
        {TABS.map((tab) => {
          const count = countFor(swaps, tab);
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
          <div className="py-16 text-center text-sm text-slate-400">
            Loading…
          </div>
        ) : filtered.length === 0 ? (
          <div className="py-16 text-center text-sm text-slate-400">
            No {activeTab !== "All" ? activeTab.toLowerCase() : ""} swap
            requests.
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-slate-100">
                <th className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wide px-5 py-3">
                  Requester
                </th>
                <th className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wide px-3 py-3">
                  Requestee
                </th>
                <th className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wide px-3 py-3 hidden md:table-cell">
                  Shifts
                </th>
                <th className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wide px-3 py-3 hidden lg:table-cell">
                  Reason
                </th>
                <th className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wide px-3 py-3">
                  Status
                </th>
                <th className="px-3 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-50">
              {filtered.map((swap) => (
                <tr
                  key={swap.id}
                  className="hover:bg-slate-50/60 transition-colors"
                >
                  {/* Requester */}
                  <td className="px-5 py-3.5">
                    <div className="flex items-center gap-2.5">
                      <div className="w-8 h-8 rounded-lg bg-accent/10 text-accent text-xs font-bold flex items-center justify-center shrink-0">
                        {swap.requesterInitials}
                      </div>
                      <div>
                        <p className="font-medium text-slate-800">
                          {swap.requesterName}
                        </p>
                        <p className="text-xs text-slate-400">
                          {swap.requesterDepartment}
                        </p>
                      </div>
                    </div>
                  </td>

                  {/* Requestee */}
                  <td className="px-3 py-3.5">
                    <div className="flex items-center gap-2.5">
                      <div className="w-8 h-8 rounded-lg bg-slate-100 text-slate-500 text-xs font-bold flex items-center justify-center shrink-0">
                        {swap.requesteeInitials}
                      </div>
                      <div>
                        <p className="font-medium text-slate-800">
                          {swap.requesteeName}
                        </p>
                        <p className="text-xs text-slate-400">
                          {swap.requesteeDepartment}
                        </p>
                      </div>
                    </div>
                  </td>

                  {/* Shifts */}
                  <td className="px-3 py-3.5 hidden md:table-cell">
                    <p className="text-slate-700 whitespace-nowrap">
                      {formatShortDate(swap.requesterShiftDate)} ·{" "}
                      {swap.requesterShiftType}
                    </p>
                    <p className="text-xs text-slate-400 mt-0.5">
                      ↔ {formatShortDate(swap.requesteeShiftDate)} ·{" "}
                      {swap.requesteeShiftType}
                    </p>
                  </td>

                  {/* Reason */}
                  <td className="px-3 py-3.5 text-slate-500 max-w-40 truncate hidden lg:table-cell">
                    {swap.reason}
                  </td>

                  {/* Status */}
                  <td className="px-3 py-3.5">
                    <span
                      className={clsx(
                        "px-2.5 py-1 rounded-full text-xs font-semibold whitespace-nowrap",
                        STATUS_STYLE[swap.status],
                      )}
                    >
                      {SWAP_STATUS_LABELS[swap.status]}
                    </span>
                  </td>

                  {/* Action */}
                  <td className="px-3 py-3.5">
                    <button
                      onClick={() => setViewing(swap)}
                      className="px-3 py-1.5 rounded-lg border border-slate-200 text-xs font-medium text-slate-600 hover:bg-slate-50 hover:border-slate-300 transition-colors cursor-pointer"
                    >
                      {getActionLabel(swap)}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {requestOpen && (
        <RequestSwapModal onClose={() => setRequestOpen(false)} />
      )}
      {viewing && (
        <ViewSwapModal
          swap={viewing}
          myId={user!.id}
          canReview={canReview}
          onClose={() => setViewing(null)}
        />
      )}
    </div>
  );
}
