using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers.Admin;
using RBA.DBase.Workers.RoomWorkers;
using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.Admin.Cancel;
using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
using RBA.Models.Request.Admin.NewRoom;
using RBA.Models.Request.RoomBooking.UnbookRoom;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Workers.AdminWorkers
{
    public class AdminDbWorker(AppDbContext appDbContext, ILogger<AdminDbWorker> logger) : IAdminDbManager
    {
        public async Task<bool> CancelBookingAsync(CancelBookingRequest request)
        {
            var reservation = await appDbContext.Reservations.FirstOrDefaultAsync(r => r.Id == request.Id);
            if (reservation == null)
            {
                logger.LogWarning($"Cancel booking attempt failed for Booking ID: {request.Id}");
                return false;
            }

            var room = await appDbContext.Rooms.FirstOrDefaultAsync(r => r.ReservationsId != null && r.ReservationsId.Contains(reservation.Id));
            room?.ReservationsId?.Remove(reservation.Id);
            appDbContext.Reservations.Remove(reservation);
            await appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<ReservationModel>> GetAllReservations()
        {
            return await appDbContext.Reservations.ToListAsync();
        }

        public async Task<int> GetAllRoomsCount()
        {
            return await appDbContext.Rooms.CountAsync();
        }

        public Task<List<AmenityModel>> GetRoomConfigs()
        {
            return appDbContext.Amenities.ToListAsync();
        }

        public async Task<int> GetTotalBookings()
        {
            return await appDbContext.Reservations.CountAsync();
        }

        public async Task<int> GetTodayBookings()
        {
            return await appDbContext.Reservations.Where(r => r.Date.Date == DateTime.Today).CountAsync();
        }

        public async Task<RoomModel> InsertNewRoom(NewRoomRequest request)
        {
            var getAmenity = await appDbContext.Amenities.FirstOrDefaultAsync(a => a.Id == request.Amenities.Id);

            if (getAmenity == null)
            {
                logger.LogWarning($"Insert new room attempt failed for Room Name: {request.Name}");
                return null;
            }

            var newRoom = new RoomModel
            {
                Name = request.Name,
                Floor = request.Floor,
                Capacity = request.Capacity,
                Amenities = getAmenity,
                Image = request.Image,
                ReservationsId = null
            };
            appDbContext.Rooms.Add(newRoom);
            await appDbContext.SaveChangesAsync();
            return newRoom;
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

            if (findCurrentAdmin == null)
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
    }
}