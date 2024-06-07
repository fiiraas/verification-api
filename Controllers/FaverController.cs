using Faver2.Data;
using Faver2.FaverModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace Faver2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaverController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger<FaverController> _logger;

        public FaverController(ApplicationDbContext context, HttpClient httpClient, ILogger<FaverController> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserProfiles>>> GetAllProfiles()
        {
            var profiles = await _context.Profiles.ToListAsync();
            return Ok(profiles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfiles>> GetProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
                return NotFound("Profile not found.");

            return Ok(profile);
        }

        [HttpPost]
        public async Task<ActionResult<List<UserProfiles>>> AddProfile(UserProfiles profile)
        {
            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();
            return Ok(await _context.Profiles.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<List<UserProfiles>>> UpdateProfile(UserProfiles updatedProfile)
        {
            var dbProfile = await _context.Profiles.FindAsync(updatedProfile.Id);
            if (dbProfile == null)
                return NotFound("Profile not found.");

            dbProfile.UserName = updatedProfile.UserName;
            dbProfile.Gambar = updatedProfile.Gambar;
            dbProfile.MyKadNumber = updatedProfile.MyKadNumber;

            await _context.SaveChangesAsync();
            return Ok(await _context.Profiles.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<UserProfiles>>> DeleteProfile(int id)
        {
            var dbProfile = await _context.Profiles.FindAsync(id);
            if (dbProfile == null)
                return NotFound("Profile not found.");

            _context.Profiles.Remove(dbProfile);
            await _context.SaveChangesAsync();
            return Ok(await _context.Profiles.ToListAsync());
        }

        //verification endpoint
        [HttpPost("verify")]
        public async Task<ActionResult<string>> VerifyFace([FromBody] FaceVerificationRequest request)
        {
            var userProfile = await _context.Profiles.FindAsync(request.UserId);
            if (userProfile == null)
                return NotFound("Profile not found.");

            if (userProfile.Gambar == null)
                return BadRequest("User profile does not have an image to compare.");

            var payload = new
            {
                user_id = request.UserId,
                img1_path = "data:image/jpeg;base64," + request.CapturedImage,
                img2_path = "data:image/jpeg;base64," + Convert.ToBase64String(userProfile.Gambar),
                model_name = "Facenet512",
                detector_backend = "fastmtcnn",
                distance_metric = "cosine"
            };

            string jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending payload to verification API: {Payload}", jsonPayload);

            HttpResponseMessage response = await _httpClient.PostAsync("http://192.168.0.90:5000/verify", content);

            //if code 200
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from verification API: {ResponseContent}", responseContent);

                var jsonDoc = JsonDocument.Parse(responseContent);
                var verified = jsonDoc.RootElement.GetProperty("verified").GetBoolean();

                if (verified)
                {
                    var clockIn = new ClockIn
                    {
                        UserProfilePenggunaId = request.UserId,
                        ClockInTime = DateTime.Now,
                    };

                    _context.ClockIns.Add(clockIn);
                    await _context.SaveChangesAsync();

                    return Ok(responseContent);
                }

                else if(!verified)
                {
                    return Ok(responseContent);
                }

                else
                {
                    return BadRequest("Face verification failed.");
                }
                
            }

            // other than 200
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error from verification API: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                return StatusCode((int)response.StatusCode, $"Failed to verify faces. Error: {errorContent}");
            }
        }

        public class FaceVerificationRequest
        {
            public int UserId { get; set; }
            public string CapturedImage { get; set; }
        }
    }
}
