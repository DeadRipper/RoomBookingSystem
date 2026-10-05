using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.Admin.Cancel
{
    public class CancelBookingRequest : RequestBase
    {
        public int Id { get; set; }
    }
}