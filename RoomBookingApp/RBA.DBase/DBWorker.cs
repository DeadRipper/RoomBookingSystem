using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers;
using RBA.Models.Models;
using RBA.Models.Request;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase
{
    public class DBWorker(AppDbContext appDbContext, ILogger<DBWorker> logger) : IDBWorker
    {
        public async Task<RoomState> GetRoomAvailabilityState(int roomId)
        {
            try
            {
                var a = appDbContext.Rooms?.Where(x => x.Id == roomId)?.Select(xx => xx.RoomState)?.FirstOrDefault() ?? RoomState.Occupied;
                return RoomState.Available;
            }
            catch
            {
                logger.LogError($"Error while getting room state for Room ID: {roomId}");
                throw;
            }
        }

        public async Task<BookState> BookingRoom(BookRoomRequest bookRoomRequest)
        {
            try
            {
                var roomSearch = appDbContext.Rooms.Select(x => x.Id == bookRoomRequest.RoomId);
                if (roomSearch != null && roomSearch.FirstOrDefault() == false)
                    return BookState.Failed;
            }
            catch
            {
                logger.LogError($"Error while booking room for Room ID: {bookRoomRequest.RoomId}");
                throw;
            }

            appDbContext?.Rooms?.Where(x => x.Id == bookRoomRequest.RoomId)?.FirstOrDefault()?.RoomState = RoomState.Occupied;
            appDbContext?.SaveChangesAsync();
            return BookState.Confirmed;
        }

        public async Task<BookState> UnbookingRoom(UnbookRoomRequest unbookRoomRequest)
        {
            try
            {
                var roomSearch = appDbContext.Rooms.Select(x => x.Id == unbookRoomRequest.RoomId);
                if (roomSearch != null && roomSearch.FirstOrDefault() == false)
                    return BookState.Failed;

            }
            catch
            {
                logger.LogError($"Error while unbooking room for Room ID: {unbookRoomRequest.RoomId}");
                throw;
            }
            appDbContext?.Rooms?.Where(x => x.Id == unbookRoomRequest.RoomId)?.FirstOrDefault()?.RoomState = RoomState.Available;
            appDbContext?.SaveChangesAsync();
            return BookState.Cancelled;
        }

        public async Task<IEnumerable<RoomModel>> GetAllRooms()
        {
            return await appDbContext.Rooms.Where(x => x.Id != 0).ToListAsync();
        }
    }
}