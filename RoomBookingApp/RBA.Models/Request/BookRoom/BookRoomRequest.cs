using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.BookRoom
{
    public class BookRoomRequest : RequestBase
    {
        public int RoomId { get; set; }
        public DateTime BookingDate { get; set; }
        public string MeetingTitle { get; set; }
    }
}