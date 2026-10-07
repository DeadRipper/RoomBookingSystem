using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers.Auth;
using RoomBookingApp.Helpers;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController(IConfiguration configuration, IAuthManager authManager) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("getToken")]
        public async Task<IActionResult> GenerateToken()
        {
            var tokenRes = await authManager.GenerateToken();
            return Ok(new
            {
                token = tokenRes.Token,
                exparationDate = tokenRes.ExparationDate
            });
        }
    }
}