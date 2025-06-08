namespace DAL.ExternalSystems
{
    /// <summary>
    /// Simulates university Active Directory authentication system
    /// </summary>
    public static class ActiveDirectory
    {
        // Test users for development - Students and Admins
        private static readonly Dictionary<string, UserInfo> ValidUsers = new()
        {
            // Students
            {"SCoppey", new UserInfo("welcome", "Student")},
            {"JDupont", new UserInfo("password123", "Student")},
            {"AMartin", new UserInfo("mdp123", "Student")},
            
            // Administrators
            {"admin", new UserInfo("admin123", "Admin")},
            {"prof", new UserInfo("profpass", "Admin")},
        };

        /// <summary>
        /// Authenticates user credentials against directory
        /// </summary>
        public static bool Auth(string username, string password)
        {
            try
            {
                Console.WriteLine($"[ActiveDirectory] Authenticating user: {username}");

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("[ActiveDirectory] Empty username or password");
                    return false;
                }

                System.Threading.Thread.Sleep(200); // Simulate network delay

                bool isAuthenticated = ValidUsers.ContainsKey(username) &&
                                     ValidUsers[username].Password == password;

                if (isAuthenticated)
                {
                    Console.WriteLine($"[ActiveDirectory] Authentication successful for {username} as {ValidUsers[username].Role}");
                }
                else
                {
                    Console.WriteLine($"[ActiveDirectory] Authentication failed for {username}");
                }

                return isAuthenticated;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ActiveDirectory] Error during authentication: {ex.Message}");
                return false;
            }
        }

        // Get user role (Student/Admin)
        public static string GetUserRole(string username)
        {
            return ValidUsers.ContainsKey(username) ? ValidUsers[username].Role : "Unknown";
        }

        // Check if user has admin privileges
        public static bool IsAdmin(string username)
        {
            return ValidUsers.ContainsKey(username) && ValidUsers[username].Role == "Admin";
        }

        // Check if user is a student
        public static bool IsStudent(string username)
        {
            return ValidUsers.ContainsKey(username) && ValidUsers[username].Role == "Student";
        }

        // Verify username exists in directory
        public static bool UserExists(string username)
        {
            return ValidUsers.ContainsKey(username);
        }

        // Get all registered usernames for admin interface
        public static IEnumerable<string> GetAllUsernames()
        {
            return ValidUsers.Keys;
        }

        // Get complete user information
        public static UserInfo? GetUserInfo(string username)
        {
            return ValidUsers.ContainsKey(username) ? ValidUsers[username] : null;
        }
    }

    // User credentials and role information
    public class UserInfo
    {
        public string Password { get; set; }
        public string Role { get; set; }

        public UserInfo(string password, string role)
        {
            Password = password;
            Role = role;
        }
    }
}