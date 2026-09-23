using RBA.Models.Models;
using RBA.Models.Request;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers
{
    public interface IDBWorker
    {
        Task<RoomState> GetRoomAvailabilityState(int roomId);
        Task<BookState> BookingRoom(BookRoomRequest bookRoomRequest);
        Task<BookState> UnbookingRoom(int roomId);
        Task<IEnumerable<RoomModel>> GetAllRooms();
    }
}