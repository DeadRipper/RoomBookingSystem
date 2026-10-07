using RBA.Models.Models.AuthModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers.Auth
{
    public interface IDbAuthManager
    {
        Task<AuthModel> GenerateToken();
        Task<AuthModel> RefreshToken(DateTime experationDate);
    }
}