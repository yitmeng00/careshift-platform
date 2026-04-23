import clsx from "clsx";
import type { LucideIcon } from "lucide-react";

export type StatCardVariant = "blue" | "amber" | "red" | "green";

interface StatCardProps {
  label: string;
  value: number | string;
  sub: string;
  variant: StatCardVariant;
  icon: LucideIcon;
  isLoading?: boolean;
}

const VARIANT_STYLES: Record<
  StatCardVariant,
  { value: string; iconBg: string; iconColor: string }
> = {
  blue: { value: "text-accent", iconBg: "bg-accent/10", iconColor: "text-accent" },
  amber: { value: "text-amber-500", iconBg: "bg-amber-50", iconColor: "text-amber-500" },
  red: { value: "text-red-500", iconBg: "bg-red-50", iconColor: "text-red-500" },
  green: { value: "text-emerald-500", iconBg: "bg-emerald-50", iconColor: "text-emerald-500" },
};

export default function StatCard({
  label,
  value,
  sub,
  variant,
  icon: Icon,
  isLoading = false,
}: StatCardProps) {
  const styles = VARIANT_STYLES[variant];

  return (
    <div className="rounded-xl p-5 bg-white border border-slate-200">
      <div className="flex items-start justify-between mb-3">
        <div className={clsx("w-9 h-9 rounded-lg flex items-center justify-center", styles.iconBg)}>
          <Icon size={18} className={styles.iconColor} />
        </div>
      </div>
      <div className={clsx("text-2xl font-bold mb-1", styles.value)}>
        {isLoading ? "—" : value}
      </div>
      <div className="text-sm font-medium text-slate-700">{label}</div>
      <div className="text-xs text-slate-400 mt-0.5">{sub}</div>
    </div>
  );
}
