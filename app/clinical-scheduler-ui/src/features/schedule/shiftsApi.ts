import { api } from "../../services/api";
import type { Shift } from "../../types/shift";

export interface CreateShiftRequest {
  staffId: number;
  departmentId: number;
  date: string;
  shiftType: string;
  notes?: string;
}

export const shiftsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getShiftsByWeek: builder.query<
      Shift[],
      { weekStart: string; departmentId?: number }
    >({
      query: ({ weekStart, departmentId }) => ({
        url: "/shifts",
        params: {
          weekStart,
          ...(departmentId !== undefined ? { departmentId } : {}),
        },
      }),
      providesTags: ["Shift"],
    }),
    getShiftsByMonth: builder.query<
      Shift[],
      { monthStart: string; departmentId?: number }
    >({
      query: ({ monthStart, departmentId }) => ({
        url: "/shifts",
        params: {
          monthStart,
          ...(departmentId !== undefined ? { departmentId } : {}),
        },
      }),
      providesTags: ["Shift"],
    }),
    createShift: builder.mutation<Shift, CreateShiftRequest>({
      query: (body) => ({ url: "/shifts", method: "POST", body }),
      invalidatesTags: ["Shift"],
    }),
    updateShift: builder.mutation<Shift, { id: number; newDate: string }>({
      query: ({ id, newDate }) => ({
        url: `/shifts/${id}`,
        method: "PATCH",
        body: { newDate },
      }),
      invalidatesTags: ["Shift"],
    }),
    deleteShift: builder.mutation<void, number>({
      query: (id) => ({ url: `/shifts/${id}`, method: "DELETE" }),
      invalidatesTags: ["Shift"],
    }),

    getUpcomingShifts: builder.query<Shift[], number>({
      query: (staffId) => ({ url: "/shifts/upcoming", params: { staffId } }),
      providesTags: ["Shift"],
    }),
  }),
});

export const {
  useGetShiftsByWeekQuery,
  useGetShiftsByMonthQuery,
  useCreateShiftMutation,
  useUpdateShiftMutation,
  useDeleteShiftMutation,
  useGetUpcomingShiftsQuery,
} = shiftsApi;
