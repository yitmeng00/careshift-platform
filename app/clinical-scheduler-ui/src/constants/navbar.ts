import {
  ArrowLeftRight,
  CalendarCheck,
  ClipboardClock,
  ClockPlus,
  LayoutDashboard,
  Logs,
  User,
} from "lucide-react";

import type { StaffRole } from "../types/auth";
import type { NavItem } from "../types/navbar";

export const ROLE_LABELS: Record<StaffRole, string> = {
  Admin: "Admin",
  DepartmentLead: "Dept Lead",
  ChargeNurse: "Charge Nurse",
  Doctor: "Doctor",
  Nurse: "Nurse",
  Receptionist: "Receptionist",
};

export const NAV_ITEMS: NavItem[] = [
  {
    to: "/dashboard",
    label: "Dashboard",
    icon: LayoutDashboard,
    roles: [
      "Admin",
      "DepartmentLead",
      "ChargeNurse",
      "Doctor",
      "Nurse",
      "Receptionist",
    ],
  },
  {
    to: "/schedule",
    label: "Schedule",
    icon: CalendarCheck,
    roles: [
      "Admin",
      "DepartmentLead",
      "ChargeNurse",
      "Doctor",
      "Nurse",
      "Receptionist",
    ],
  },
  {
    to: "/leaves",
    label: "Leave Requests",
    icon: ClipboardClock,
    roles: [
      "Admin",
      "DepartmentLead",
      "ChargeNurse",
      "Doctor",
      "Nurse",
      "Receptionist",
    ],
  },
  {
    to: "/swaps",
    label: "Shift Swaps",
    icon: ArrowLeftRight,
    roles: [
      "Admin",
      "DepartmentLead",
      "ChargeNurse",
      "Doctor",
      "Nurse",
      "Receptionist",
    ],
  },
  {
    to: "/overtime",
    label: "Overtime",
    icon: ClockPlus,
    roles: ["Admin", "DepartmentLead", "ChargeNurse"],
  },
  {
    to: "/staff",
    label: "Staff",
    icon: User,
    roles: ["Admin", "DepartmentLead"],
  },
  { to: "/audit", label: "Audit Log", icon: Logs, roles: ["Admin"] },
];
