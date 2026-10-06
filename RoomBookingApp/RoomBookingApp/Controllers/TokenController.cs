using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoomBookingApp.Helpers;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController(IConfiguration configuration) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("getToken")]
        public async Task<IActionResult> GenerateToken()
        {
            return Ok( new
            {
                token = await new TokenHelper(configuration).GenerateToken()
            });
        }
    }
}