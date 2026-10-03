using RBA.DBase.Managers;
using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
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
    }
}