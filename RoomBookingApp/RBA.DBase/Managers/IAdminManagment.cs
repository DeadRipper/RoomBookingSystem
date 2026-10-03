using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers
{
    public interface IAdminManagment
    {
        Task<bool> LoginAsync(LoginRequest request);
        Task LogoutAsync(LogoutRequest request);
    }
}