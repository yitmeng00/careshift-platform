export type ShiftType = "Morning" | "Afternoon" | "Night";
export type SwapStatus =
  | "PendingRequestee"
  | "PendingAdmin"
  | "Approved"
  | "Rejected"
  | "Cancelled";

export interface Shift {
  id: number;
  staffId: number;
  departmentId: number;
  staffName: string;
  staffInitials: string;
  departmentName: string;
  startTime: string;
  endTime: string;
  shiftType: ShiftType;
  notes?: string;
}

export interface SwapAuditEntry {
  at: string;
  by: string;
  action: string;
  note?: string;
}

export interface ShiftSwapRequest {
  id: number;
  requesterId: number;
  requesterName: string;
  requesterInitials: string;
  requesteeId: number;
  requesteeName: string;
  requesteeInitials: string;
  requesterShift: Shift;
  requesteeShift: Shift;
  reason: string;
  status: SwapStatus;
  submittedAt: string;
  auditEntries: SwapAuditEntry[];
}
