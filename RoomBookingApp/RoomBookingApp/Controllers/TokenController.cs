using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBA.DBase.Managers.Auth;
using RoomBookingApp.Helpers;

namespace RoomBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController(IAuthManager authManager) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("getToken")]
        public async Task<IActionResult> GetToken()
        {
            var tokenRes = await authManager.GetToken();
            return Ok(new
            {
                token = tokenRes.Token,
                expirationDate = tokenRes.ExpirationDate
            });
        }
    }
}