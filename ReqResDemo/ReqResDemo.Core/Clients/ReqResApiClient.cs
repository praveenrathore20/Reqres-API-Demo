using System.Net.Http.Headers;
using System.Threading.Tasks;
using ReqResDemo.Core.Models;
using System.Text.Json;

namespace ReqResDemo.Core.Clients
{
    public class ReqResApiClient
    {
        private readonly HttpClient _httpClient;
        public ReqResApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserListResponse> GetUsersAsync(int page)
        {
            var response = await _httpClient.GetAsync($"users?page={page}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"users/{userId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleUserResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result?.Data;
        }
    }
}
