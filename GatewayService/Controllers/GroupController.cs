using GatewayService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

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
        [HttpGet("followed")]
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
        [HttpGet("admin/{id}")]
        public async Task<ActionResult> IsUserAdmin(int id)
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.GetAsync($"api/Group/admin/{id}/{UserId}");

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<bool>();
                return Ok(tasks);
            }
            else
            {
                return BadRequest("IsUserAdmin failed");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetGroupById(int id)
        {

            HttpResponseMessage response = await client.GetAsync($"api/Group/{id}");

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<Entities.GroupModel>();
                return Ok(tasks);
            }
            else
            {
                return BadRequest("GetGroupById failed");
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllGroups()
        {

            HttpResponseMessage response = await client.GetAsync($"api/Group/list");

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<Entities.GroupModel[]>();
                return Ok(tasks);
            }
            else
            {
                return BadRequest("GetAllGroups failed");
            }
        }

        [Authorize]
        [HttpGet("{id}/add")]
        public async Task<ActionResult> AddUserToGroup(int id)
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            if (!int.TryParse(UserId, out var userId))
            {
                return BadRequest("UserId parsing failed");
            }

            Console.WriteLine("requete");
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/Group/{id}/add", userId);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("ok");
                var group = await response.Content.ReadFromJsonAsync<Entities.GroupModel>();
                return Ok(group);
            }
            else
            {
                return BadRequest("AddUserToGroup failed");
            }
        }

        [Authorize]
        [HttpGet("{id}/remove")]
        public async Task<ActionResult> RemoveUserFromGroup(int id)
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            if (!int.TryParse(UserId, out var userId))
            {
                return BadRequest("UserId parsing failed");
            }

            HttpResponseMessage response = await client.PutAsJsonAsync($"api/Group/{id}/remove", userId);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var group = await response.Content.ReadFromJsonAsync<Entities.GroupModel>();
                return Ok(group);
            }
            else
            {
                return BadRequest("RemoveUserFromGroup failed");
            }
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult> CreateGroup(GroupCreate group)
        {
            if (string.IsNullOrWhiteSpace(group.Titre))
                return BadRequest("Please fill in all required fields.");

            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            if(!int.TryParse(UserId, out var userId))
            {
                return BadRequest("CreateGroup failed");
            }
            group.ManagerIds.Add(userId);
            HttpResponseMessage response = await client.GetAsync($"api/Group/list");

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var existingGroups = await response.Content.ReadFromJsonAsync<Entities.GroupModel[]>();
                if (existingGroups.Any(g => g.Titre.Equals(group.Titre, StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest("A group with this name already exists. Please choose another one.");
                }
                else
                {
                    response = await client.PostAsJsonAsync($"api/Group/create", group);
                    // Check if the response status code is 2XX
                    if (response.IsSuccessStatusCode)
                    {
                        var newGroup = await response.Content.ReadFromJsonAsync<Entities.GroupModel>();
                        return Ok(newGroup);
                    }
                    else
                    {
                        return BadRequest("CreateGroup failed");
                    }
                }
            }
            return BadRequest("CreateGroup failed");
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