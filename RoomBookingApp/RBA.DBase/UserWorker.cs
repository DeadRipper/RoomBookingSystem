using RBA.DBase.Managers;
using RBA.Models.Request.User.GetUserId;
using RBA.Models.Request.User.Registration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase
{
    public class UserWorker(IDBWorker dBWorker) : IUserManagment
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
    }
}