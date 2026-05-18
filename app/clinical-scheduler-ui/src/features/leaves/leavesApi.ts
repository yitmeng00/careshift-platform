import { api } from "../../services/api";
import type {
  ApprovedLeave,
  LeaveRequest,
  LeaveStatus,
  LeaveType,
} from "../../types/leave";

interface SubmitLeaveRequest {
  leaveType: LeaveType;
  startDate: string;
  endDate: string;
  reason: string;
}

interface ReviewLeaveRequest {
  action: "Approve" | "Reject";
  note?: string;
}

export const leavesApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getLeaveRequests: builder.query<LeaveRequest[], void>({
      query: () => "/leaves",
      providesTags: ["Leave"],
    }),

    getApprovedLeaves: builder.query<
      ApprovedLeave[],
      { from: string; to: string }
    >({
      query: ({ from, to }) => ({
        url: "/leaves/approved",
        params: { from, to },
      }),
      providesTags: ["Leave"],
    }),

    submitLeave: builder.mutation<LeaveRequest, SubmitLeaveRequest>({
      query: (body) => ({ url: "/leaves", method: "POST", body }),
      invalidatesTags: ["Leave"],
    }),

    reviewLeave: builder.mutation<
      LeaveRequest,
      { id: number } & ReviewLeaveRequest
    >({
      query: ({ id, ...body }) => ({
        url: `/leaves/${id}/review`,
        method: "PUT",
        body,
      }),
      invalidatesTags: ["Leave"],
    }),

    cancelLeave: builder.mutation<void, number>({
      query: (id) => ({ url: `/leaves/${id}`, method: "DELETE" }),
      invalidatesTags: ["Leave"],
    }),
  }),
});

export const {
  useGetLeaveRequestsQuery,
  useGetApprovedLeavesQuery,
  useSubmitLeaveMutation,
  useReviewLeaveMutation,
  useCancelLeaveMutation,
} = leavesApi;

export const selectPendingCount = (leaves: LeaveRequest[]) =>
  leaves.filter((l) => l.status === "Pending").length;

export const filterByStatus = (
  leaves: LeaveRequest[],
  status: LeaveStatus | "All",
) => (status === "All" ? leaves : leaves.filter((l) => l.status === status));
