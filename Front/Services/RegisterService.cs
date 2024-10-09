using Front.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;

namespace Front.Services
{
    public class RegisterService
    {
        private readonly HttpClient _httpClient;

        public RegisterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(UserDTO? user, string error)> CreateUser(string username, string password, string email)
        {
            UserCreateModel userregister = new()
            {
                Password = password,
                Name = username,
                Email = email,
            };
            
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/User/register", userregister);

            // Check if the response status code is 200 (OK)
            if (response.IsSuccessStatusCode)
            {
                // You can deserialize the response content here if needed
                var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                return (result, "") ;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return (null, error);
            }
        }
    }
}

