using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using ReqResDemo.Core.Clients;
using ReqResDemo.Core.Models;
using ReqResDemo.Core.Services;
using System.Text.Json;
using Xunit;
using System.Threading;
using System.Collections.Generic;

namespace ReqResDemo.Tests
{
    public class ExternalUserServiceTests
    {
        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            var user = new User { Id = 1, FirstName = "Test", LastName = "User", Email = "test@reqres.in" };
            var response = new SingleUserResponse { Data = user };
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(response))
                });
            var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("https://reqres.in/api/") };
            var apiClient = new ReqResApiClient(httpClient);
            var service = new ExternalUserService(apiClient);
            var result = await service.GetUserByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserNotFound()
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });
            var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("https://reqres.in/api/") };
            var apiClient = new ReqResApiClient(httpClient);
            var service = new ExternalUserService(apiClient);
            var result = await service.GetUserByIdAsync(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers_AcrossPages()
        {
            var page1 = new UserListResponse
            {
                Page = 1,
                PerPage = 2,
                Total = 3,
                TotalPages = 2,
                Data = new List<User> { new User { Id = 1 }, new User { Id = 2 } }
            };
            var page2 = new UserListResponse
            {
                Page = 2,
                PerPage = 2,
                Total = 3,
                TotalPages = 2,
                Data = new List<User> { new User { Id = 3 } }
            };
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonSerializer.Serialize(page1)) })
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonSerializer.Serialize(page2)) });
            var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("https://reqres.in/api/") };
            var apiClient = new ReqResApiClient(httpClient);
            var service = new ExternalUserService(apiClient);
            var result = await service.GetAllUsersAsync();
            Assert.Equal(3, new List<User>(result).Count);
        }
    }
}
