using RBA.Models.Models.RoomBookingModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.RoomBooking.ChangeBookingSettings
{
    public class ChangeBookingSettingsRequest : RequestBase
    {
        public DateTime Date { get; set; }
        public UserModel Users { get; set; }
        public int RoomId { get; set; }
    }
}