import { zodResolver } from "@hookform/resolvers/zod";
import clsx from "clsx";
import { CircleCheckBig } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";

import { useLoginMutation } from "./authApi";
import { setCredentials } from "./authSlice";
import { useAppDispatch } from "../../app/hooks";
import FormField from "../../components/ui/FormField";
import { loginSchema } from "../../schemas/auth";
import type { LoginFormValues } from "../../types/auth";

const FEATURES: string[] = [
  "Role-based access control",
  "Drag-and-drop shift scheduling",
  "Leave request workflow",
  "Real-time conflict detection",
  "Overtime tracking & alerts",
];

export default function LoginPage() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [login, { isLoading }] = useLoginMutation();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({ resolver: zodResolver(loginSchema) });

  const onSubmit = async (values: LoginFormValues) => {
    setServerError(null);
    try {
      const result = await login(values).unwrap();
      dispatch(
        setCredentials({
          token: result.accessToken,
          user: result.staff,
        }),
      );
      navigate("/dashboard", { replace: true });
    } catch (err: unknown) {
      const error = err as { data?: { title?: string }; status?: number };
      setServerError(error?.data?.title ?? "Login failed. Please try again.");
    }
  };

  const currentYear = new Date().getFullYear();
  const inputClass = (hasError: boolean) =>
    `w-full rounded-lg px-3.5 py-2.5 text-sm text-slate-900 outline-none transition-shadow duration-150 form-input ${hasError ? "form-input-error" : ""}`;

  return (
    <div className="min-h-screen flex">
      {/* Left panel */}
      <div className="hidden lg:flex flex-col justify-between shrink-0 p-10 w-[320px] bg-slate-900/95">
        <div>
          {/* Logo */}
          <div className="flex items-center gap-3 mb-12">
            <div className="w-8 h-8 flex items-center justify-center">
              <img src="/assets/logo.png" alt="Logo" />
            </div>
            <div>
              <div className="text-white font-bold text-lg leading-tight">
                CareShift
              </div>
              <div className="text-xs text-slate-400">
                Clinical Staff Scheduler
              </div>
            </div>
          </div>
          {/* Title */}
          <h1 className="text-white text-3xl font-bold leading-snug mb-4">
            Smarter scheduling
            <br />
            for clinical teams
          </h1>
          <p className="text-sm leading-relaxed text-slate-400">
            Coordinate shifts, manage leave requests, track overtime, and keep
            your department running smoothly — all in one place.
          </p>
          {/* Feature list */}
          <ul className="mt-8 space-y-3 list-none">
            {FEATURES.map((feature) => (
              <li
                key={feature}
                className="flex items-center gap-3 text-sm text-slate-300"
              >
                <span className="rounded-full flex items-center justify-center shrink-0 font-semibold text-blue-400">
                  <CircleCheckBig size={16} />
                </span>
                {feature}
              </li>
            ))}
          </ul>
        </div>
        {/* Copyright */}
        <p className="text-xs text-slate-500">
          © {currentYear} CareShift - Clinical Staff Scheduling Platform
        </p>
      </div>

      {/* Right panel */}
      <div className="flex-1 flex items-center justify-center p-8">
        <div className="w-full max-w-md">
          {/* Mobile logo */}
          <div className="flex items-center gap-3 mb-8 lg:hidden">
            <div className="w-8 h-8 flex items-center justify-center">
              <img src="/assets/logo.png" alt="Logo" />
            </div>
            <div>
              <div className="font-bold text-slate-900 leading-tight">
                CareShift
              </div>
              <div className="text-xs text-slate-400">
                Clinical Staff Scheduler
              </div>
            </div>
          </div>
          <div className="bg-white rounded-2xl p-8 border border-slate-200 shadow-[0_4px_24px_rgba(0,0,0,0.06)]">
            <div className="mb-8">
              <h2 className="text-2xl font-bold text-slate-900 mb-1">
                Sign in
              </h2>
              <p className="text-sm text-slate-500">
                Enter your credentials to access the scheduler.
              </p>
            </div>
            <form
              onSubmit={handleSubmit(onSubmit)}
              noValidate
              className="space-y-5"
            >
              {/* Server error */}
              {serverError && (
                <div className="rounded-lg px-4 py-3 text-sm text-error border border-error-border bg-error-bg">
                  {serverError}
                </div>
              )}
              <FormField label="Email address" error={errors.email?.message}>
                <input
                  type="email"
                  autoComplete="email"
                  placeholder="you@hospital.org"
                  {...register("email")}
                  className={inputClass(!!errors.email)}
                />
              </FormField>
              <FormField label="Password" error={errors.password?.message}>
                <input
                  type="password"
                  autoComplete="current-password"
                  placeholder="••••••••"
                  {...register("password")}
                  className={inputClass(!!errors.password)}
                />
              </FormField>
              <button
                type="submit"
                disabled={isLoading}
                className={clsx(
                  "w-full py-2.5 rounded-lg text-sm text-white font-semibold bg-accent transition-opacity",
                  {
                    "opacity-65 cursor-not-allowed": isLoading,
                    "opacity-100 cursor-pointer": !isLoading,
                  },
                )}
              >
                {isLoading ? "Signing in…" : "Sign in"}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
