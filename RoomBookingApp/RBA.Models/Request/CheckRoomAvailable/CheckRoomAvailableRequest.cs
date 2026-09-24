using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.CheckRoomAvailable
{
    public class CheckRoomAvailableRequest : RequestBase
    {
        public int RoomId { get; set; }
        public DateTime Date { get; set; }
    }
}