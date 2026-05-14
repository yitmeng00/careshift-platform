export interface DashboardStats {
  onDutyToday: number;
  pendingLeaves: number;
  overtimeAlerts: number;
  activeStaff: number;
}

export interface TodayShift {
  id: number;
  staffName: string;
  staffInitials: string;
  departmentName: string;
  shiftType: "Morning" | "Afternoon" | "Night";
  startTime: string;
  endTime: string;
}

export interface PendingLeave {
  id: number;
  staffName: string;
  staffInitials: string;
  departmentName: string;
  leaveType: string;
  startDate: string;
  endDate: string;
  daysCount: number;
  reason: string;
  submittedAt: string;
}
