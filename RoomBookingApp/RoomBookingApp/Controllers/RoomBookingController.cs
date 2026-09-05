using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers;
using RBA.Models.Request;
using System.Reflection;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomBookingController(IRoomManagment _roomManagment) : ControllerBase
    {
        [HttpPost("BookRoom")]
        public async Task<IActionResult> BookRoom([FromBody] BookRoomRequest request)
        {
            await _roomManagment.BookRoom(request.RoomId);
            return Ok($"Room {request.RoomId} booked successfully.");
        }

        [HttpPost("UnbookRoom")]
        public async Task<IActionResult> UnbookRoom([FromBody] BookRoomRequest request)
        {
            await _roomManagment.UnbookRoom(request.RoomId);
            return Ok($"Room {request.RoomId} unbooked successfully.");
        }

        [HttpPost(template: "checkAvailable")]
        public async Task<IActionResult> CheckAvailable([FromBody] BookRoomRequest request)
        {
            var isAvailable = await _roomManagment.CheckIfRoomIsAvailable(request.RoomId);
            return Ok(new { IsAvailable = isAvailable });
        }
    }
}