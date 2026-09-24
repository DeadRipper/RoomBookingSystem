using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.UnbookRoom
{
    public class UnbookRoomRequest : RequestBase
    {
        public int RoomId { get; set; }
    }
}