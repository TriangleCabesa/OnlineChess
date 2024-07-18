using Microsoft.AspNetCore.Mvc;

namespace ChessAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChessController : ControllerBase
    {
        [HttpGet(Name = "GetWeatherForecast")]
        public int Get()
        {
            return 0;
        }
    }
}
