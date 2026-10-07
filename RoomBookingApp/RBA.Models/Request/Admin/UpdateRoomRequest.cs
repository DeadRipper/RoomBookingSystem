using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.Admin
{
    public class UpdateRoomRequest
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public List<int> AmenityIds { get; set; }
    }
}