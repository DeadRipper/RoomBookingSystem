using Microsoft.Extensions.Logging;
using RBA.DBase.Managers;
using RBA.Models.Models;
using RBA.Models.Request.BookRoom;
using RBA.Models.Request.CheckRoomAvailable;
using RBA.Models.Request.UnbookRoom;
using RBA.Models.States;
using System.Text.Json;

namespace RBA.DBase
{
    public class RoomManagment(IDBWorker dbWorker, ILogger<RoomManagment> logger) : IRoomManagment
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
    }
}