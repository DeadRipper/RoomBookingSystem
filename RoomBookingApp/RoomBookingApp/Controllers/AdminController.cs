using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers;
using RBA.Models.Request.Admin.Cancel;
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

        [HttpGet("getAllRoomsCount")]
        public async Task<IActionResult> GetAllRoomsCount()
        {
            return Ok(await _adminManagment.GetAllRoomsCount());
        }

        [HttpGet("getTodayBookings")]
        public async Task<IActionResult> GetTodayBookings()
        {
            return Ok(await _adminManagment.GetTodayBookings());
        }

        [HttpGet("totalBookings")]
        public async Task<IActionResult> TotalBookings()
        {
            return Ok(await _adminManagment.GetTotalBookings());
        }

        [HttpGet("getAllReservations")]
        public async Task<IActionResult> GetAllReservations()
        {
            return Ok(await _adminManagment.GetAllReservations());
        }

        [HttpGet("getRoomConfigs")]
        public async Task<IActionResult> GetRoomConfigs()
        {
            return Ok(await _adminManagment.GetRoomConfigs());
        }

        [HttpPost("addRoom")]
        public async Task<IActionResult> AddRoom([FromBody] NewRoomRequest request)
        {            
            return Ok(await _adminManagment.InsertNewRoom(request));
        }

        [HttpPost("cancelBooking")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequest request)
        {
            return Ok(await _adminManagment.CancelBookingAsync(request));
        }
    }
}