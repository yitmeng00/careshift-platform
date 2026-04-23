import { createBrowserRouter, Navigate } from "react-router-dom";

import AppContainer from "../components/layout/AppContainer";
import ComingSoon from "../components/ui/ComingSoon";
import LoginPage from "../features/auth/LoginPage";
import DashboardPage from "../features/dashboard/DashboardPage";

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/",
    element: <AppContainer />,
    children: [
      { index: true, element: <Navigate to="/dashboard" replace /> },
      { path: "dashboard", element: <DashboardPage /> },
      {
        path: "schedule",
        element: <ComingSoon />,
      },
      {
        path: "leaves",
        element: <ComingSoon />,
      },
      {
        path: "swaps",
        element: <ComingSoon />,
      },
      {
        path: "overtime",
        element: <ComingSoon />,
      },
      {
        path: "staff",
        element: <ComingSoon />,
      },
      {
        path: "audit",
        element: <ComingSoon />,
      },
    ],
  },
  {
    path: "*",
    element: <Navigate to="/dashboard" replace />,
  },
]);
