using RBA.Models.Request.RoomBooking.ChangeBookingSettings;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.CacheBookings
{
    public interface ICacheWorker
    {
        Task<ChangesState> InsertNewChanges(ChangeBookingSettingsRequest changeBookingSettingsRequest);
    }
}