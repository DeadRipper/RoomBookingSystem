import { Route, Routes } from "react-router-dom";
import Layout from "./components/Layout";
import RoomsPage from "./pages/RoomsPage";
import RoomDetailPage from "./pages/RoomDetailPage";
import MyBookingsPage from "./pages/MyBookingsPage";
import AdminLoginPage from "./pages/AdminLoginPage";
import RegisterPage from "./pages/RegisterPage";
import AdminDashboardPage from "./pages/AdminDashboardPage";
import RequireAdmin from "./components/RequireAdmin";

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<RoomsPage />} />
        <Route path="rooms/:roomId" element={<RoomDetailPage />} />
        <Route path="bookings" element={<MyBookingsPage />} />
        <Route path="register" element={<RegisterPage />} />
        <Route path="admin/login" element={<AdminLoginPage />} />
        <Route path="admin" element={<RequireAdmin />}>
          <Route index element={<AdminDashboardPage />} />
        </Route>
      </Route>
    </Routes>
  );
}
