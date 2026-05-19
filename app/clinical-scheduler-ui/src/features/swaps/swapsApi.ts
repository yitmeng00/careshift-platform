import { api } from "../../services/api";
import type { SwapRequest } from "../../types/swap";

interface SubmitSwapRequest {
  requesterShiftId: number;
  requesteeShiftId: number;
  reason: string;
}

interface RespondSwapRequest {
  action: "Accept" | "Reject";
  note?: string;
}

interface ReviewSwapRequest {
  action: "Approve" | "Reject";
  note?: string;
}

export const swapsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getSwapRequests: builder.query<SwapRequest[], void>({
      query: () => "/swaps",
      providesTags: ["Swap"],
    }),

    submitSwap: builder.mutation<SwapRequest, SubmitSwapRequest>({
      query: (body) => ({ url: "/swaps", method: "POST", body }),
      invalidatesTags: ["Swap"],
    }),

    respondSwap: builder.mutation<
      SwapRequest,
      { id: number } & RespondSwapRequest
    >({
      query: ({ id, ...body }) => ({
        url: `/swaps/${id}/respond`,
        method: "PUT",
        body,
      }),
      invalidatesTags: ["Swap"],
    }),

    reviewSwap: builder.mutation<
      SwapRequest,
      { id: number } & ReviewSwapRequest
    >({
      query: ({ id, ...body }) => ({
        url: `/swaps/${id}/review`,
        method: "PUT",
        body,
      }),
      invalidatesTags: ["Swap"],
    }),

    cancelSwap: builder.mutation<void, number>({
      query: (id) => ({ url: `/swaps/${id}`, method: "DELETE" }),
      invalidatesTags: ["Swap"],
    }),
  }),
});

export const {
  useGetSwapRequestsQuery,
  useSubmitSwapMutation,
  useRespondSwapMutation,
  useReviewSwapMutation,
  useCancelSwapMutation,
} = swapsApi;

export const selectSwapPendingCount = (swaps: SwapRequest[]) =>
  swaps.filter(
    (s) => s.status === "PendingRequestee" || s.status === "PendingAdmin",
  ).length;
