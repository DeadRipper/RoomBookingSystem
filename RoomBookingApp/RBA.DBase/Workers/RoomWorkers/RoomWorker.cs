using Microsoft.Extensions.Logging;
using RBA.CacheBookings;
using RBA.DBase.Managers.DbWorker;
using RBA.DBase.Managers.Room;
using RBA.Models.Models;
using RBA.Models.Request.RoomBooking.BookRoom;
using RBA.Models.Request.RoomBooking.ChangeBookingSettings;
using RBA.Models.Request.RoomBooking.CheckRoomAvailable;
using RBA.Models.Request.RoomBooking.UnbookRoom;
using RBA.Models.States;
using System.Text.Json;

namespace RBA.DBase.Workers.RoomWorkers
{
    public class RoomWorker(ICacheWorker cacheWorker, IRoomDbManager dbWorker, ILogger<RoomWorker> logger) : IRoomManager
    {
        public async Task<string> GetAllRooms()
        {
            logger.LogInformation("Getting information for all rooms");
            return JsonSerializer.Serialize(await dbWorker.GetAllRooms());
        }

        public async Task<string> GetRoomInfo()
        {
            return null;
        }

        public async Task<RoomState> CheckIfRoomIsAvailable(CheckRoomAvailableRequest checkRoomAvailableRequest)
        {
            logger.LogInformation($"Checking availability for room ID: {checkRoomAvailableRequest.RoomId}");
            RoomState roomState = await dbWorker.GetRoomAvailabilityState(checkRoomAvailableRequest);  
            logger.LogInformation($"End checking availability for room ID: {checkRoomAvailableRequest.RoomId}; Status: {roomState.ToString()}");
            return roomState;
        }

        public async Task<BookState> BookRoom(BookRoomRequest bookRoomRequest)
        {
            logger.LogInformation("Attempting to book room ID: {RoomId}", bookRoomRequest.RoomId);
            BookState roomState = await dbWorker.BookingRoom(bookRoomRequest);
            logger.LogInformation("End booking for room ID: {RoomId}; Status: {roomState}", bookRoomRequest.RoomId, roomState);
            return roomState;
        }

        public async Task<BookState> UnbookRoom(UnbookRoomRequest unbookRoomRequest)
        {
            logger.LogInformation("Attempting to unbook room ID: {RoomId}", unbookRoomRequest.RoomId);
            BookState roomState = await dbWorker.UnbookingRoom(unbookRoomRequest);
            logger.LogInformation("End unbooking for room ID: {RoomId}; Status: {roomState}", unbookRoomRequest.RoomId, roomState);
            return roomState;
        }

        public async Task<ChangesState> ChangeBookingSettings(ChangeBookingSettingsRequest changeBookingSettingsRequest)
        {
            logger.LogInformation("Attempting to change booking settings for room ID: {RoomId}", changeBookingSettingsRequest.RoomId);
            cacheWorker.InsertNewChanges(changeBookingSettingsRequest);
            logger.LogInformation("End changing booking settings for room ID: {RoomId}; Status: {changesState}", changeBookingSettingsRequest.RoomId, ChangesState.Accepted);
            return ChangesState.Accepted;
        }
    }
}