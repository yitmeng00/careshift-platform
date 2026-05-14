export type LeaveStatus = "Pending" | "Approved" | "Rejected";
export type LeaveType = "Annual" | "Sick" | "Training" | "Personal";

export interface LeaveAuditEntry {
  at: string;
  by: string;
  action: string;
  note?: string;
}

export interface LeaveRequest {
  id: number;
  staffId: number;
  staffName: string;
  staffInitials: string;
  staffColor: string;
  leaveType: LeaveType;
  startDate: string;
  endDate: string;
  reason: string;
  status: LeaveStatus;
  reviewedBy?: string;
  reviewedAt?: string;
  reviewNote?: string;
  submittedAt: string;
  auditEntries: LeaveAuditEntry[];
}
