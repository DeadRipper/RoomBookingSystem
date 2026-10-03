using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers;
using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
using RBA.Models.Request.Admin.NewRoom;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IAdminManagment _adminManagment) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!await _adminManagment.LoginAsync(request))
                return Unauthorized();
            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (!await _adminManagment.LogoutAsync(request))
                return BadRequest();
            return Ok();
        }

        [HttpGet("totalBookings")]
        public async Task<IActionResult> TotalBookings()
        {
            return Ok(await _adminManagment.GetTotalBookings());
        }

        [HttpGet("getRoomConfigs")]
        public async Task<IActionResult> GetRoomConfigs()
        {
            var roomConfigs = await _adminManagment.GetRoomConfigs();
            return Ok(roomConfigs);
        }

        [HttpPost("addRoom")]
        public async Task<IActionResult> AddRoom([FromBody] NewRoomRequest request)
        {            
            return Ok(await _adminManagment.InsertNewRoom(request));
        }
    }
}