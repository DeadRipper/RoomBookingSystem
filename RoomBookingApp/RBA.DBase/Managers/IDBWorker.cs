using RBA.Models.Models;
using RBA.Models.Request.BookRoom;
using RBA.Models.Request.CheckRoomAvailable;
using RBA.Models.Request.UnbookRoom;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers
{
    public interface IDBWorker
    {
        Task<RoomState> GetRoomAvailabilityState(CheckRoomAvailableRequest checkRoomAvailableRequest);
        Task<BookState> BookingRoom(BookRoomRequest bookRoomRequest);
        Task<BookState> UnbookingRoom(UnbookRoomRequest unbookRoomRequest);
        Task<IEnumerable<RoomModel>> GetAllRooms();
    }
}