using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request
{
    public class BookRoomRequest
    {
        public Guid RequestId { get; set; }
        public int RoomId { get; set; }
        public DateTime BookingDate { get; set; }
        public string MeetingTitle { get; set; }
    }
}