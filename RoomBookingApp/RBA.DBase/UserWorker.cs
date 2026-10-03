using RBA.DBase.Managers;
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
    }
}