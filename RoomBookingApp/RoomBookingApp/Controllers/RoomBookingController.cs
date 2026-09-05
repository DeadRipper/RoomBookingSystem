using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers;
using System.Reflection;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomBookingController(IRoomManagment _roomManagment) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> BookRoom(int roomId)
        {
            await _roomManagment.BookRoom(roomId);
            return Ok($"Room {roomId} booked successfully.");
        }
    }
}