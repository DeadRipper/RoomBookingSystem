using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.Admin.Cancel;
using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
using RBA.Models.Request.Admin.NewRoom;
using RBA.Models.Request.Admin.Reservations;
using RBA.Models.Request.RoomBooking.BookRoom;
using RBA.Models.Request.RoomBooking.CheckRoomAvailable;
using RBA.Models.Request.RoomBooking.UnbookRoom;
using RBA.Models.Request.User.GetUserById;
using RBA.Models.Request.User.GetUserId;
using RBA.Models.Request.User.Registration;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers.DbWorker
{
    public interface IUserDBWorker
    {
        Task<int> GetUserId(GetUserIdRequest request);
        Task<UserModel> GetUserById(GetUserByIdRequest request);
        Task<List<int>> GetAllUsersId();
        Task<bool> AddUserAsync(RegistrationRequest request);
    }
}