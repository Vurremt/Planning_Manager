using GatewayService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;

namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        HttpClient client;
        public TaskController()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:5002/");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetMyTaskAsync()
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.GetAsync($"api/Task/list/{UserId}");
            Console.WriteLine(response.Content);
            Console.WriteLine(response.StatusCode);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<Entities.TaskModel[]>();
                //var tasks = JsonSerializer.Deserialize<Entities.Task[]>(json);
                return Ok(tasks);
            }
            else
            {
                return BadRequest("GetMyTaskAsync failed");
            }

        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult> CreateTask(TaskCreateModel task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                return BadRequest("Title can't be empty.");

            if (task.Deadline < DateTime.Today)
                return BadRequest("Deadline can't be in the past.");

            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.PostAsJsonAsync($"api/Task/create/{UserId}", task);
            Console.WriteLine("on sait pas");
            Console.WriteLine(response.Content);
            Console.WriteLine(response.StatusCode);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("ok");
                var newTask = await response.Content.ReadFromJsonAsync<Entities.TaskModel>();
                return Ok(newTask);
            }
            else
            {
                Console.WriteLine("pas ok");
                return BadRequest("CreateTask failed");

            }
            // Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJDeXJpdXMiLCJVc2VySWQiOiIxIiwibmFtZSI6IkN5cml1cyIsImV4cCI6MTcwMzIwMDUwMSwiaXNzIjoiWW91cklzc3VlciIsImF1ZCI6IllvdXJBdWRpZW5jZSJ9.xIuvzZ8UhPvClf5gP1GY33N-JrMSBUdrtQ6lvTnRJ0I
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateTask(int id, TaskCreateModel task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                return BadRequest("Title can't be empty.");

            if (task.Deadline < DateTime.Today)
                return BadRequest("Deadline can't be in the past.");

            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.PutAsJsonAsync($"api/Task/update/{UserId}/{id}", task);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var newTask = await response.Content.ReadFromJsonAsync<Entities.TaskModel>();
                return Ok(newTask);
            }
            else
            {
                return BadRequest("UpdateTask failed");
            }

        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.DeleteAsync($"api/Task/delete/{UserId}/{id}");

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
                return BadRequest("UpdateTask failed");
            }
        }
    }
}