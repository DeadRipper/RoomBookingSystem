using RBA.Models.Models.RoomBookingModels;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.Admin.NewRoom
{
    public class NewRoomRequest
    {
        public string Name { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public AmenityModel Amenities { get; set; }
        public string Image { get; set; }
    }
}