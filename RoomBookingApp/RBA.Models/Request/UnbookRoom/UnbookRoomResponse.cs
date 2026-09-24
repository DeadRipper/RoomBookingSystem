using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.UnbookRoom
{
    public class UnbookRoomResponse : ResponseBase
    {
        public int RoomId { get; set; }
        public BookState BookState { get; set; }
    }
}