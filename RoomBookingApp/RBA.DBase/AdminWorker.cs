using RBA.DBase.Managers;
using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
using RBA.Models.Request.Admin.NewRoom;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase
{
    public class AdminWorker(IDBWorker dbWorker) : IAdminManagment
    {
        public Task<bool> LoginAsync(LoginRequest request)
        {
            return dbWorker.LoginAsync(request);
        }

        public Task<bool> LogoutAsync(LogoutRequest request)
        {
            return dbWorker.LogoutAsync(request);
        }

        public Task<RoomModel> InsertNewRoom(NewRoomRequest roomRequest)
        {
            return dbWorker.InsertNewRoom(roomRequest);
        }

        public Task<List<AmenityModel>> GetRoomConfigs()
        {
            return dbWorker.GetRoomConfigs();
        }

        public Task<int> GetTotalBookings()
        {
            return dbWorker.GetTotalBookings();
        }

        public Task<int> GetTodayBookings()
        {
            return dbWorker.GetTodayBookings();
        }

        public Task<int> GetAllRoomsCount()
        {
            return dbWorker.GetAllRoomsCount();
        }

        public Task<List<ReservationModel>> GetAllReservations()
        {
            return dbWorker.GetAllReservations();
        }
    }
}