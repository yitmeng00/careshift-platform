import clsx from "clsx";

import { useAppSelector } from "../../app/hooks";

const GREETINGS = ["Good morning", "Good afternoon", "Good evening"];
const STAT_CARDS = [
  {
    label: "On Duty Today",
    value: "—",
    sub: "shifts scheduled",
    color: "text-blue-600",
  },
  {
    label: "Pending Leave",
    value: "—",
    sub: "awaiting review",
    color: "text-amber-500",
  },
  {
    label: "Overtime Alerts",
    value: "—",
    sub: "over 40 hrs this week",
    color: "text-red-500",
  },
  {
    label: "Active Staff",
    value: "—",
    sub: "across departments",
    color: "text-emerald-500",
  },
];

function getGreeting(): string {
  const h = new Date().getHours();
  if (h < 12) return GREETINGS[0];
  if (h < 17) return GREETINGS[1];
  return GREETINGS[2];
}

export default function DashboardPage() {
  const user = useAppSelector((s) => s.auth.user);

  return (
    <div className="p-9 max-w-6xl mx-auto">
      <div className="mb-7">
        <h1 className="text-2xl font-bold text-slate-900 tracking-tight">
          {getGreeting()}!
        </h1>
        <p className="text-sm text-slate-500 mt-1">
          {new Date().toLocaleDateString("en-US", {
            weekday: "long",
            year: "numeric",
            month: "long",
            day: "numeric",
          })}
        </p>
      </div>

      {/* Stat cards — placeholder */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
        {STAT_CARDS.map((card) => (
          <div
            key={card.label}
            className="rounded-xl p-5 bg-white border border-slate-200"
          >
            <div className={clsx("text-2xl font-bold mb-1", card.color)}>
              {card.value}
            </div>
            <div className="text-sm font-medium text-slate-700">
              {card.label}
            </div>
            <div className="text-xs text-slate-400 mt-0.5">{card.sub}</div>
          </div>
        ))}
      </div>
      {/* Info banner */}
      <div className="rounded-xl p-6 text-center bg-white border border-slate-200">
        <p className="text-sm text-slate-500">
          Welcome back,{" "}
          <span className="font-semibold text-slate-700">{user?.fullName}</span>
          . Dashboard data will load once the API is connected.
        </p>
      </div>
    </div>
  );
}
