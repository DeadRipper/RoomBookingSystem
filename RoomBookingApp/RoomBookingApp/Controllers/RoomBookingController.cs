using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers.Room;
using RBA.Models.Request.RoomBooking.BookRoom;
using RBA.Models.Request.RoomBooking.ChangeBookingSettings;
using RBA.Models.Request.RoomBooking.CheckRoomAvailable;
using RBA.Models.Request.RoomBooking.UnbookRoom;
using RoomBookingApp.Helpers;
using System.Reflection;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomBookingController(IRoomManager _roomManagment) : ControllerBase
    {
        [Authorize]
        [HttpPost("getAllrooms")]
        public async Task<IActionResult> RoomInfo()
        {
            return Ok(await _roomManagment.GetAllRooms() ?? "no rooms");
        }

        [Authorize]
        [HttpPost("bookRoom")]
        public async Task<IActionResult> BookRoom([FromBody] BookRoomRequest request)
        {
            return Ok(new BookRoomResponse
            {
                RequestId = request.RequestId,
                RoomId = request.RoomId,
                BookState = await _roomManagment.BookRoom(request)
            });
        }

        [Authorize]
        [HttpPost("unbookRoom")]
        public async Task<IActionResult> UnbookRoom([FromBody] UnbookRoomRequest request)
        {
            return Ok(new UnbookRoomResponse
            {
                RequestId = request.RequestId,
                RoomId = request.RoomId,
                BookState = await _roomManagment.UnbookRoom(request)
            });
        }

        [Authorize]
        [HttpPost("changeBookingSettings")]
        public async Task<IActionResult> ChangeBookingSettings([FromBody] ChangeBookingSettingsRequest request)
        {
            return Ok(_roomManagment.ChangeBookingSettings(request));
        }

        [Authorize]
        [HttpPost("checkAvailable")]
        public async Task<IActionResult> CheckAvailable([FromBody] CheckRoomAvailableRequest request)
        {
            return Ok(new CheckRoomAvailableResponse
            {
                RequestId = request.RequestId,
                roomState = await _roomManagment.CheckIfRoomIsAvailable(request)
            });
        }
    }
}