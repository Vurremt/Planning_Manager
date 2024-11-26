using Front.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;

namespace Front.Services
{
    public class LoginService
    {
        private readonly HttpClient _httpClient;
        private ProtectedLocalStorage _sessionStorage;

        public LoginService(HttpClient httpClient, ProtectedLocalStorage sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
        }

    public async Task<(UserDTO? user, string error)> AuthenticateUser(string username, string password)
        {
            UserLogin userlogin = new()
            {
                Name = username,
                Pass = password,
            };

            var response =  await _httpClient.PostAsJsonAsync("http://localhost:5000/api/User/login", userlogin);

            // Check if the response status code is 200 (OK)
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JWTAndUser>();

                if (result != null && result.User != null)
                {
                    await _sessionStorage.SetAsync("jwt", result.Token);
                    Console.Out.WriteLine("Token JWT : " + result.Token);
                    return (result.User, "");
                }
            }
            var error = await response.Content.ReadAsStringAsync();
            return (null, error);
        }
    }
}

