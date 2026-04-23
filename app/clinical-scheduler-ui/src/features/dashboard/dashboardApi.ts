import { api } from "../../services/api";
import type { AuditLogEntry } from "../../types/audit";
import type { DashboardStats } from "../../types/dashboard";

export const dashboardApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getDashboardStats: builder.query<DashboardStats, void>({
      query: () => "/dashboard/stats",
    }),
    getRecentActivity: builder.query<AuditLogEntry[], void>({
      query: () => "/dashboard/activity",
      providesTags: ["AuditLog"],
    }),
  }),
});

export const { useGetDashboardStatsQuery, useGetRecentActivityQuery } =
  dashboardApi;
