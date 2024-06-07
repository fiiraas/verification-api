using Faver2.Data;
using Faver2.FaverModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace Faver2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger<FaverController> _logger;

        public HelloController(ApplicationDbContext context, HttpClient httpClient, ILogger<FaverController> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetHello()
        {
            return Ok("Hello, World!");
        }
    }
}
