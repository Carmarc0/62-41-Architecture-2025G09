namespace DAL.ExternalSystems
{
    /// <summary>
    /// Interface to external print system for quota synchronization
    /// </summary>
    public static class PrintSystemAPI
    {
        /// <summary>
        /// Sends page quota updates to external print system
        /// </summary>
        public static bool AddPages(float pages, string username)
        {
            Console.WriteLine($"[PrintSystemAPI] Adding {pages} pages to user {username}");

            Thread.Sleep(100); // Simulate network delay

            // In real implementation: HTTP POST to print system
            Console.WriteLine($"[PrintSystemAPI] Successfully added {pages} pages to {username}");
            return true;
        }

        /// <summary>
        /// Retrieves current page balance from external print system
        /// </summary>
        public static float GetAvailableAmount(string username)
        {
            Console.WriteLine($"[PrintSystemAPI] Retrieving balance for user {username}");

            Thread.Sleep(50); // Simulate network delay

            // In real implementation: HTTP GET from print system
            float mockBalance = 125.0f;
            Console.WriteLine($"[PrintSystemAPI] Balance for {username}: {mockBalance} pages");
            return mockBalance;
        }
    }
}