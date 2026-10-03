using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.User.GetUserById;
using RBA.Models.Request.User.GetUserId;
using RBA.Models.Request.User.Registration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers
{
    public interface IUserManagment
    {
        Task<UserModel> GetUserById(GetUserByIdRequest request);
        Task<int> GetUserId(GetUserIdRequest request);
        Task<List<int>> GetAllUsersId();
        Task<bool> RegistrateUserAsync(RegistrationRequest request);
    }
}