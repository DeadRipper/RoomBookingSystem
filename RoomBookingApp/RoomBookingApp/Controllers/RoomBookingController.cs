using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers;
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
    public class RoomBookingController(IRoomManagment _roomManagment) : ControllerBase
    {
        [HttpPost("bookRoom")]
        public async Task<IActionResult> BookRoom([FromBody] BookRoomRequest request)
        {
            return Ok(JsonBuildHelper.BuildJsonResponse(new BookRoomResponse
            {
                RequestId = request.RequestId,
                RoomId = request.RoomId,
                BookState = await _roomManagment.BookRoom(request)
            }));
        }

        [HttpPost("unbookRoom")]
        public async Task<IActionResult> UnbookRoom([FromBody] UnbookRoomRequest request)
        {
            return Ok(JsonBuildHelper.BuildJsonResponse(new UnbookRoomResponse
            {
                RequestId = request.RequestId,
                RoomId = request.RoomId,
                BookState = await _roomManagment.UnbookRoom(request)
            }));
        }

        [HttpPost("changeBookingSettings")]
        public async Task<IActionResult> ChangeBookingSettings([FromBody] ChangeBookingSettingsRequest request)
        {
            return Ok(await _roomManagment.ChangeBookingSettings(request));
        }

        [HttpPost("checkAvailable")]
        public async Task<IActionResult> CheckAvailable([FromBody] CheckRoomAvailableRequest request)
        {
            return Ok(JsonBuildHelper.BuildJsonResponse(new CheckRoomAvailableResponse
            {
                RequestId = request.RequestId,
                roomState = await _roomManagment.CheckIfRoomIsAvailable(request)
            }));
        }
    }
}