import type { StaffRole } from "./auth";

export interface NavItem {
  to: string;
  label: string;
  icon: React.ElementType;
  roles: StaffRole[];
}
