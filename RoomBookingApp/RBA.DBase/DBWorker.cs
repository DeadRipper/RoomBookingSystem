using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers;
using RBA.Models.Models.AdminModels;
using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
using RBA.Models.Request.Admin.NewRoom;
using RBA.Models.Request.RoomBooking.BookRoom;
using RBA.Models.Request.RoomBooking.CheckRoomAvailable;
using RBA.Models.Request.RoomBooking.UnbookRoom;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace RBA.DBase
{
    public class DBWorker(AppDbContext appDbContext, ILogger<DBWorker> logger) : IDBWorker
    {
        public async Task<RoomState> GetRoomAvailabilityState(CheckRoomAvailableRequest checkRoomAvailableRequest)
        {
            try
            {
                var a = appDbContext.Rooms?.
                    Where(x => 
                    x.Id == checkRoomAvailableRequest.RoomId)?.
                    Select(xx => 
                    xx.RoomState)?.FirstOrDefault() ?? RoomState.Occupied;
                return RoomState.Available;
            }
            catch
            {
                logger.LogError($"Error while getting room state for Room ID: {checkRoomAvailableRequest.RoomId}");
                throw;
            }
        }

        public async Task<BookState> BookingRoom(BookRoomRequest bookRoomRequest)
        {
            try
            {
                var roomSearch = appDbContext.Rooms.Where(x => x.Id == bookRoomRequest.RoomId);
                if (roomSearch != null && roomSearch.FirstOrDefault() == null)
                    return BookState.Failed;
            }
            catch
            {
                logger.LogError($"Error while booking room for Room ID: {bookRoomRequest.RoomId}");
                throw;
            }

            var room = appDbContext.Rooms.FirstOrDefault(x => x.Id == bookRoomRequest.RoomId);
            if (room != null)
                room.RoomState = RoomState.Occupied;

            var reservation = new ReservationModel
            {
                RoomId = bookRoomRequest.RoomId,
                Date = bookRoomRequest.BookingDate,
            };
            appDbContext.Reservations.Add(reservation);
            appDbContext.Entry(reservation).Property("UsersId").CurrentValue = bookRoomRequest.UserId;

            await appDbContext.SaveChangesAsync();
            return BookState.Confirmed;
        }

        public async Task<BookState> UnbookingRoom(UnbookRoomRequest unbookRoomRequest)
        {
            try
            {
                var roomSearch = appDbContext.Rooms.Where(x => x.Id == unbookRoomRequest.RoomId);
                if (roomSearch != null && roomSearch.FirstOrDefault() == null)
                    return BookState.Failed;

            }
            catch
            {
                logger.LogError($"Error while unbooking room for Room ID: {unbookRoomRequest.RoomId}");
                throw;
            }
            var room = appDbContext.Rooms.FirstOrDefault(x => x.Id == unbookRoomRequest.RoomId);
            if (room != null)
                room.RoomState = RoomState.Available;

            await appDbContext.SaveChangesAsync();
            return BookState.Cancelled;
        }

        public async Task<IEnumerable<RoomModel>> GetAllRooms()
        {
            return await appDbContext.Rooms.Where(x => x.Id != 0).ToListAsync();
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            var isUserExists = await appDbContext.AdminInfo.AnyAsync(x => x.Username == request.UserName);
            if (!isUserExists)
            {
                logger.LogWarning($"Login attempt failed for Admin Username: {request.UserName}");
                return false;
            }

            var passCheck = await appDbContext.AdminInfo.Where(x => x.Username == request.UserName && x.Password == request.Password).FirstOrDefaultAsync();

            if (passCheck == null)
            {
                logger.LogWarning($"Login attempt failed for Admin Username: {request.UserName}");
                return false;
            }

            var info = await appDbContext.AdminInfo
                .FirstAsync(x => x.Username == request.UserName && x.Password == request.Password);
            info.CurrentlyIn = 1;
            info.LoginDate = DateTime.Now;

            appDbContext.Admins.Where(x => x.AdminInfo.Username == request.UserName)?.FirstOrDefault()?.AdminInfo = info;
            await appDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> LogoutAsync(LogoutRequest request)
        {
            var findCurrentAdmin = appDbContext.AdminInfo.FirstOrDefault(x => x.Username == request.UserName);

            if(findCurrentAdmin == null)
            {
                logger.LogWarning($"Logout attempt failed for Admin Username: {request.UserName}");
                return false;
            }

            var info = await appDbContext.AdminInfo.FirstAsync(x => x.Username == request.UserName);
            info.CurrentlyIn = 0;
            info.LogoutDate = DateTime.Now;

            appDbContext.Admins.Where(x => x.AdminInfo.Username == request.UserName)?.FirstOrDefault()?.AdminInfo = info;
            await appDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<RoomModel> InsertNewRoom(NewRoomRequest request)
        {
            var newRoom = new RoomModel
            {
                Name = request.Name,
                Floor = request.Floor,
                Capacity = request.Capacity,
                Amenities = request.Amenities,
                Image = request.Image,
                RoomState = RoomState.Available,
                Reservations = new List<ReservationModel>()
            };
            appDbContext.Rooms.Add(newRoom);
            await appDbContext.SaveChangesAsync();
            return newRoom;
        }

        public Task<List<string>> GetRoomConfigs()
        {
            return appDbContext.Amenities.Select(x => x.Name).ToListAsync();
        }
    }
}