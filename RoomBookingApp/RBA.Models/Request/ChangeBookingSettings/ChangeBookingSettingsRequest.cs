using RBA.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.ChangeBookingSettings
{
    public class ChangeBookingSettingsRequest : RequestBase
    {
        public DateTime Date { get; set; }
        public UserModel Users { get; set; }
        public int RoomId { get; set; }
    }
}