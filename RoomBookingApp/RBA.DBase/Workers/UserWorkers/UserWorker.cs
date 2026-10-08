using RBA.DBase.Managers.DbWorker;
using RBA.DBase.Managers.User;
using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.User.GetUserById;
using RBA.Models.Request.User.GetUserId;
using RBA.Models.Request.User.Registration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Workers.UserWorkers
{
    public class UserWorker(IUserDBWorker dBWorker) : IUserManager
    {
        public async Task<bool> RegistrateUserAsync(RegistrationRequest request)
        {
            await dBWorker.AddUserAsync(request);
            return true;
        }

        public async Task<int> GetUserId(GetUserIdRequest request)
        {
            var userId = await dBWorker.GetUserId(request);
            return userId;
        }

        public async Task<List<int>> GetAllUsersId()
        {
            var userIds = await dBWorker.GetAllUsersId();
            return userIds;
        }

        public Task<UserModel> GetUserById(GetUserByIdRequest request)
        {
            return dBWorker.GetUserById(request);
        }
    }
}