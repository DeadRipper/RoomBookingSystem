using RBA.Models.Request.ChangeBookingSettings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RBA.CacheBookings
{
    public class ChangesCluster
    {
        public ConcurrentDictionary<Guid, ChangeBookingSettingsRequest> ChangeBookingSettingsRequest { get; set; }
    }
}