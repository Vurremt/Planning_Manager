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
using System.Text.RegularExpressions;

namespace Front.Services
{
    public class EventService
    {
        private readonly HttpClient _httpClient;
        private ProtectedLocalStorage _sessionStorage;

        public EventService(HttpClient httpClient, ProtectedLocalStorage sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
        }

        public async Task<EventModel[]> GetAllEvents()
        {
            try
            {
                var token = await _sessionStorage.GetAsync<string>("jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:5000/api/Event");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<EventModel[]>();

                    return result ?? Array.Empty<EventModel>();
                }
                else
                {
                    return Array.Empty<EventModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllEvents: {ex.Message}");
                return Array.Empty<EventModel>();
            }
        }

        public async Task<(EventModel? e, string? error)> CreateEvent(string title, string place, string? description, DateTime startingDate, int Duration, int groupId)
        {
            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            var token = await _sessionStorage.GetAsync<string>("jwt");

            if (token.Success)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var task = new EventCreateModel()
                {
                    Title = title,
                    Place = place,
                    Description = description,
                    StartingDate = startingDate,
                    Duration = Duration,
                    GroupId = groupId
                };
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/Task/create", task);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<EventModel>();

                    return (result, "");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return (null, error);
                }
            }
            return (null, "Invalid Token");
        }

        public async Task<(EventModel? e, string? error)> UpdateEvent(EventModel newEvent)
        {
            var token = await _sessionStorage.GetAsync<string>("jwt");

            if (token.Success)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var e = new EventCreateModel()
                {
                    Title = newEvent.Title,
                    Place = newEvent.Place,
                    Description = newEvent.Description,
                    StartingDate = newEvent.StartingDate,
                    Duration = newEvent.Duration,
                    GroupId = newEvent.GroupId
                };

                var jwt = await _sessionStorage.GetAsync<string>("jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Value);
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"http://localhost:5000/api/Event/update/{newEvent.Id}", e);

                Console.WriteLine(response.Content.ToString());
                Console.WriteLine(response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<EventModel>();
                    return (result, "");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return (null, error);
                }
            }
            Console.WriteLine("Erreur : Token JWT impossible à récupérer");
            return (null, "Invalid Token");
        }

        public async Task DeleteEvent(int id)
        {

            var token = await _sessionStorage.GetAsync<string>("jwt");

            if (token.Success)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                HttpResponseMessage response = await _httpClient.DeleteAsync($"http://localhost:5000/api/Event/delete/{id}");

                Console.WriteLine(response.Content.ToString());
                Console.WriteLine(response.StatusCode);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Error deleting");
                }
            }
            else
            {
                Console.WriteLine("Erreur : Token JWT impossible à récupérer");
                throw new Exception("Error deleting");
            }
        }

    }
}