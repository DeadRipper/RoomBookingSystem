using RBA.DBase.Managers;
using RBA.Models.Request.Admin.Login;
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
    }
}