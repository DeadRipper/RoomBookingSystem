import { Navigate, Outlet } from "react-router-dom";
import { useAdmin } from "../store/AdminContext";

export default function RequireAdmin() {
  const { admin } = useAdmin();
  return admin ? <Outlet /> : <Navigate to="/admin/login" replace />;
}
