using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers;
using RBA.Models.Request;
using RoomBookingApp.Helpers;
using System.Reflection;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomBookingController(IRoomManagment _roomManagment) : ControllerBase
    {
        [HttpPost("bookRoom")]
        public async Task<IActionResult> BookRoom([FromBody] BookRoomRequest request)
        {
            return Ok(new BookRoomResponse
            {
                RoomId = request.RoomId,
                BookState = await _roomManagment.BookRoom(request.RoomId)
            });
        }

        [HttpPost("unbookRoom")]
        public async Task<IActionResult> UnbookRoom([FromBody] BookRoomRequest request)
        {
            return Ok(new BookRoomResponse
            {
                RoomId = request.RoomId,
                BookState = await _roomManagment.UnbookRoom(request.RoomId)
            });
        }

        [HttpPost("checkAvailable")]
        public async Task<IActionResult> CheckAvailable([FromBody] BookRoomRequest request)
        {
            return Ok(JsonBuildHelper.BuildJsonResponse(new BookRoomResponse
            {
                RoomId = request.RoomId,
                RoomState = await _roomManagment.CheckIfRoomIsAvailable(request.RoomId)
            }));
        }
    }
}