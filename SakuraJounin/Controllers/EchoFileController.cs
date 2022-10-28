using Microsoft.AspNetCore.Mvc;

namespace SakuraJounin.Controllers
{
    [ApiController]
    [Route("/api/echofile")]
    public class EchoFileController : ControllerBase
    {
        public EchoFileController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetFile([FromServices]IConfiguration configuration)
        {
            var filename = configuration.GetValue<string>("EchoFilePath");
            var fileContents = await System.IO.File.ReadAllTextAsync(filename);

            return Ok(fileContents);
        }
    }
    
}
