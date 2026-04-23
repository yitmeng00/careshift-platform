import { Outlet, Navigate } from "react-router-dom";

import Sidebar from "./Sidebar";
import TopBar from "./TopBar";
import { useAppSelector } from "../../app/hooks";

export default function AppContainer() {
  const isAuthenticated = useAppSelector((s) => s.auth.isAuthenticated);

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return (
    <div className="flex min-h-screen">
      <Sidebar />
      <main className="flex-1 flex flex-col min-h-screen">
        <TopBar />
        <div className="flex-1">
          <Outlet />
        </div>
      </main>
    </div>
  );
}
