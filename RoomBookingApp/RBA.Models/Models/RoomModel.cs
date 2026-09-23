using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Models
{
    public class RoomModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public AmenityModel Amenities { get; set; }
        public string Image { get; set; }
        public RoomState RoomState { get; set; }
    }
}