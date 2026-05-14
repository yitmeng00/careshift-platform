import { api } from "./api";
import type { StaffSummary } from "../types/user";

export const staffApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getStaffList: builder.query<StaffSummary[], void>({
      query: () => "/staff",
      providesTags: ["Staff"],
    }),
  }),
});

export const { useGetStaffListQuery } = staffApi;
