import { z } from "zod";

import type { loginSchema } from "../schemas/auth";

export type LoginFormValues = z.infer<typeof loginSchema>;

export type StaffRole =
  | "Admin"
  | "DepartmentLead"
  | "ChargeNurse"
  | "Doctor"
  | "Nurse"
  | "Receptionist";

export interface AuthUser {
  id: number;
  fullName: string;
  email: string;
  role: StaffRole;
  department: string;
  initials: string;
}
