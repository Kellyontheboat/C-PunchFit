using Microsoft.AspNetCore.Mvc;

namespace MyApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CounterController : ControllerBase
    {
        private static int _counter = 0;

        [HttpGet]
        public IActionResult GetCounter()
        {
            return Ok(new { counter = _counter });
        }

        [HttpPost]
        public IActionResult IncrementCounter()
        {
            _counter++;
            return Ok(new { counter = _counter });
        }

        [HttpPost("reset")]
        public IActionResult ResetCounter()
        {
            _counter = 0;
            return Ok(new { counter = _counter });
        }
    }
}

