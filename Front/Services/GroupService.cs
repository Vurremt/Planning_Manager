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
    public class GroupService
    {
        private readonly HttpClient _httpClient;
        private ProtectedLocalStorage _sessionStorage;

        public GroupService(HttpClient httpClient, ProtectedLocalStorage sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
        }

        public async Task<GroupModel?> GetGroupById(int id)
        {
            try
            {
                var token = await _sessionStorage.GetAsync<string>("jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                HttpResponseMessage response = await _httpClient.GetAsync($"http://localhost:5000/api/Group/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GroupModel>();

                    return result ?? null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetGroupById: {ex.Message}");
                return null;
            }
        }


        public async Task<bool> IsUserAdmin(int id)
        {
            try
            {
                var token = await _sessionStorage.GetAsync<string>("jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                HttpResponseMessage response = await _httpClient.GetAsync($"http://localhost:5000/api/Group/admin/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<bool>();

                    return result;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetGroupById: {ex.Message}");
                return false;
            }
        }
        public async Task<GroupModel[]> GetFollowedGroups()
        {
            try
            {
                var token = await _sessionStorage.GetAsync<string>("jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:5000/api/Group/followed");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GroupModel[]>();

                    return result ?? Array.Empty<GroupModel>();
                }
                else
                {
                    return Array.Empty<GroupModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllGroups: {ex.Message}");
                return Array.Empty<GroupModel>();
            }
        }

        public async Task<GroupModel[]> GetAllGroups()
        {
            try
            {
                var token = await _sessionStorage.GetAsync<string>("jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:5000/api/Group");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GroupModel[]>();

                    return result ?? Array.Empty<GroupModel>();
                }
                else
                {
                    return Array.Empty<GroupModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllGroups: {ex.Message}");
                return Array.Empty<GroupModel>();
            }
        }

        public async Task<GroupModel?> AddUserToGroup(int id)
        {
            try
            {
                var token = await _sessionStorage.GetAsync<string>("jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                HttpResponseMessage response = await _httpClient.GetAsync($"http://localhost:5000/api/Group/{id}/add");
                Console.WriteLine("response");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("ok");
                    var result = await response.Content.ReadFromJsonAsync<GroupModel>();

                    return result ?? null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddUserToGroup: {ex.Message}");
                return null;
            }
        }

        public async Task<GroupModel?> RemoveUserFromGroup(int id)
        {
            try
            {
                var token = await _sessionStorage.GetAsync<string>("jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                HttpResponseMessage response = await _httpClient.GetAsync($"http://localhost:5000/api/Group/{id}/remove");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GroupModel>();

                    return result ?? null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RemoveUserFromGroup: {ex.Message}");
                return null;
            }
        }
        public async Task<(GroupModel? group, string? error)> CreateGroup(string title, string? description)
        {
            var jwt = await _sessionStorage.GetAsync<string>("jwt");
            var token = await _sessionStorage.GetAsync<string>("jwt");

            if (token.Success)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var group = new GroupCreateModel()
                {
                    Titre = title,
                    Description = description,
                    ManagerIds = [],
                    SubscriberIds = []
                };
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/Group/create", group);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GroupModel>();

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

        public async Task<(GroupModel? group, string? error)> UpdateGroup(GroupModel newGroup)
        {
            var token = await _sessionStorage.GetAsync<string>("jwt");

            if (token.Success)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var group = new GroupCreateModel()
                {
                    Titre = newGroup.Titre,
                    Description = newGroup.Description,
                    ManagerIds = newGroup.ManagerIds,
                    SubscriberIds = newGroup.SubscriberIds,
                };

                var jwt = await _sessionStorage.GetAsync<string>("jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Value);
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"http://localhost:5000/api/Group/update/{newGroup.Id}", group);

                Console.WriteLine(response.Content.ToString());
                Console.WriteLine(response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GroupModel>();
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

        public async Task DeleteGroup(int id)
        {

            var token = await _sessionStorage.GetAsync<string>("jwt");

            if (token.Success)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                HttpResponseMessage response = await _httpClient.DeleteAsync($"http://localhost:5000/api/Group/delete/{id}");

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