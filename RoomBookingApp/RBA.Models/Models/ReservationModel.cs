using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Models
{
    public class ReservationModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public List<UserModel> Users { get; set; }
    }
}