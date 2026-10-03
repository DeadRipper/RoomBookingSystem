using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers;
using RBA.Models.Request.User.Registration;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserManagment userManagment) : ControllerBase
    {
        [HttpPost("registrate")]
        public async Task<IActionResult> RegistrateUser([FromBody] RegistrationRequest request)
        {
            await userManagment.RegistrateUserAsync(request);
            return Ok();
        }
    }
}