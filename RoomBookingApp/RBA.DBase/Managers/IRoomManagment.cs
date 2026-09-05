using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Managers
{
    public interface IRoomManagment
    {
        Task<RoomState> CheckIfRoomIsAvailable(int roomId);
        Task<BookState> BookRoom(int roomId);
        Task GetAllRooms();
    }
}