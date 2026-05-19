export type SwapStatus =
  | "PendingRequestee"
  | "PendingAdmin"
  | "Approved"
  | "Rejected"
  | "Cancelled";

export interface SwapAuditEntry {
  at: string;
  by: string;
  action: string;
  note: string | null;
}

export interface SwapRequest {
  id: number;
  requesterId: number;
  requesterName: string;
  requesterInitials: string;
  requesterDepartment: string;
  requesteeId: number;
  requesteeName: string;
  requesteeInitials: string;
  requesteeDepartment: string;
  requesterShiftId: number;
  requesterShiftDate: string;
  requesterShiftType: string;
  requesteeShiftId: number;
  requesteeShiftDate: string;
  requesteeShiftType: string;
  reason: string;
  status: SwapStatus;
  submittedAt: string;
  auditEntries: SwapAuditEntry[];
}

export const SWAP_STATUS_LABELS: Record<SwapStatus, string> = {
  PendingRequestee: "Awaiting Response",
  PendingAdmin: "Pending Approval",
  Approved: "Approved",
  Rejected: "Rejected",
  Cancelled: "Cancelled",
};
