using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers;
using RBA.Models.Request.Admin.Login;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IAdminManagment _adminManagment) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _adminManagment.LoginAsync(request);
            if (!result)
            {
                return Unauthorized();
            }
            return Ok();
        }
    }
}