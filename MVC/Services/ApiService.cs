using System.Text;
using System.Text.Json;

namespace MVC.Services
{
    public interface IApiService
    {
        Task<bool> AuthenticateAsync(string username, string password);
        Task<bool> AuthenticateAdminAsync(string username, string password);
        Task<bool> AuthenticateStudentAsync(string username, string password);
        Task<double> GetBalanceAsync(string username);
        Task<bool> AddAmountAsync(string username, double amount);
        Task<bool> AddSemesterQuotaAsync(string username, double amount);
        Task<bool> CheckUserExistsAsync(string username);
        Task<List<UserInfo>> GetAllUsersAsync();
    }

    /// <summary>
    /// API service for communicating with Print System WebAPI
    /// </summary>
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Support both local development and Azure deployment
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7065/api";
        }

        // Generic authentication method
        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/Authentication/login", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during authentication: {ex.Message}");
                return false;
            }
        }

        // Admin-specific authentication
        public async Task<bool> AuthenticateAdminAsync(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/Authentication/login-admin", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during admin authentication: {ex.Message}");
                return false;
            }
        }

        // Student-specific authentication
        public async Task<bool> AuthenticateStudentAsync(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/Authentication/login-student", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during student authentication: {ex.Message}");
                return false;
            }
        }

        // Get user's current balance in CHF
        public async Task<double> GetBalanceAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/Quota/balance/{username}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var balanceData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                    if (balanceData.TryGetProperty("balanceChf", out var balanceElement))
                    {
                        return balanceElement.GetDouble();
                    }
                }
                return 0.0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting balance: {ex.Message}");
                return 0.0;
            }
        }

        // Add credit to user account
        public async Task<bool> AddAmountAsync(string username, double amount)
        {
            try
            {
                var data = new { Username = username, ChfAmount = amount };
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/Quota/add-amount", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding amount: {ex.Message}");
                return false;
            }
        }

        // Add semester quota (admin function)
        public async Task<bool> AddSemesterQuotaAsync(string username, double amount)
        {
            try
            {
                var data = new { Username = username, ChfAmount = amount };
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/Quota/semester-quota", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding semester quota: {ex.Message}");
                return false;
            }
        }

        // Check if user exists in system
        public async Task<bool> CheckUserExistsAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/Authentication/user-exists/{username}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking user existence: {ex.Message}");
                return false;
            }
        }

        // Get all users for admin interface
        public async Task<List<UserInfo>> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/Users/all");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var usersData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                    if (usersData.TryGetProperty("users", out var usersElement))
                    {
                        var users = new List<UserInfo>();
                        foreach (var userElement in usersElement.EnumerateArray())
                        {
                            var user = new UserInfo
                            {
                                Username = userElement.GetProperty("username").GetString() ?? "",
                                FullName = userElement.GetProperty("fullName").GetString() ?? "",
                                Role = userElement.GetProperty("role").GetString() ?? "",
                                HasQuota = userElement.GetProperty("hasQuota").GetBoolean(),
                                Balance = userElement.GetProperty("balance").GetDouble()
                            };
                            users.Add(user);
                        }
                        return users;
                    }
                }
                return new List<UserInfo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all users: {ex.Message}");
                return new List<UserInfo>();
            }
        }
    }

    // User information model for admin interface
    public class UserInfo
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool HasQuota { get; set; }
        public double Balance { get; set; }
    }
}