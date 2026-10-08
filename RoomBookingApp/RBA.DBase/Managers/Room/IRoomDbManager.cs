using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.RoomBooking.BookRoom;
using RBA.Models.Request.RoomBooking.CheckRoomAvailable;
using RBA.Models.Request.RoomBooking.UnbookRoom;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers.Room
{
    public interface IRoomDbManager
    {
        Task<RoomState> GetRoomAvailabilityState(CheckRoomAvailableRequest checkRoomAvailableRequest);
        Task<BookState> BookingRoom(BookRoomRequest bookRoomRequest);
        Task<BookState> UnbookingRoom(UnbookRoomRequest unbookRoomRequest);
        Task<IEnumerable<RoomModel>> GetAllRooms();
    }
}