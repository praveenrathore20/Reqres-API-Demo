using ReqResDemo.Core.Models;
using ReqResDemo.Core.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Retry;
using System;

namespace ReqResDemo.Core.Services
{
    public class ExternalUserService
    {
        private readonly ReqResApiClient _apiClient;
        private readonly IMemoryCache? _cache;
        private readonly AsyncRetryPolicy _retryPolicy;
        public ExternalUserService(ReqResApiClient apiClient, IMemoryCache? cache = null)
        {
            _apiClient = apiClient;
            _cache = cache;
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            if (_cache != null && _cache.TryGetValue($"user_{userId}", out User? cachedUser) && cachedUser != null)
                return cachedUser;
            var user = await _retryPolicy.ExecuteAsync(() => _apiClient.GetUserByIdAsync(userId));
            if (_cache != null && user != null)
                _cache.Set($"user_{userId}", user, TimeSpan.FromMinutes(5));
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            if (_cache != null && _cache.TryGetValue("all_users", out List<User>? cachedUsers) && cachedUsers != null)
                return cachedUsers;
            var users = new List<User>();
            int page = 1;
            UserListResponse? response;
            do
            {
                response = await _retryPolicy.ExecuteAsync(() => _apiClient.GetUsersAsync(page));
                if (response?.Data != null)
                    users.AddRange(response.Data);
                page++;
            } while (response != null && page <= response.TotalPages);
            if (_cache != null)
                _cache.Set("all_users", users, TimeSpan.FromMinutes(5));
            return users;
        }
    }
}
