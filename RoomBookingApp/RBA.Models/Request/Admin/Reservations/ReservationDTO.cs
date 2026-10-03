using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.Admin.Reservations
{
    public class ReservationDTO
    {
        public DateTime Date { get; set; }
        public string RoomName { get; set; }
        public string UserName { get; set; }
        public string MeetingTitle { get; set; }
    }
}