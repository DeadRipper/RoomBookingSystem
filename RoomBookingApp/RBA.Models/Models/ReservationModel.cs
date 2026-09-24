using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Models
{
    public class ReservationModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public UserModel Users { get; set; }
        public int RoomId { get; set; }
        public RoomModel Room { get; set; }
    }
}