using System.Text.Json.Serialization;

namespace ReqResDemo.Core.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("first_Name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_Name")]
        public string LastName { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
    }

    public class UserListResponse
    {

        [JsonPropertyName("page")]
        public int Page { get; set; }


        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }


        [JsonPropertyName("total")]
        public int Total { get; set; }


        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }


        [JsonPropertyName("data")]
        public List<User> Data { get; set; }
    }

    public class SingleUserResponse
    {
        public User Data { get; set; }
    }
}
