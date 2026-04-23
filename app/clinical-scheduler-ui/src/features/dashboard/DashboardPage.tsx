import {
  AlertTriangle,
  CalendarCheck,
  ClipboardClock,
  Users,
} from "lucide-react";
import type { LucideIcon } from "lucide-react";

import ActivityFeed from "./components/ActivityFeed";
import StatCard from "./components/StatCard";
import type { StatCardVariant } from "./components/StatCard";
import {
  useGetDashboardStatsQuery,
  useGetRecentActivityQuery,
} from "./dashboardApi";
import { useAppSelector } from "../../app/hooks";
import type { DashboardStats } from "../../types/dashboard";
import { formatDisplayDate, getGreeting } from "../../utils/dateUtils";

interface StatCardConfig {
  key: keyof DashboardStats;
  label: string;
  sub: string;
  variant: StatCardVariant;
  icon: LucideIcon;
}

const STAT_CARD_CONFIGS: StatCardConfig[] = [
  {
    key: "onDutyToday",
    label: "On Duty Today",
    sub: "shifts scheduled",
    variant: "blue",
    icon: CalendarCheck,
  },
  {
    key: "pendingLeaves",
    label: "Pending Leave",
    sub: "awaiting review",
    variant: "amber",
    icon: ClipboardClock,
  },
  {
    key: "overtimeAlerts",
    label: "Overtime Alerts",
    sub: "over 40 hrs this week",
    variant: "red",
    icon: AlertTriangle,
  },
  {
    key: "activeStaff",
    label: "Active Staff",
    sub: "across departments",
    variant: "green",
    icon: Users,
  },
];

export default function DashboardPage() {
  const user = useAppSelector((state) => state.auth.user);
  const { data: stats, isLoading: statsLoading } = useGetDashboardStatsQuery();
  const { data: activity, isLoading: activityLoading } =
    useGetRecentActivityQuery();

  return (
    <div className="p-4 lg:p-9 max-w-6xl mx-auto">
      {/* Header */}
      <div className="mb-7">
        <h1 className="text-2xl font-bold text-slate-900 tracking-tight">
          {getGreeting()}, {user?.fullName?.split(" ")[0]}!
        </h1>
        <p className="text-sm text-slate-500 mt-1">{formatDisplayDate()}</p>
      </div>

      {/* Stat cards */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
        {STAT_CARD_CONFIGS.map((config) => (
          <StatCard
            key={config.key}
            label={config.label}
            sub={config.sub}
            variant={config.variant}
            icon={config.icon}
            value={stats?.[config.key] ?? "—"}
            isLoading={statsLoading}
          />
        ))}
      </div>

      {/* Activity feed */}
      <ActivityFeed entries={activity} isLoading={activityLoading} />
    </div>
  );
}
