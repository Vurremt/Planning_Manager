using GatewayService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;

namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        HttpClient client;
        public EventController()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:5002/");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetMyEventAsync()
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.GetAsync($"api/Event/list/{UserId}");
            Console.WriteLine(response.Content);
            Console.WriteLine(response.StatusCode);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<Entities.EventModel[]>();
                //var tasks = JsonSerializer.Deserialize<Entities.Event[]>(json);
                return Ok(tasks);
            }
            else
            {
                return BadRequest("GetMyEventAsync failed");
            }

        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult> CreateEvent(EventCreateModel _event)
        {
            if (string.IsNullOrWhiteSpace(_event.Title))
                return BadRequest("Title can't be empty.");

            if (string.IsNullOrWhiteSpace(_event.Place))
                return BadRequest("Place can't be empty.");

            if (_event.StartingDate < DateTime.Today)
                return BadRequest("StartingDate can't be in the past.");

            if (_event.Duration < 0)
                return BadRequest("Duration can't be negative.");

            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.PostAsJsonAsync($"api/Event/create", _event);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("ok");
                var newEvent = await response.Content.ReadFromJsonAsync<Entities.EventModel>();
                return Ok(newEvent);
            }
            else
            {
                return BadRequest("CreateEvent failed");
            }
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateEvent(int id, EventCreateModel _event)
        {
            if (string.IsNullOrWhiteSpace(_event.Title))
                return BadRequest("Title can't be empty.");

            if (string.IsNullOrWhiteSpace(_event.Place))
                return BadRequest("Place can't be empty.");

            if (_event.StartingDate < DateTime.Today)
                return BadRequest("StartingDate can't be in the past.");

            if (_event.Duration < 0)
                return BadRequest("Duration can't be negative.");

            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.PutAsJsonAsync($"api/Event/update/{id}", _event);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var newEvent = await response.Content.ReadFromJsonAsync<Entities.EventModel>();
                return Ok(newEvent);
            }
            else
            {
                return BadRequest("UpdateEvent failed");
            }

        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.DeleteAsync($"api/Event/delete/{id}");

            Console.WriteLine(response.Content);
            Console.WriteLine(response.StatusCode);
            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                string str = await response.Content.ReadAsStringAsync();
                if (str == "true")
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            else
            {
                return BadRequest("DeleteEvent failed");
            }
        }
    }
}