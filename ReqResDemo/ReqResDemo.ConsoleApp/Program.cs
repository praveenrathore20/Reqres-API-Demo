using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReqResDemo.Core.Clients;
using ReqResDemo.Core.Services;

namespace ReqResDemo.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);
            services.AddHttpClient<ReqResApiClient>(client =>
            {
                client.BaseAddress = new Uri(config["ReqResApi:BaseUrl"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("x-api-key", "reqres-free-v1");
            });
            services.AddMemoryCache();
            services.AddTransient<ExternalUserService>(sp =>
                new ExternalUserService(
                    sp.GetRequiredService<ReqResApiClient>(),
                    sp.GetRequiredService<IMemoryCache>()
                )
            );

            var provider = services.BuildServiceProvider();
            var userService = provider.GetRequiredService<ExternalUserService>();

            var allUsers = await userService.GetAllUsersAsync();
            foreach (var user in allUsers)
            {
                Console.WriteLine($"{user.Id}: {user.FirstName} {user.LastName} - {user.Email}");
            }

            var singleUser = await userService.GetUserByIdAsync(2);
            if (singleUser != null)
                Console.WriteLine($"\nSingle User: {singleUser.FirstName} {singleUser.LastName} - {singleUser.Email}");
            else
                Console.WriteLine("User not found.");
        }
    }
}
