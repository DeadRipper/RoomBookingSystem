using Microsoft.Extensions.Logging;
using RBA.DBase.Managers.DbWorker;
using RBA.DBase.Managers.User;
using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.User.GetUserById;
using RBA.Models.Request.User.GetUserId;
using RBA.Models.Request.User.Registration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Workers.UserWorkers
{
    public class UserWorker(IUserDBWorker dBWorker, ILogger<UserWorker> logger) : IUserManager
    {
        public async Task<bool> RegistrateUserAsync(RegistrationRequest request)
        {
            logger.LogInformation("UserWorker: RegistrateUserAsync called with UserName: {UserName}", request.UserName);
            var result = await dBWorker.AddUserAsync(request);
            logger.LogInformation("UserWorker: RegistrateUserAsync completed with result: {Result}", result);
            return result;
        }

        public async Task<int> GetUserId(GetUserIdRequest request)
        {
            logger.LogInformation("UserWorker: GetUserId called with UserName: {UserName}", request.UserName);
            var userId = await dBWorker.GetUserId(request);
            logger.LogInformation("UserWorker: GetUserId completed with result: {Result}", userId);
            return userId;
        }

        public async Task<List<int>> GetAllUsersId()
        {
            logger.LogInformation("UserWorker: GetAllUsersId called");
            var userIds = await dBWorker.GetAllUsersId();
            logger.LogInformation("UserWorker: GetAllUsersId completed with result: {Result}", userIds);
            return userIds;
        }

        public Task<UserModel> GetUserById(GetUserByIdRequest request)
        {
            logger.LogInformation("UserWorker: GetUserById called with UserId: {UserId}", request.UserId);
            var user = dBWorker.GetUserById(request);
            logger.LogInformation("UserWorker: GetUserById completed with result: {Result}", user);
            return user;
        }
    }
}