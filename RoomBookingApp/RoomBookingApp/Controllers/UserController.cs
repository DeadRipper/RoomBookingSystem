using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers;
using RBA.Models.Request.User.GetUserId;
using RBA.Models.Request.User.Registration;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserManagment userManagment) : ControllerBase
    {
        [HttpPost("getUserId")]
        public async Task<IActionResult> GetUserId([FromBody] GetUserIdRequest request)
        {
            return Ok(await userManagment.GetUserId(request));
        }

        [HttpGet("getAllUsersId")]
        public async Task<IActionResult> GetAllUsersId()
        {
            return Ok(await userManagment.GetAllUsersId());
        }

        [HttpPost("getUserById")]
        public async Task<IActionResult> GetUserById([FromBody] GetUserIdRequest request)
        {
            return Ok(await userManagment.GetUserId(request));
        }

        [HttpPost("registrate")]
        public async Task<IActionResult> RegistrateUser([FromBody] RegistrationRequest request)
        {
            return Ok(await userManagment.RegistrateUserAsync(request));
        }

        

    }
}