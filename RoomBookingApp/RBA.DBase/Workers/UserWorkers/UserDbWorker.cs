using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers.DbWorker;
using RBA.Models.Models.AdminModels;
using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.Admin.Cancel;
using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
using RBA.Models.Request.Admin.NewRoom;
using RBA.Models.Request.Admin.Reservations;
using RBA.Models.Request.RoomBooking.BookRoom;
using RBA.Models.Request.RoomBooking.CheckRoomAvailable;
using RBA.Models.Request.RoomBooking.UnbookRoom;
using RBA.Models.Request.User.GetUserById;
using RBA.Models.Request.User.GetUserId;
using RBA.Models.Request.User.Registration;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace RBA.DBase.Workers.UserWorkers
{
    public class UserDbWorker(AppDbContext appDbContext, ILogger<UserDbWorker> logger) : IUserDBWorker
    {
        public async Task<bool> AddUserAsync(RegistrationRequest request)
        {
            await appDbContext.Users.AddAsync(new UserModel
            {
                UserName = request.UserName,
                Password = request.Password,
                Email = request.Email
            });
            await appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUserId(GetUserIdRequest request)
        {
            var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            return user?.Id ?? 0;
        }

        public async Task<List<int>> GetAllUsersId()
        {
            return await appDbContext.Users.Select(u => u.Id).ToListAsync();
        }

        public async Task<UserModel> GetUserById(GetUserByIdRequest request)
        {
            return await appDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        }
    }
}