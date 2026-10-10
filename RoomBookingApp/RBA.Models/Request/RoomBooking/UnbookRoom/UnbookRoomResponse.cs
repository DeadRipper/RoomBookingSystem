using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.RoomBooking.UnbookRoom
{
    public class UnbookRoomResponse : ResponseBase
    {
        public BookState BookState { get; set; }
    }
}