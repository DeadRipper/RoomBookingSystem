using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Models
{
    public class RoomModel
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public RoomState RoomState { get; set; }
    }
}