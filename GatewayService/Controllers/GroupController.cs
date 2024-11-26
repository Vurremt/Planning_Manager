using GatewayService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;

namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        HttpClient client;
        public GroupController()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:5002/");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetMyGroupAsync()
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.GetAsync($"api/Group/list/{UserId}");
            Console.WriteLine(response.Content);
            Console.WriteLine(response.StatusCode);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<Entities.GroupModel[]>();
                //var tasks = JsonSerializer.Deserialize<Entities.Event[]>(json);
                return Ok(tasks);
            }
            else
            {
                return BadRequest("GetMyGroupAsync failed");
            }

        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult> CreateGroup(GroupCreate _group)
        {
            if (string.IsNullOrWhiteSpace(_group.Titre))
                return BadRequest("Title can't be empty.");

            if (!_group.ManagerIds.Any())
                return BadRequest("List of manager can't be empty.");

            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.PostAsJsonAsync($"api/Group/create", _group);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("ok");
                var newGroup = await response.Content.ReadFromJsonAsync<Entities.GroupModel>();
                return Ok(newGroup);
            }
            else
            {
                return BadRequest("CreateGroup failed");
            }
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateGroup(int id, GroupCreate _group)
        {
            if (string.IsNullOrWhiteSpace(_group.Titre))
                return BadRequest("Title can't be empty.");

            if (!_group.ManagerIds.Any())
                return BadRequest("List of manager can't be empty.");

            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.PutAsJsonAsync($"api/Group/update/{id}", _group);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var newGroup = await response.Content.ReadFromJsonAsync<Entities.GroupModel>();
                return Ok(newGroup);
            }
            else
            {
                return BadRequest("UpdateGroup failed");
            }

        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteGroup(int id)
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.DeleteAsync($"api/Group/delete/{id}");

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
                return BadRequest("DeleteGroup failed");
            }
        }
    }
}