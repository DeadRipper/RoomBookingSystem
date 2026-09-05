using Microsoft.Extensions.Logging;
using RBA.DBase.Managers;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase
{
    public class DBWorker(ILogger<DBWorker> logger) : IDBWorker
    {
        public async Task<RoomState> GetRoomState(int roomId)
        {
            try
            {
                throw new NotImplementedException();
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

        public async Task GetAllRooms()
        {
            throw new NotImplementedException();
        }
    }
}