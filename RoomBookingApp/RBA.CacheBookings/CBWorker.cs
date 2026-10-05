using RBA.Models.Request.RoomBooking.ChangeBookingSettings;
using RBA.Models.States;

namespace RBA.CacheBookings
{
    public class CBWorker : ICacheWorker
    {
        public Task<ChangesState> InsertNewChanges(ChangeBookingSettingsRequest changeBookingSettingsRequest)
        {
            new ChangesCluster().ChangeBookingSettingsRequest.TryAdd(Guid.NewGuid(), changeBookingSettingsRequest);
            return Task.FromResult(ChangesState.Penging);
        }
    }
}