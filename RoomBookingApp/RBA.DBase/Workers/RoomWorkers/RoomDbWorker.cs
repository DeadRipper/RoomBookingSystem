using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers.Room;
using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.RoomBooking.BookRoom;
using RBA.Models.Request.RoomBooking.CheckRoomAvailable;
using RBA.Models.Request.RoomBooking.UnbookRoom;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Workers.RoomWorkers
{
    public class RoomDbWorker(AppDbContext appDbContext, ILogger<RoomDbWorker> logger) : IRoomDbManager
    {
        public async Task<BookState> BookingRoom(BookRoomRequest bookRoomRequest)
        {
            if (bookRoomRequest.RoomId == 0 || bookRoomRequest.UserId == 0)
                return BookState.Failed;

            if (appDbContext.Users.FirstOrDefault(x => x.Id == bookRoomRequest.UserId) == null)
            {
                logger.LogError($"User with ID: {bookRoomRequest.UserId} not found while booking room for Room ID: {bookRoomRequest.RoomId}");
                return BookState.Failed;
            }

            RoomModel room;

            try
            {
                room = appDbContext?.Rooms?.Where(x => x.Id == bookRoomRequest.RoomId)?.FirstOrDefault();
                var reserv = appDbContext?.Reservations.Where(r => r.Date == bookRoomRequest.BookingDate && r.RoomId == bookRoomRequest.RoomId)?.FirstOrDefault();
                if (room == null || reserv == null)
                    return BookState.Failed;
            }
            catch
            {
                logger.LogError($"Error while booking room for Room ID: {bookRoomRequest.RoomId}");
                throw;
            }

            var reservation = new ReservationModel
            {
                RoomId = bookRoomRequest.RoomId,
                Date = bookRoomRequest.BookingDate,
                UserId = bookRoomRequest.UserId,
                MeetingTitle = bookRoomRequest.MeetingTitle
            };
            appDbContext.Reservations.Add(reservation);

            await appDbContext.SaveChangesAsync();
            return BookState.Confirmed;
        }

        public async Task<IEnumerable<RoomModel>> GetAllRooms()
        {
            return await appDbContext.Rooms.Where(x => x.Id != 0).ToListAsync();
        }

        public async Task<RoomState> GetRoomAvailabilityState(CheckRoomAvailableRequest checkRoomAvailableRequest)
        {
            try
            {
                return appDbContext.Rooms?.
                    Where(x =>
                    x.Id == checkRoomAvailableRequest.RoomId)?
                    .Select(xx =>
                    xx.ReservationsId.Count() > 0 ? RoomState.Available : RoomState.Occupied)?.FirstOrDefault() ?? RoomState.Occupied;
            }
            catch
            {
                logger.LogError($"Error while getting room state for Room ID: {checkRoomAvailableRequest.RoomId}");
                throw;
            }
        }

        public async Task<BookState> UnbookingRoom(UnbookRoomRequest unbookRoomRequest)
        {
            RoomModel room;
            ReservationModel reservationModel;
            try
            {
                room = appDbContext.Rooms.Where(x => x.Id == unbookRoomRequest.RoomId)?.FirstOrDefault();
                if (room == null)
                    return BookState.Failed;
            }
            catch
            {
                logger.LogError($"Error while unbooking room for Room ID: {unbookRoomRequest.RoomId}");
                throw;
            }

            if (room.ReservationsId != null)
            {
                reservationModel = appDbContext.Reservations?.FirstOrDefault(r => r.Id == unbookRoomRequest.ReservationId);
                if (reservationModel == null)
                {
                    return BookState.Failed;
                }   
            }
            else
            {
                return BookState.Failed;
            }

            appDbContext.Rooms.Where(x => x.Id == unbookRoomRequest.RoomId).FirstOrDefault().ReservationsId.Remove(unbookRoomRequest.ReservationId);
            appDbContext.Reservations.Remove(reservationModel);

            await appDbContext.SaveChangesAsync();
            return BookState.Cancelled;
        }
    }
}