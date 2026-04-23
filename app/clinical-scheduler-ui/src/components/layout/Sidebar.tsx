import clsx from "clsx";
import { LogOut } from "lucide-react";
import { NavLink, useNavigate } from "react-router-dom";

import { useAppDispatch, useAppSelector } from "../../app/hooks";
import { NAV_ITEMS, ROLE_LABELS } from "../../constants/navbar";
import { logout } from "../../features/auth/authSlice";
import type { StaffRole } from "../../types/auth";

export default function Sidebar() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const user = useAppSelector((s) => s.auth.user);

  if (!user) return null;

  const visibleNav = NAV_ITEMS.filter((n) => n.roles.includes(user.role));

  const handleLogout = () => {
    dispatch(logout());
    navigate("/login", { replace: true });
  };

  return (
    <aside className="w-56 shrink-0 flex flex-col no-print border-r border-r-slate-200 bg-white z-100">
      {/* Logo */}
      <div className="flex items-center gap-2.5 px-4 py-5 border-b border-b-slate-200">
        <div className="w-7 h-7 flex items-center justify-center">
          <img src="/assets/logo.png" alt="Logo" />
        </div>
        <div>
          <div className="text-sm font-bold text-slate-900 leading-tight">
            CareShift
          </div>
          <div className="text-xs text-slate-400">Clinical Staff Scheduler</div>
        </div>
      </div>
      {/* Nav */}
      <nav className="flex-1 overflow-y-auto p-2 space-y-0.5">
        {visibleNav.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                clsx(
                  "flex items-center gap-2.5 w-full rounded-lg px-2.5 py-2.5 text-sm transition-all no-underline",
                  isActive
                    ? "font-semibold bg-accent/10 text-accent"
                    : "text-slate-500 hover:bg-slate-50 hover:text-slate-700 font-normal",
                )
              }
            >
              <span className="text-base w-5 text-center shrink-0">
                <Icon size={20} />
              </span>
              {item.label}
            </NavLink>
          );
        })}
      </nav>
      {/* User info + logout */}
      <div className="p-4 border-t border-t-slate-200">
        <div className="flex items-center gap-2.5">
          <div className="w-8 h-8 rounded-lg flex items-center justify-center text-xs font-bold shrink-0 bg-accent/10 text-accent">
            {user.initials}
          </div>
          <div className="flex-1 min-w-0">
            <div className="text-xs font-semibold text-slate-900 truncate">
              {user.fullName}
            </div>
            <div className="text-xs text-slate-400 capitalize">
              {ROLE_LABELS[user.role as StaffRole]}
            </div>
          </div>
          <button
            onClick={handleLogout}
            title="Sign out"
            className="text-slate-400 hover:text-slate-600 transition-colors text-sm cursor-pointer p-1"
          >
            <LogOut size={18} />
          </button>
        </div>
      </div>
    </aside>
  );
}
