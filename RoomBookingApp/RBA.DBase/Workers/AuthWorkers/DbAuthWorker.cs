using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers.Auth;
using RBA.Models.Models.AuthModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace RBA.DBase.Workers.AuthWorkers
{
    public class DbAuthWorker(AppDbContext appDbContext, ILogger<DbAuthWorker> logger, IConfiguration configuration) : IDbAuthManager
    {
        public async Task<AuthModel> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
              configuration["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            var authResp = new AuthModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExparationDate = token.ValidTo
            };

            appDbContext.Auths.Add(authResp);
            await appDbContext.SaveChangesAsync();
            return authResp;
        }

        public async Task<AuthModel> RefreshToken(DateTime experationDate)
        {
            throw new NotImplementedException();
        }
    }
}