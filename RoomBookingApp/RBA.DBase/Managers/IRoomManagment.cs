using RBA.Models.Request;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers
{
    public interface IRoomManagment
    {
        Task<RoomState> CheckIfRoomIsAvailable(int roomId);
        Task<BookState> BookRoom(BookRoomRequest bookRoomRequest);
        Task<BookState> UnbookRoom(int roomId);
        Task<string> GetRoomInfo();
        Task<string> GetAllRooms();
    }
}