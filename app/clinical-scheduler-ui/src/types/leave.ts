export type LeaveType =
  | "Annual"
  | "Sick"
  | "MaternityPaternity"
  | "Compassionate"
  | "Emergency"
  | "Unpaid";

export type LeaveStatus = "Pending" | "Approved" | "Rejected";

export interface LeaveAuditEntry {
  at: string;
  by: string;
  action: string;
  note: string | null;
}

export interface LeaveRequest {
  id: number;
  staffId: number;
  staffFullName: string;
  staffInitials: string;
  staffDepartment: string;
  leaveType: LeaveType;
  startDate: string;
  endDate: string;
  durationDays: number;
  reason: string;
  status: LeaveStatus;
  reviewNote: string | null;
  reviewedBy: string | null;
  reviewedAt: string | null;
  submittedAt: string;
  auditEntries: LeaveAuditEntry[];
}

export interface ApprovedLeave {
  staffId: number;
  startDate: string;
  endDate: string;
}

export const LEAVE_TYPE_LABELS: Record<LeaveType, string> = {
  Annual: "Annual Leave",
  Sick: "Sick Leave",
  MaternityPaternity: "Maternity / Paternity",
  Compassionate: "Compassionate Leave",
  Emergency: "Emergency Leave",
  Unpaid: "Unpaid Leave",
};

export const LEAVE_TYPES: LeaveType[] = [
  "Annual",
  "Sick",
  "MaternityPaternity",
  "Compassionate",
  "Emergency",
  "Unpaid",
];
