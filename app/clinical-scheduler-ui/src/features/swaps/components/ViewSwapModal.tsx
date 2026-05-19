import clsx from "clsx";
import { ArrowLeftRight, Check, X } from "lucide-react";
import { useState } from "react";

import type { SwapRequest } from "../../../types/swap";
import { SWAP_STATUS_LABELS } from "../../../types/swap";
import { formatDateTime, formatShortDate } from "../../../utils/dateUtils";
import {
  useRespondSwapMutation,
  useReviewSwapMutation,
  useCancelSwapMutation,
} from "../swapsApi";

interface ViewSwapModalProps {
  swap: SwapRequest;
  myId: number;
  canReview: boolean;
  onClose: () => void;
}

const STATUS_STYLE: Record<string, string> = {
  PendingRequestee: "bg-yellow-100 text-yellow-700",
  PendingAdmin: "bg-blue-100 text-blue-700",
  Approved: "bg-emerald-100 text-emerald-700",
  Rejected: "bg-red-100 text-red-700",
  Cancelled: "bg-slate-100 text-slate-500",
};

const ACTION_LABEL: Record<string, string> = {
  submitted: "submitted this request",
  accepted: "accepted this request",
  rejected: "rejected this request",
  approved: "approved this request",
  cancelled: "cancelled this request",
};

export default function ViewSwapModal({
  swap,
  myId,
  canReview,
  onClose,
}: ViewSwapModalProps) {
  const [note, setNote] = useState("");
  const [respondSwap, { isLoading: responding }] = useRespondSwapMutation();
  const [reviewSwap, { isLoading: reviewing }] = useReviewSwapMutation();
  const [cancelSwap, { isLoading: cancelling }] = useCancelSwapMutation();

  const isLoading = responding || reviewing || cancelling;
  const isRequester = swap.requesterId === myId;
  const isRequestee = swap.requesteeId === myId;
  const canRespond = isRequestee && swap.status === "PendingRequestee";
  const canAdminReview = canReview && swap.status === "PendingAdmin";
  const canCancel =
    isRequester &&
    (swap.status === "PendingRequestee" || swap.status === "PendingAdmin");

  const handleRespond = async (action: "Accept" | "Reject") => {
    await respondSwap({
      id: swap.id,
      action,
      note: note || undefined,
    }).unwrap();
    onClose();
  };

  const handleReview = async (action: "Approve" | "Reject") => {
    await reviewSwap({ id: swap.id, action, note: note || undefined }).unwrap();
    onClose();
  };

  const handleCancel = async () => {
    await cancelSwap(swap.id).unwrap();
    onClose();
  };

  return (
    <div
      className="fixed inset-0 z-200 flex items-center justify-center p-4 bg-black/40"
      onClick={onClose}
    >
      <div
        className="bg-white rounded-2xl shadow-xl w-full max-w-md max-h-[90vh] overflow-y-auto"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="p-6">
          {/* Header */}
          <div className="flex items-start justify-between mb-1">
            <div>
              <h2 className="text-xl font-bold text-slate-900">Shift Swap</h2>
              <p className="text-sm text-slate-500 mt-0.5">
                {swap.requesterName} → {swap.requesteeName}
              </p>
            </div>
            <span
              className={clsx(
                "px-3 py-1 rounded-full text-xs font-semibold shrink-0 ml-3",
                STATUS_STYLE[swap.status],
              )}
            >
              {SWAP_STATUS_LABELS[swap.status]}
            </span>
          </div>

          {/* Shift swap card */}
          <div className="bg-slate-50 rounded-xl p-4 mt-4 space-y-3">
            <div className="flex items-center gap-3">
              <div className="flex-1">
                <p className="text-xs font-semibold text-slate-400 uppercase tracking-wide mb-1">
                  {swap.requesterName}
                </p>
                <p className="text-sm font-medium text-slate-800">
                  {formatShortDate(swap.requesterShiftDate)} ·{" "}
                  {swap.requesterShiftType}
                </p>
                <p className="text-xs text-slate-400 mt-0.5">
                  {swap.requesterDepartment}
                </p>
              </div>
              <ArrowLeftRight size={16} className="text-slate-400 shrink-0" />
              <div className="flex-1 text-right">
                <p className="text-xs font-semibold text-slate-400 uppercase tracking-wide mb-1">
                  {swap.requesteeName}
                </p>
                <p className="text-sm font-medium text-slate-800">
                  {formatShortDate(swap.requesteeShiftDate)} ·{" "}
                  {swap.requesteeShiftType}
                </p>
                <p className="text-xs text-slate-400 mt-0.5">
                  {swap.requesteeDepartment}
                </p>
              </div>
            </div>
            <div>
              <p className="text-xs font-semibold text-slate-400 uppercase tracking-wide mb-1">
                Reason
              </p>
              <p className="text-sm text-slate-700">{swap.reason}</p>
            </div>
          </div>

          {/* Audit trail */}
          {swap.auditEntries.length > 0 && (
            <div className="mt-5">
              <p className="text-xs font-semibold text-slate-400 uppercase tracking-wide mb-3">
                Audit Trail
              </p>
              <div className="space-y-3">
                {swap.auditEntries.map((entry, i) => (
                  <div key={i} className="flex gap-3 items-start">
                    <div className="w-2 h-2 rounded-full bg-accent mt-1.5 shrink-0" />
                    <div>
                      <p className="text-sm text-slate-700">
                        <span className="font-semibold">{entry.by}</span>{" "}
                        {ACTION_LABEL[entry.action] ?? entry.action}
                        {entry.note && (
                          <span className="text-slate-500">
                            {" "}
                            — {entry.note}
                          </span>
                        )}
                      </p>
                      <p className="text-xs text-slate-400 mt-0.5">
                        {formatDateTime(entry.at)}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Respond — requestee accepting/rejecting */}
          {canRespond && (
            <div className="mt-5 space-y-3">
              <div className="border-t border-slate-100 pt-4">
                <textarea
                  value={note}
                  onChange={(e) => setNote(e.target.value)}
                  rows={2}
                  placeholder="Add a note (optional)…"
                  className="form-input w-full rounded-xl text-sm px-3 py-2.5 resize-none"
                />
              </div>
              <div className="flex gap-3">
                <button
                  onClick={() => handleRespond("Accept")}
                  disabled={isLoading}
                  className="flex-1 flex items-center justify-center gap-2 py-2.5 rounded-xl border border-emerald-300 bg-emerald-50 text-emerald-700 text-sm font-semibold hover:bg-emerald-100 disabled:opacity-60 transition-colors cursor-pointer"
                >
                  <Check size={15} />
                  Accept
                </button>
                <button
                  onClick={() => handleRespond("Reject")}
                  disabled={isLoading}
                  className="flex-1 flex items-center justify-center gap-2 py-2.5 rounded-xl border border-red-300 bg-red-50 text-red-700 text-sm font-semibold hover:bg-red-100 disabled:opacity-60 transition-colors cursor-pointer"
                >
                  <X size={15} />
                  Reject
                </button>
              </div>
            </div>
          )}

          {/* Review — admin/lead/charge approving/rejecting */}
          {canAdminReview && (
            <div className="mt-5 space-y-3">
              <div className="border-t border-slate-100 pt-4">
                <textarea
                  value={note}
                  onChange={(e) => setNote(e.target.value)}
                  rows={2}
                  placeholder="Add a note (optional)…"
                  className="form-input w-full rounded-xl text-sm px-3 py-2.5 resize-none"
                />
              </div>
              <div className="flex gap-3">
                <button
                  onClick={() => handleReview("Approve")}
                  disabled={isLoading}
                  className="flex-1 flex items-center justify-center gap-2 py-2.5 rounded-xl border border-emerald-300 bg-emerald-50 text-emerald-700 text-sm font-semibold hover:bg-emerald-100 disabled:opacity-60 transition-colors cursor-pointer"
                >
                  <Check size={15} />
                  Approve
                </button>
                <button
                  onClick={() => handleReview("Reject")}
                  disabled={isLoading}
                  className="flex-1 flex items-center justify-center gap-2 py-2.5 rounded-xl border border-red-300 bg-red-50 text-red-700 text-sm font-semibold hover:bg-red-100 disabled:opacity-60 transition-colors cursor-pointer"
                >
                  <X size={15} />
                  Reject
                </button>
              </div>
            </div>
          )}

          <div
            className={clsx(
              "flex gap-3",
              canRespond || canAdminReview ? "mt-3" : "mt-5",
            )}
          >
            {canCancel && (
              <button
                onClick={handleCancel}
                disabled={isLoading}
                className="flex-1 py-2.5 rounded-xl border border-red-200 text-sm font-medium text-red-600 hover:bg-red-50 disabled:opacity-60 transition-colors cursor-pointer"
              >
                {cancelling ? "Cancelling…" : "Cancel Request"}
              </button>
            )}
            <button
              onClick={onClose}
              className={clsx(
                "py-2.5 rounded-xl border border-slate-200 text-sm font-medium text-slate-600 hover:bg-slate-50 transition-colors cursor-pointer",
                canCancel ? "flex-1" : "w-full",
              )}
            >
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
