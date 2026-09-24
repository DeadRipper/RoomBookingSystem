using RBA.Models.Request.BookRoom;
using RBA.Models.Request.CheckRoomAvailable;
using RBA.Models.Request.UnbookRoom;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers
{
    public interface IRoomManagment
    {
        Task<RoomState> CheckIfRoomIsAvailable(CheckRoomAvailableRequest checkRoomAvailableRequest);
        Task<BookState> BookRoom(BookRoomRequest bookRoomRequest);
        Task<BookState> UnbookRoom(UnbookRoomRequest unbookRoomRequest);
        Task<string> GetRoomInfo();
        Task<string> GetAllRooms();
    }
}