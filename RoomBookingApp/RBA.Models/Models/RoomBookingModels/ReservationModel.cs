using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Models.RoomBookingModels
{
    public class ReservationModel
    {
        public int Id { get; set; }
        public string MeetingTitle { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
    }
}