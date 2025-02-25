using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using GatewayService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net.Http.Json;


namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory, HttpClient client)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<ActionResult> GetAllUsersAsync()
        {
            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                /*
                var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (UserId == null) return Unauthorized();
                */

                HttpResponseMessage response = await client.GetAsync($"api/Users");

                // Check if the response status code is 2XX
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<Entities.UserDTO[]>();
                    return Ok(users);
                }
                else
                {
                    return BadRequest("GetAllUsersAsync failed");
                }
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetMyUserAsync()
        {
            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (UserId == null) return Unauthorized();

                HttpResponseMessage response = await client.GetAsync($"api/Users/{UserId}");

                // Check if the response status code is 2XX
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                    return Ok(user);
                }
                else
                {
                    return BadRequest("GetMyUserAsync failed");
                }
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserAsync(int id)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                string urlWithId = $"http://localhost:5001/api/Users/{id}";

                HttpResponseMessage response = await client.GetAsync(urlWithId);

                // Check if the response status code is 2XX
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                    return Ok(user);
                }
                else
                {
                    return BadRequest("GetUserAsync failed");
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateModel userUdpate)
        {
            if (userUdpate.Name != null && !userUdpate.Name.All(c => Char.IsLetterOrDigit(c)))
                return BadRequest("The username must only contain alphanumeric characters.");

            if (userUdpate.Password != null && !userUdpate.Password.All(c => Char.IsLetterOrDigit(c)))
                return BadRequest("The password must only contain alphanumeric characters.");

            if (string.IsNullOrWhiteSpace(userUdpate.Name))
                return BadRequest("Username can't be empty.");

            if (string.IsNullOrWhiteSpace(userUdpate.Password))
                return BadRequest("Password can't be empty.");

            if (string.IsNullOrWhiteSpace(userUdpate.Email))
                return BadRequest("Mail can't be empty.");

            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (UserId == null) return Unauthorized();

                HttpResponseMessage response = await client.PutAsJsonAsync($"api/Users/{id}", userUdpate);

                // Check if the response status code is 2XX or 204 No Content
                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return NoContent();
                }
                else
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return BadRequest("UpdateUser failed");
                }
            }
        }


        // api/User/login
        [HttpPost("login")]
        public async Task<ActionResult<JWTAndUser>> Login(UserLogin model)
        {
            if (!model.Name.All(c => Char.IsLetterOrDigit(c)))
                return BadRequest("The username must only contain alphanumeric characters.");

            if (!model.Pass.All(c => Char.IsLetterOrDigit(c)))
                return BadRequest("The password must only contain alphanumeric characters.");

            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest("Username can't be empty.");

            if (string.IsNullOrWhiteSpace(model.Pass))
                return BadRequest("Password can't be empty.");

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                // Set the base address of the API you want to call
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/login", model);

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode)
                {
                    // You can deserialize the response content here if needed
                    var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                    var jwt = GenerateJwtToken(result.Id);
                    var userAndToken = new JWTAndUser() { Token = jwt, User = result };
                    return Ok(userAndToken);
                }
                else
                {
                    return BadRequest("Login failed. Please check your username and password.");
                }
            }
        }

        // api/User/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateModel model)
        {

            if (!model.Name.All(c => Char.IsLetterOrDigit(c)))
                return BadRequest("The username must only contain alphanumeric characters.");

            if (!model.Password.All(c => Char.IsLetterOrDigit(c)))
                return BadRequest("The password must only contain alphanumeric characters.");

            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest("Username can't be empty.");

            if (string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Password can't be empty.");

            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest("Mail can't be empty.");

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                // Set the base address of the API you want to call
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/register", model);

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode)
                {
                    // You can deserialize the response content here if needed
                    var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Register failed");
                }
            }
        }

        private string GenerateJwtToken(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", userId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("La chute n’est pas un échec, l’échec c’est de rester là où l’on est tombé"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "TaskProject",
                audience: "localhost:5000",
                claims: claims,
                expires: DateTime.Now.AddMinutes(3000),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // DELETE : api/User/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                // Concaténez l'ID à l'URL de l'API
                string urlWithId = $"http://localhost:5001/api/Users/{id}";

                // Envoie la requête HTTP DELETE
                HttpResponseMessage response = await client.DeleteAsync(urlWithId);

                // Vérifie la réponse
                if (response.IsSuccessStatusCode)
                {
                    return Ok($"Success {id}");
                }
                else
                {
                    return BadRequest("Delete failed");

                }
            }
        }

    }
}
