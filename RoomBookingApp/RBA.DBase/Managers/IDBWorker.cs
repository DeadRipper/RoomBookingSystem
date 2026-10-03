using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
using RBA.Models.Request.Admin.NewRoom;
using RBA.Models.Request.Admin.Reservations;
using RBA.Models.Request.RoomBooking.BookRoom;
using RBA.Models.Request.RoomBooking.CheckRoomAvailable;
using RBA.Models.Request.RoomBooking.UnbookRoom;
using RBA.Models.Request.User.GetUserId;
using RBA.Models.Request.User.Registration;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers
{
    public interface IDBWorker
    {
        //----------------------- Room Booking -----------------------
        Task<RoomState> GetRoomAvailabilityState(CheckRoomAvailableRequest checkRoomAvailableRequest);
        Task<BookState> BookingRoom(BookRoomRequest bookRoomRequest);
        Task<BookState> UnbookingRoom(UnbookRoomRequest unbookRoomRequest);
        Task<IEnumerable<RoomModel>> GetAllRooms();

        //----------------------- Admin Management -----------------------
        Task<bool> LoginAsync(LoginRequest request);
        Task<bool> LogoutAsync(LogoutRequest request);
        Task<RoomModel> InsertNewRoom(NewRoomRequest request);
        Task<List<AmenityModel>> GetRoomConfigs();
        Task<int> GetTotalBookings();
        Task<int> GetAllRoomsCount();
        Task<int> GetTodayBookings();
        Task<List<ReservationDTO>> GetAllReservations();
        //----------------------- User Management -----------------------
        Task<int> GetUserId(GetUserIdRequest request);
        Task<List<int>> GetAllUsersId();
        Task<bool> AddUserAsync(RegistrationRequest request);
    }
}