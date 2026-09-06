using RBA.DBase.Managers;
using RBA.Models.States;
using RBA.Models.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace RBA.DBase
{
    public class RoomManagment(IDBWorker dbWorker, ILogger<RoomManagment> logger) : IRoomManagment
    {
        public async Task<string> GetAllRooms()
        {
            logger.LogInformation($"GetRoomInfo");
            return JsonSerializer.Serialize(await dbWorker.GetAllRooms());
        }

        public async Task<string> GetRoomInfo()
        {
            return null;
        }

        public async Task<RoomState> CheckIfRoomIsAvailable(int roomId)
        {
            logger.LogInformation($"Checking availability for room ID: {roomId}");
            RoomState roomState = await dbWorker.GetRoomAvailabilityState(roomId);
            logger.LogInformation($"End checking availability for room ID: {roomId}; Status: {roomState.ToString()}");
            return roomState;
        }

        public async Task<BookState> BookRoom(int roomId)
        {
            logger.LogInformation($"Attempting to book room ID: {roomId}");
            BookState roomState = await dbWorker.BookingRoom(roomId);
            logger.LogInformation($"End booking for room ID: {roomId}; Status: {roomState.ToString()}");
            return roomState;
        }

        public async Task<BookState> UnbookRoom(int roomId)
        {
            logger.LogInformation($"Attempting to unbook room ID: {roomId}");
            BookState roomState = await dbWorker.UnbookingRoom(roomId);
            logger.LogInformation($"End unbooking for room ID: {roomId}; Status: {roomState.ToString()}");
            return roomState;
        }
    }
}