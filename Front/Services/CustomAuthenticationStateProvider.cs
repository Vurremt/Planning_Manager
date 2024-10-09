using Front.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace Front.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private ProtectedLocalStorage _sessionStorage;

        public CustomAuthenticationStateProvider(ProtectedLocalStorage protectedSessionStorage)
        {
            _sessionStorage = protectedSessionStorage;
        }

        public async Task<ClaimsPrincipal> MarkUserAsAuthenticated(UserDTO user)
        {
            await _sessionStorage.SetAsync("User", user);
            var claims = new[] {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            _currentUser = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            return _currentUser;
        }
        public async Task<ClaimsPrincipal> Logout()
        {
            await _sessionStorage.DeleteAsync("User");
            await _sessionStorage.DeleteAsync("jwt");
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return _currentUser;
        }

        public async Task<ClaimsPrincipal> DeleteAccount()
        {
            var userSession = await _sessionStorage.GetAsync<UserDTO>("User");
            if (userSession.Success && userSession.Value != null)
            {
                var user = userSession.Value;
                await _sessionStorage.DeleteAsync("User");

                using (HttpClient client = new HttpClient())
                {
                    var token = await _sessionStorage.GetAsync<string>("jwt");
                    if(token.Success)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                        // Concaténez l'ID à l'URL de l'API
                        string urlWithId = $"http://localhost:5000/api/User/{user.Id}";

                        // Envoie la requête HTTP DELETE
                        HttpResponseMessage response = await client.DeleteAsync(urlWithId);

                        // Vérifie la réponse
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Suppression réussie pour l'ID {user.Id}");
                        }
                        else
                        {
                            Console.WriteLine($"Erreur lors de la suppression. Code de statut : {response.StatusCode}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Erreur : Token JWT impossible à récupérer");
                    }
                    
                }
            }

            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());


            return _currentUser;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userSession = await _sessionStorage.GetAsync<UserDTO>("User");
            if(userSession.Success && userSession.Value != null)
            {
                var user = userSession.Value;
                var claims = new[] {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, "User")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                _currentUser = new ClaimsPrincipal(identity);
            } else {
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
            return await System.Threading.Tasks.Task.FromResult(new AuthenticationState(_currentUser));
        }
    }
}
