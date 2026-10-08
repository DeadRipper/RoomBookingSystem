using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.Admin.Cancel;
using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
using RBA.Models.Request.Admin.NewRoom;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers.Admin
{
    public interface IAdminDbManager
    {
        Task<bool> LoginAsync(LoginRequest request);
        Task<bool> LogoutAsync(LogoutRequest request);
        Task<RoomModel> InsertNewRoom(NewRoomRequest request);
        Task<List<AmenityModel>> GetRoomConfigs();
        Task<int> GetTotalBookings();
        Task<int> GetAllRoomsCount();
        Task<int> GetTodayBookings();
        Task<List<ReservationModel>> GetAllReservations();
        Task<bool> CancelBookingAsync(CancelBookingRequest request);
    }
}