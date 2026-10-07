using Microsoft.Extensions.Logging;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers.Auth;
using RBA.Models.Models.AuthModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Workers.AuthWorkers
{
    public class AuthWorker(AppDbContext appDbContext, ILogger<AuthWorker> logger, IDbAuthManager dbAuthManager) : IAuthManager
    {
        public async Task<AuthModel> GenerateToken()
        {
            return await dbAuthManager.GenerateToken();
        }

        public async Task<AuthModel> RefreshToken(DateTime experationDate)
        {
            return await dbAuthManager.RefreshToken(experationDate);
        }
    }
}