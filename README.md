# ReqResDemo Solution

## Overview
This solution demonstrates a .NET Core component that interacts with the public [reqres.in](https://reqres.in/) API to fetch and process user data. It is structured for testability, configurability, and extensibility.

### Projects
- **ReqResDemo.Core**: Class library with API client, models, and service logic.
- **ReqResDemo.Tests**: xUnit test project for the core logic.
- **ReqResDemo.ConsoleApp**: Console app demonstrating usage.

## How to Build & Run

1. **Restore & Build**

   dotnet restore ReqResDemoSolution.sln
   dotnet build ReqResDemoSolution.sln

2. **Run Console Demo**

   dotnet run --project ReqResDemo.ConsoleApp/ReqResDemo.ConsoleApp.csproj
   
3. **Run Tests**

   dotnet test ReqResDemo.Tests/ReqResDemo.Tests.csproj
   

## Configuration
- The API base URL and API KEY is set in `ReqResDemo.ConsoleApp/appsettings.json`.
- You can change it as needed for different environments.

## Design Decisions
- **HttpClient** is used via DI and `IHttpClientFactory` for best practices.
- **Async/await** is used throughout for non-blocking IO.
- **Error handling**: Network errors, 404s, and deserialization issues are handled gracefully.
- **Pagination**: The service fetches all users across all pages.
- **Unit tests**: Use Moq to mock HttpClient responses.

## Extensibility
- To add caching, wrap the service methods with an in-memory cache (e.g., MemoryCache).
- For retries, use [Polly](https://github.com/App-vNext/Polly) with `AddPolicyHandler` on the HttpClient registration.
- For advanced configuration, use the Options pattern with strongly-typed settings.
- For Clean Architecture, separate interfaces and infrastructure further.

## Error Handling & Resilience
- API failures and non-success codes are handled with exceptions or nulls as appropriate.
- Retry logic can be added with Polly.
---

**Author:** Praveen Rathore
