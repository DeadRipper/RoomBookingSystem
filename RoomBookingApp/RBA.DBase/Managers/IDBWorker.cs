using RBA.Models.Models;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers
{
    public interface IDBWorker
    {
        Task<RoomState> GetRoomAvailabilityState(int roomId);
        Task<BookState> BookingRoom(int roomId);
        Task<BookState> UnbookingRoom(int roomId);
        Task<IEnumerable<RoomModel>> GetAllRooms();
    }
}