using System.Text;
using System.Text.Json;

Console.WriteLine("=== HES-SO Print System Test Application ===");
Console.WriteLine("This application tests the Print Payment System APIs during development.\n");

// Configuration
var apiBaseUrl = "https://localhost:7065/api";
using var httpClient = new HttpClient();

try
{
    Console.WriteLine("🔧 Starting API tests...\n");

    // Test 1: Authentication tests
    Console.WriteLine("📋 TEST 1: Authentication Tests");
    await TestAuthentication();

    // Test 2: Quota operations
    Console.WriteLine("\n📋 TEST 2: Quota Operations");
    await TestQuotaOperations();

    // Test 3: Balance queries
    Console.WriteLine("\n📋 TEST 3: Balance Queries");
    await TestBalanceQueries();

    Console.WriteLine("\n✅ All tests completed!");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Test execution failed: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

async Task TestAuthentication()
{
    var testUsers = new[]
    {
        new { Username = "SCoppey", Password = "welcome", Type = "Student", ShouldSucceed = true },
        new { Username = "JDupont", Password = "password123", Type = "Student", ShouldSucceed = true },
        new { Username = "admin", Password = "admin123", Type = "Admin", ShouldSucceed = true },
        new { Username = "prof", Password = "profpass", Type = "Admin", ShouldSucceed = true },
        new { Username = "invalid", Password = "wrong", Type = "Student", ShouldSucceed = false }
    };

    foreach (var user in testUsers)
    {
        var endpoint = user.Type == "Admin" ? "login-admin" : "login-student";
        var result = await TestLogin(endpoint, user.Username, user.Password);

        var status = result == user.ShouldSucceed ? "✅ PASS" : "❌ FAIL";
        Console.WriteLine($"  {status} {user.Type} login: {user.Username}");
    }
}

async Task TestQuotaOperations()
{
    var quotaTests = new[]
    {
        new { Username = "SCoppey", Amount = 25, Description = "Add credit to existing student (integers only)" },
        new { Username = "JDupont", Amount = 10, Description = "Add credit to existing student (integers only)" },
        new { Username = "admin", Amount = 50, Description = "Add semester quota as admin (integers only)" }
    };

    foreach (var test in quotaTests)
    {
        var success = await TestAddQuota(test.Username, test.Amount);
        var status = success ? "✅ PASS" : "❌ FAIL";
        Console.WriteLine($"  {status} {test.Description}: {test.Amount} CHF to {test.Username}");
    }
}

async Task TestBalanceQueries()
{
    var users = new[] { "SCoppey", "JDupont", "NonExistentUser" };

    foreach (var username in users)
    {
        var balance = await TestGetBalance(username);
        var status = balance >= 0 ? "✅ PASS" : "❌ FAIL";
        Console.WriteLine($"  {status} Balance query for {username}: {balance:F2} CHF");
    }
}

async Task<bool> TestLogin(string endpoint, string username, string password)
{
    try
    {
        var loginData = new { Username = username, Password = password };
        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{apiBaseUrl}/Authentication/{endpoint}", content);
        return response.IsSuccessStatusCode;
    }
    catch
    {
        return false;
    }
}

async Task<bool> TestAddQuota(string username, int amount)
{
    try
    {
        var quotaData = new { Username = username, ChfAmount = amount };
        var json = JsonSerializer.Serialize(quotaData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{apiBaseUrl}/Quota/add-amount", content);
        return response.IsSuccessStatusCode;
    }
    catch
    {
        return false;
    }
}

async Task<double> TestGetBalance(string username)
{
    try
    {
        var response = await httpClient.GetAsync($"{apiBaseUrl}/Quota/balance/{username}");
        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var balanceData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            if (balanceData.TryGetProperty("balanceChf", out var balanceElement))
            {
                return balanceElement.GetDouble();
            }
        }
        return -1.0; // Indicates error
    }
    catch
    {
        return -1.0;
    }
}