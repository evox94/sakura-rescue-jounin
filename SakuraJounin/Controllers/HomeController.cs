using Microsoft.AspNetCore.Mvc;

namespace SakuraJounin.Controllers
{
    [ApiController]
    [Route("/")]
    [Route("/api")]
    public class HomeController : ControllerBase
    {
        public HomeController()
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hi, jounin service here, try swagger at /swagger");
        }
    }

}
