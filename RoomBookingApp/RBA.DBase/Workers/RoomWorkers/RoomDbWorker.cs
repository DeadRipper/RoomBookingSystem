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
            if (appDbContext.Users.FirstOrDefault(x => x.Id == bookRoomRequest.UserId) == null)
            {
                logger.LogError($"User with ID: {bookRoomRequest.UserId} not found while booking room for Room ID: {bookRoomRequest.RoomId}");
                return BookState.Failed;
            }

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
    }
}