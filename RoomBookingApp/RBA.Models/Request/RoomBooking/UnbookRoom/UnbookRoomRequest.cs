using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.RoomBooking.UnbookRoom
{
    public class UnbookRoomRequest : RequestBase
    {
        public int RoomId { get; set; }
        public int ReservationId { get; set; }
    }
}