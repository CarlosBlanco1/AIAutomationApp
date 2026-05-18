using Microsoft.AspNetCore.Mvc;

namespace app_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello from the API!");
        }
    }
}
