using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers;
using RBA.Models.Request;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetRoomInfoController(IRoomManagment _roomManagment) : ControllerBase
    {
        [HttpPost("roomInfo")]
        public async Task<IActionResult> RoomInfo()
        {
            return Ok(await _roomManagment.GetAllRooms() ?? "no rooms");
        }
    }
}