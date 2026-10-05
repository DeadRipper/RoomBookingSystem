using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.RoomBooking.CheckRoomAvailable
{
    public class CheckRoomAvailableResponse : ResponseBase
    {
        public RoomState roomState { get; set; }
    }
}