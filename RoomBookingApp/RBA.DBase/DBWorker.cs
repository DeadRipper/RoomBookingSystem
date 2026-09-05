using Microsoft.Extensions.Logging;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase
{
    public class DBWorker(AppDbContext appDbContext, ILogger<DBWorker> logger) : IDBWorker
    {
        public async Task<RoomState> GetRoomState(int roomId)
        {
            try
            {
                var rooms = appDbContext.Rooms.Where(xx => xx.RoomState == RoomState.Available).Select(x => x.Id).ToList();
                return RoomState.Available;
            }
            catch
            {
                logger.LogError($"Error while getting room state for Room ID: {roomId}");
                throw;
            }
        }

        public async Task<BookState> BookingRoom(int roomId)
        {
            throw new NotImplementedException();
        }

        public async Task<BookState> UnbookingRoom(int roomId)
        {
            throw new NotImplementedException();
        }

        public async Task GetAllRooms()
        {
            throw new NotImplementedException();
        }
    }
}