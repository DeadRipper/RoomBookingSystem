using RBA.Models.Request.User.Registration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers
{
    public interface IUserManagment
    {
        Task<bool> RegistrateUserAsync(RegistrationRequest request);
    }
}