using RBA.Models.Request.Admin.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers
{
    public interface IAdminManagment
    {
        Task<bool> LoginAsync(LoginRequest request);
    }
}