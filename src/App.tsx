import { Route, Routes } from "react-router-dom";
import Layout from "./components/Layout";
import RoomsPage from "./pages/RoomsPage";
import RoomDetailPage from "./pages/RoomDetailPage";
import MyBookingsPage from "./pages/MyBookingsPage";

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<RoomsPage />} />
        <Route path="rooms/:roomId" element={<RoomDetailPage />} />
        <Route path="bookings" element={<MyBookingsPage />} />
      </Route>
    </Routes>
  );
}
