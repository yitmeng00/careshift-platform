import type { StaffRole } from "./auth";

export type EmploymentType = "FullTime" | "PartTime" | "Contract";

export interface StaffMember {
  id: number;
  fullName: string;
  email: string;
  role: StaffRole;
  departmentId: number;
  departmentName: string;
  isActive: boolean;
  phone?: string;
  initials: string;
  employmentType: EmploymentType;
  createdAt: string;
}

export interface Department {
  id: number;
  name: string;
  description?: string;
}

export interface StaffSummary {
  id: number;
  fullName: string;
  initials: string;
  departmentId: number;
  departmentName: string;
}
